import React, { Component } from 'react';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import LangHelper from '@jsroot/common/langHelper';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import DatePicker from "@jsroot/components/date-picker/index";
import ResponseMessage from '@jsroot/components/ResponseMessage';
import { validateFormik } from '../common.functions';

export default class Create extends Component {
    constructor(props) {
        super(props);
    }

    handleCloseModalDialog = () => {
        this.props.handleModalCreateDialog( false );
    };

    handleSubmit = (values) => {
        const params = {
            name: values.name,
            settings: [
                {
                    processId: 1,
                    fromDate: values.fromDateUnitHead,
                    todate: values.toDateUnitHead,
                },
                {
                    processId: 2,
                    fromDate: values.fromDateVerifier,
                    todate: values.toDateVerifier,
                },
                {
                    processId: 3,
                    fromDate: values.fromDateApprover,
                    todate: values.toDateApprover,
                }
            ]
        };
        this.props.createPeriod(params);
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        setFieldValue,
        handleSubmit,
        isSubmitting
    }) => {
        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage className="summary-errors" touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.createResult &&
                        <ResponseMessage isSuccessful={this.props.createResult.isSuccessful}
                            message={this.props.createResult.message || LangHelper.getResource('CreatePeriodSuccessfully')}
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
                        <Form.Label column sm="4" md="3">
                        </Form.Label>
                        <Col sm="8" md="9" className="label-from-to-date">
                            <Form.Label className="label-from-date">{LangHelper.getResource('FromDate')}</Form.Label>
                            <Form.Label >{LangHelper.getResource('ToDate')}</Form.Label>
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('UnitHead')}
                        </Form.Label>
                        <Col sm="8" md="9" className="dataPicker-from-to-date">
                            <DatePicker size="sm" selected={values.fromDateUnitHead} value={values.fromDateUnitHead}
                                name="fromDateUnitHead" id="fromDateUnitHead"
                                isInvalid={touched.unitHead && errors.unitHead ? "is-invalid" : ""}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('fromDateUnitHead', value)} />
                            <DatePicker size="sm" selected={values.toDateUnitHead} value={values.toDateUnitHead}
                                name="toDateUnitHead" id="toDateUnitHead"
                                isInvalid={touched.unitHead && errors.unitHead ? "is-invalid" : ""}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('toDateUnitHead', value)} />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('Verifier')}
                        </Form.Label>
                        <Col sm="8" md="9" className="dataPicker-from-to-date">
                            <DatePicker size="sm" selected={values.fromDateVerifier} value={values.fromDateVerifier}
                                name="fromDateVerifier" id="fromDateVerifier"
                                isInvalid={touched.verifier && errors.verifier ? "is-invalid" : ""}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('fromDateVerifier', value)}/>
                            <DatePicker size="sm" selected={values.toDateVerifier} value={values.toDateVerifier}
                                dateFormat="dd MMM,yyyy"
                                name="toDateVerifier" id="toDateVerifier"
                                isInvalid={touched.verifier && errors.verifier ? "is-invalid" : ""}
                                onChange={value => setFieldValue('toDateVerifier', value)}/>
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('Approver')}
                        </Form.Label>
                        <Col sm="8" md="9" className="dataPicker-from-to-date">
                            <DatePicker size="sm" selected={values.fromDateApprover} value={values.fromDateApprover}
                                name="fromDateApprover" id="fromDateApprover"
                                isInvalid={touched.approver && errors.approver ? "is-invalid" : ""}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('fromDateApprover', value)}/>
                            <DatePicker size="sm" selected={values.toDateApprover} value={values.toDateApprover}
                                name="toDateApprover" id="toDateApprover"
                                isInvalid={touched.approver && errors.approver ? "is-invalid" : ""}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('toDateApprover', value)}/>

                        </Col>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button type="submit" variant="primary" size="sm">
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
            toDateUnitHead: '',
            toDateVerifier: '',
            toDateApprover: '',
            fromDateUnitHead: '',
            fromDateVerifier: '',
            fromDateApprover: '',
            unitHead: '',
            verifier: '',
            approver: ''
        };
        return (
            <Modal show 
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.handleCloseModalDialog}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('AddNew')}</Modal.Title>
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