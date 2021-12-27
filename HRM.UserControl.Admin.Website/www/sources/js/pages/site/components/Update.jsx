import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { Formik } from 'formik';
import Select from 'react-select';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';
import { validateFormik } from '../common.functions';

export class UpdateSite extends Component {
    constructor(props) {
        super(props);
    }

    handleOpenConfirmModalDialog = () => {
        this.props.setConfirmModalDialog({ show: true });
    }

    handleCloseConfirmModalDialog = () => {
        this.props.setConfirmModalDialog({ show: false });
    }

    generateToken = () => {
        Site.blockUI();
        this.props.generateToken().then(response => {
            Site.unblockUI()
            this.handleCloseConfirmModalDialog();
            this.formik.setFieldValue('accessToken', response.data);
        });
    }

    handleCloseModalDialog = () => {
        this.props.setUpdateModalDialog({ show: false });
        if (this.props.updateSiteResult) {
            this.props.setUpdateSiteResult(null);
        }
    };

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const params = {
            companyId: values.company.value,
            siteId: values.siteId,
            siteName: values.name,
            accessToken: values.accessToken,
            siteURL: values.url,
            isActive: values.isActive
        };
        this.props.updateSite(params).then(() => {
            if (this.props.updateSiteResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.getAllSite(this.props.getAllSiteParams);
                }, 3000);
            } else {
                setSubmitting(false);
            }
        });
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        isSubmitting
    }) => {
        const companyOptions = window.companies ? window.companies.map(item => {
            return { value: item.companyId, label: item.name };
        }) : [];

        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.updateSiteResult &&
                        <ResponseMessage isSuccessful={this.props.updateSiteResult.isSuccessful}
                            message={this.props.updateSiteResult.message || LangHelper.getResource('UpdateSiteSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="name">
                            {LangHelper.getResource('Name')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.name}
                                name="name" id="name"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.name && errors.name}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="accessToken">
                            {LangHelper.getResource('AccessToken')}
                        </Form.Label>
                        <Col sm="8" md="9" className="d-flex">
                            <Form.Control type="text" size="sm" value={values.accessToken}
                                name="accessToken" id="accessToken"
                                className="shadow-none mr-2" autoComplete="off"
                                disabled
                                isInvalid={touched.accessToken && errors.accessToken}
                            />
                            <OverlayTrigger overlay={<Tooltip id="tooltip-refresh-token">Regenerate token</Tooltip>}>
                                <span className="text-center rounded-circle fa fa-refresh icon-edit"
                                    onClick={this.handleOpenConfirmModalDialog}
                                />
                            </OverlayTrigger>

                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="url">
                            {LangHelper.getResource('URL')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.url}
                                name="url" id="url"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.url && errors.url}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('Company')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className={`react-select-container ${errors && errors.company ? 'is-invalid' : ''}`}
                                classNamePrefix="react-select" isSearchable={false}
                                options={companyOptions} value={values.company} placeholder={LangHelper.getResource('PleaseSelect')}
                                onChange={value => setFieldValue('company', value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="isActive">{LangHelper.getResource('Active')}</Form.Label>
                        <Col sm="8" md="9">
                            <Form.Check type="checkbox"
                                checked={values.isActive}
                                name="isActive" id="isActive"
                                className="shadow-none"
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button type="submit" variant="primary" size="sm" disabled={isSubmitting}>
                        {LangHelper.getResource('Save')}
                    </Button>
                    <Button variant="secondary" size="sm" onClick={this.handleCloseModalDialog}>
                        {LangHelper.getResource('Cancel')}
                    </Button>
                </Modal.Footer>
            </Form>
        );
    };

    render() {
        const { updateModalDialog, confirmModalDialog } = this.props;
        const site = updateModalDialog.site;

        const initialValues = {
            company: { value: site.companyId, label: site.companyName },
            siteId: site.siteId,
            name: site.siteName,
            url: site.siteURL,
            accessToken: site.accessToken,
            isActive: site.isActive
        };
        return (
            <>
                <Modal show
                    backdrop="static" keyboard={false} size="lg" centered
                    onHide={this.handleCloseModalDialog}
                >
                    <Modal.Header closeButton>
                        <Modal.Title as="div">{LangHelper.getResource('UpdateSite')}</Modal.Title>
                    </Modal.Header>
                    <Formik
                        innerRef={el => this.formik = el}
                        initialValues={initialValues}
                        validate={validateFormik}
                        onSubmit={this.handleSubmit}
                    >
                        {this.renderForm}
                    </Formik>
                </Modal>
                {
                    confirmModalDialog.show &&
                    <SweetModalDialog size='sm'
                        title={LangHelper.getResource('Confirmation')}
                        contents={
                            <div>Are you sure you want to regenerate access token?</div>
                        }
                        buttons={[
                            <Button variant="primary" size="sm" onClick={this.generateToken}>
                                {LangHelper.getResource('OK')}
                            </Button>,
                            <Button variant="secondary" size="sm" onClick={this.handleCloseConfirmModalDialog}>
                                {LangHelper.getResource('Cancel')}
                            </Button>
                        ]}
                        onClose={this.handleCloseConfirmModalDialog}
                    />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAllSiteParams, updateModalDialog, updateSiteResult, confirmModalDialog } = state;
    return { getAllSiteParams, updateModalDialog, updateSiteResult, confirmModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(UpdateSite);