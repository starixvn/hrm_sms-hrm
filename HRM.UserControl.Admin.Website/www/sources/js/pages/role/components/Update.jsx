import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import Select from 'react-select';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import LangHelper from '@jsroot/common/langHelper';
import { validateFormik } from '../common.functions';

export class UpdateRole extends Component {
    constructor(props) {
        super(props);
    }

    handleCloseModalDialog = () => {
        this.props.setUpdateModalDialog({ show: false });
        if (this.props.updateRoleResult) {
            this.props.setUpdateRoleResult(null);
        }
    };

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const params = {
            roleId: values.roleId,
            name: values.name,
            siteId: values.site.value,
            isDefault: values.isDefault,
            isActive: values.isActive
        };
        this.props.updateRole(params).then(() => {
            if (this.props.updateRoleResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.getRoleBySiteId(this.props.getRoleParams);
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
        const siteOptions = this.props.sites.map(item => {
            return { value: item.siteId, label: item.siteName };
        });

        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.updateRoleResult &&
                        <ResponseMessage isSuccessful={this.props.updateRoleResult.isSuccessful}
                            message={this.props.updateRoleResult.message || LangHelper.getResource('UpdateRoleSuccessfully')}
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
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('Site')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className={`react-select-container ${errors && errors.site ? 'is-invalid' : ''}`}
                                classNamePrefix="react-select" isSearchable={false}
                                options={siteOptions} value={values.site} placeholder={LangHelper.getResource('PleaseSelect')}
                                onChange={value => setFieldValue('site', value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="isDefault">{LangHelper.getResource('Default')}</Form.Label>
                        <Col sm="8" md="9">
                            <Form.Check type="checkbox"
                                checked={values.isDefault}
                                name="isDefault" id="isDefault"
                                className="shadow-none"
                                onChange={handleChange}
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
        const { sites, updateModalDialog } = this.props;
        const role = updateModalDialog.role;
        let site = sites.find(item => item.siteId === role.siteId);
        const initialValues = {
            roleId: role.roleId,
            name: role.name,
            site: site ? { value: site.siteId, label: site.siteName } : null,
            isDefault: role.isDefault,
            isActive: role.isActive
        };

        return (
            <>
                <Modal show
                    backdrop="static" keyboard={false} size="lg" centered
                    onHide={this.handleCloseModalDialog}
                >
                    <Modal.Header closeButton>
                        <Modal.Title as="div">{LangHelper.getResource('UpdateRole')}</Modal.Title>
                    </Modal.Header>
                    <Formik
                        initialValues={initialValues}
                        validate={validateFormik}
                        onSubmit={this.handleSubmit}
                    >
                        {this.renderForm}
                    </Formik>
                </Modal>
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getRoleParams, sites, updateModalDialog, updateRoleResult } = state;
    return { getRoleParams, sites, updateModalDialog, updateRoleResult };
};

export default connect(mapStateToProps, mapDispatchToProps)(UpdateRole);