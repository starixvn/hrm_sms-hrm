import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { Formik } from 'formik';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import Select from 'react-select';
import LangHelper from '@jsroot/common/langHelper';
import { PolicyOptions } from '@jsroot/common/constants';
import { validateFormik } from '../common.functions';

export class Update extends Component {
    constructor(props) {
        super(props);
        this.state = {
            siteOptions: null,
            site: null,
            parentOptions: null,
            parent: null
        };
    }

    componentDidMount() {
        this.handleSiteOptions();
        this.handleParentOptions(this.props.functions);
    }

    handleSiteOptions = () =>{
        const {functionSelected, sites} = this.props;
        const siteOptions = sites.map(item =>{
            return {
                value: item.siteId,
                label: item.siteName
            };
        });
        const site = siteOptions.filter(x=>x.value === functionSelected.siteId)[0];
        this.setState({siteOptions, site});
    }

    handleParentOptions = parents =>{
        const defaultParent = [{
            functionId: null,
            functionName: 'None'
        }];
        const functionSelected = this.props.functionSelected;
        const parentCopy = [...defaultParent, ...parents];
        const parentOptions = parentCopy.map(item =>{
            return {
                value: item.functionId,
                label: item.functionName
            };
        });
        const parentFilter = parentOptions.filter(x=>x.value === functionSelected.parentId);
        const parent = (functionSelected.parentId === 0 || parentFilter.length === 0) ? parentOptions[0] : parentFilter[0];
        this.setState({parentOptions, parent});
    }

    handleOpenConfirmRegenerateKeyModalDialog = () => {
        this.props.setConfirmRegenerateKeyModalDialog({ show: true});
    }

    handleCloseConfirmRegenerateKeyModalDialog = () => {
        this.props.setConfirmRegenerateKeyModalDialog({ show: false });
    }


    handleCloseModalDialog = () => {
        this.props.setUpdateModalDialog({ show: false });
        if (this.props.updateFunctionResult) {
            this.props.setUpdateFunctionResult(null);
        }
    };

    handleChangeSite = site =>{
        this.setState({site});
        this.props.getParentById(site.value).then(data => { this.handleParentOptions(data.data); });
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const {site, parent} = this.state;
        const params = {
            functionId: this.props.functionSelected.functionId,
            functionName: values.functionName,
            functionKey: values.functionKey,
            siteId: site.value,
            parentId: parent.value,
            policy: values.policy.value,
            displayOrder: values.order,
            description: values.description,
            isActive: values.isActive
        };

        this.props.updateFunction(params).then(() => {
            if (this.props.updateFunctionResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.setGetSiteParams(params.siteId);
                    this.props.getFunctionById(params.siteId);
                }, 3000);
            } else {
                setSubmitting(false);
            }
        });
    };

    handleRegenerateFunctionKey = () => {
        const functionKey = this.formik.values.functionName.replace(/\s/g, '');
        this.formik.setFieldValue('functionKey', functionKey);
        this.handleCloseConfirmRegenerateKeyModalDialog();
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        const {siteOptions, site, parent, parentOptions} = this.state;
        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.updateFunctionResult &&
                        <ResponseMessage isSuccessful={this.props.updateFunctionResult.isSuccessful}
                            message={this.props.updateFunctionResult.message || LangHelper.getResource('UpdateFunctionSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="functionName">
                            {LangHelper.getResource('FunctionName')}
                        </Form.Label>
                        <Col sm="7" md="8">
                            <Form.Control type="text" size="sm" value={values.functionName}
                                name="functionName" id="functionName"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.functionName && errors.functionName}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="functionKey">
                            {LangHelper.getResource('FunctionKey')}
                        </Form.Label>
                        <Col sm="7" md="8">
                            <Form.Control type="text" size="sm" value={values.functionKey}
                                disabled
                                name="functionKey" id="functionKey"
                                className='shadow-none' autoComplete="off"
                                onChange={handleChange}
                            />
                        </Col>
                        <OverlayTrigger overlay={<Tooltip id="tooltip-refresh-token">Regenerate Key</Tooltip>}>
                                <span className="text-center rounded-circle fa fa-refresh icon-edit"
                                    onClick={this.handleOpenConfirmRegenerateKeyModalDialog}
                                />
                            </OverlayTrigger>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="site">
                            {LangHelper.getResource('Site')}
                        </Form.Label>
                        <Col sm="7" md="8">
                            <Select className={`react-select-container ${errors.siteId && errors.siteId ? 'is-invalid' : ''}`}
                                classNamePrefix="react-select" isSearchable={false}
                                options={siteOptions} value={site}
                                onChange={value => this.handleChangeSite(value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="description">
                            {LangHelper.getResource('Description')}
                        </Form.Label>
                        <Col sm="7" md="8">
                            <Form.Control type="text" size="sm" value={values.description}
                                name="description" id="description"
                                className="shadow-none" autoComplete="off"
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="policy">
                            {LangHelper.getResource('Policy')}
                        </Form.Label>
                        <Col sm="7" md="8">
                            <Select className='react-select-container' classNamePrefix="react-select" isSearchable={false}
                                options={PolicyOptions} value={values.policy}
                                onChange={value => setFieldValue('policy', value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="parent">
                            {LangHelper.getResource('Parent')}
                        </Form.Label>
                        <Col sm="7" md="8">
                            <Select className='react-select-container' classNamePrefix="react-select" isSearchable
                                options={parentOptions} value={parent}
                                onChange={value => this.setState({parent: value})}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="order">
                            {LangHelper.getResource('Order')}
                        </Form.Label>
                        <Col sm="7" md="8">
                            <Form.Control type="number" size="sm" value={values.order}
                                name="order" id="order"
                                className="shadow-none" autoComplete="off"
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="isActive" className="required">
                            {LangHelper.getResource('Status')}
                        </Form.Label>
                        <Col sm="7" md="8">
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
        const { functionSelected } = this.props;
        const policy = PolicyOptions.filter(x=>x.value === functionSelected.policy);
        const initialValues = {
            functionName: functionSelected.functionName,
            functionKey: functionSelected.functionKey,
            description: functionSelected.description,
            policy: policy,
            order: functionSelected.displayOrder,
            isActive: functionSelected.isActive
        };
        return (
            <>
                <Modal show
                    backdrop="static" keyboard={false} size="lg" centered
                    onHide={this.handleCloseModalDialog}
                >
                    <Modal.Header closeButton>
                        <Modal.Title as="div">{LangHelper.getResource('UpdateFunction')}</Modal.Title>
                    </Modal.Header>
                    <Formik
                        innerRef={el => this.formik = el}
                        initialValues={initialValues}
                        validate={values =>validateFormik(values, this.state.site.value)}
                        onSubmit={this.handleSubmit}
                    >
                        {this.renderForm}
                    </Formik>
                </Modal>
                {
                    this.props.confirmModalRegenerateKeyDialog.show &&
                    <SweetModalDialog size='sm'
                        title={LangHelper.getResource('Confirmation')}
                        contents={
                            <div>Do you want to regenerate function key ?</div>
                        }
                        buttons={[
                            <Button variant="primary" size="sm" onClick={this.handleRegenerateFunctionKey}>
                                {LangHelper.getResource('OK')}
                            </Button>,
                            <Button variant="secondary" size="sm" onClick={this.handleCloseConfirmRegenerateKeyModalDialog}>
                                {LangHelper.getResource('Cancel')}
                            </Button>
                        ]}
                        onClose={this.handleCloseConfirmRegenerateKeyModalDialog}
                    />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { params, sites, functions, functionSelected, updateFunctionResult, confirmModalDialog, confirmModalRegenerateKeyDialog } = state;
    return { params, sites, functions, functionSelected, updateFunctionResult, confirmModalDialog, confirmModalRegenerateKeyDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(Update);