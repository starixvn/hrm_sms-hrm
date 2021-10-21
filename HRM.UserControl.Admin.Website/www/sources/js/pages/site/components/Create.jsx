import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import LangHelper from '@jsroot/common/langHelper';
import { validateFormik } from '../common.functions';

export class CreateSite extends Component {
    constructor(props) {
        super(props);
    }

    handleCloseModalDialog = () => {
        this.props.setCreateModalDialog({ show: false });
        if (this.props.createSiteResult) {
            this.props.setCreateSiteResult(null);
        }
    };

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const params = {
            siteName: values.name,
            siteURL: values.url,
            isActive: values.isActive
        };
        this.props.createSite(params).then(() => {
            if (this.props.createSiteResult.isSuccessful) {
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
        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.createSiteResult &&
                        <ResponseMessage isSuccessful={this.props.createSiteResult.isSuccessful}
                            message={this.props.createSiteResult.message || LangHelper.getResource('CreateSiteSuccessfully')}
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
                        <Form.Label column sm="4" md="3"
                            htmlFor="isActive"
                        >
                            {LangHelper.getResource('Status')}
                        </Form.Label>
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
        const initialValues = {
            name: '',
            url: '',
            isActive: true
        };
        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.handleCloseModalDialog}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('CreateSite')}</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={initialValues}
                    validate={validateFormik}
                    onSubmit={this.handleSubmit}
                >
                    {this.renderForm}
                </Formik>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAllSiteParams, createSiteResult } = state;
    return { getAllSiteParams, createSiteResult };
};

export default connect(mapStateToProps, mapDispatchToProps)(CreateSite);