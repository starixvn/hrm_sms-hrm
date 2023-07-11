import React, { Component } from 'react';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import dayjs from 'dayjs';

import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import DatePicker from '@jsroot/components/date-picker/index';
import { validateFormik } from '../common.functions';
import LangHelper from '@jsroot/common/langHelper';
import { ManpowerFlow } from '@jsroot/common/constants';

export default class Update extends Component {
    handleSubmit = values => {
        const DateFormat = 'YYYY-MM-DD';
        const params = {
            periodId: values.periodId,
            name: values.name,
            settings: [
                {
                    processId: ManpowerFlow.UnitHead,
                    fromDate: dayjs(values.fromDateUnitHead).format(DateFormat),
                    todate: dayjs(values.toDateUnitHead).format(DateFormat)
                },
                {
                    processId: ManpowerFlow.Verifier,
                    fromDate: dayjs(values.fromDateVerifier).format(DateFormat),
                    todate: dayjs(values.toDateVerifier).format(DateFormat)
                },
                {
                    processId: ManpowerFlow.Approver,
                    fromDate: dayjs(values.fromDateApprover).format(DateFormat),
                    todate: dayjs(values.toDateApprover).format(DateFormat)
                }
            ]
        };
        this.props.onSubmit(params);
    };

    isValidInput = (touched, errors) => touched && errors ? 'is-invalid' : '';

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        setFieldValue,
        handleSubmit
    }) => {
        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage className="summary-errors" touched={touched} errors={errors} fieldNames={Object.keys(values)} />
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
                    <Form.Group as={Row} className="group-datepicker-label-container">
                        <Form.Label column sm="4" md="3" />
                        <Col sm="8" md="9" className="group-datepicker-label">
                            <Form.Label>{LangHelper.getResource('FromDate')}</Form.Label>
                            <Form.Label>{LangHelper.getResource('ToDate')}</Form.Label>
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('UnitHead')}
                        </Form.Label>
                        <Col sm="8" md="9" className="group-datepicker-input">
                            <DatePicker selected={values.fromDateUnitHead}
                                isInvalid={this.isValidInput(touched.unitHead, errors.unitHead)}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('fromDateUnitHead', value)}
                            />
                            <DatePicker selected={values.toDateUnitHead}
                                isInvalid={this.isValidInput(touched.unitHead, errors.unitHead)}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('toDateUnitHead', value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('Verifier')}
                        </Form.Label>
                        <Col sm="8" md="9" className="group-datepicker-input">
                            <DatePicker selected={values.fromDateVerifier}
                                isInvalid={this.isValidInput(touched.verifier, errors.verifier)}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('fromDateVerifier', value)}
                            />
                            <DatePicker selected={values.toDateVerifier}
                                dateFormat="dd MMM,yyyy"
                                isInvalid={this.isValidInput(touched.verifier, errors.verifier)}
                                onChange={value => setFieldValue('toDateVerifier', value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('Approver')}
                        </Form.Label>
                        <Col sm="8" md="9" className="group-datepicker-input">
                            <DatePicker selected={values.fromDateApprover}
                                isInvalid={this.isValidInput(touched.approver, errors.approver)}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('fromDateApprover', value)}
                            />
                            <DatePicker selected={values.toDateApprover}
                                isInvalid={this.isValidInput(touched.approver, errors.approver)}
                                dateFormat="dd MMM,yyyy"
                                onChange={value => setFieldValue('toDateApprover', value)}
                            />

                        </Col>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button type="submit" variant="primary" size="sm">
                        {LangHelper.getResource('Save')}
                    </Button>
                    <Button variant="secondary" size="sm" onClick={this.props.onClose}>
                        {LangHelper.getResource('Cancel')}
                    </Button>
                </Modal.Footer>
            </Form>
        );
    };

    render() {
        const period = this.props.period;

        const initialValues = {
            periodId: period.periodId,
            name: period.name,
            fromDateUnitHead: '',
            toDateUnitHead: '',
            fromDateVerifier: '',
            toDateVerifier: '',
            fromDateApprover: '',
            toDateApprover: '',
            unitHead: '',
            verifier: '',
            approver: ''
        };
        period.settings.map((item, index) => {
            switch (item.processId) {
                case ManpowerFlow.UnitHead:
                    initialValues.fromDateUnitHead = new Date(item.fromDate);
                    initialValues.toDateUnitHead = new Date(item.toDate);
                    break;

                case ManpowerFlow.Verifier:
                    initialValues.fromDateVerifier = new Date(item.fromDate);
                    initialValues.toDateVerifier = new Date(item.toDate);
                    break;

                case ManpowerFlow.Approver:
                    initialValues.fromDateApprover = new Date(item.fromDate);
                    initialValues.toDateApprover = new Date(item.toDate);
                    break;
            }
        });

        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.props.onClose}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('Update')}</Modal.Title>
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