import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import RijndaelService from '@jsroot/common/rijndaelService';
import LangHelper from '@jsroot/common/langHelper';

export class ResetPassword extends Component {
    constructor(props) {
        super(props);
    }

    handleCloseModalDialog = () => {
        this.props.setResetPasswordModalDialog({ show: false });
        if (this.props.resetPasswordResult) {
            this.props.setResetPasswordResult(null);
        }
    };

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const token = document.getElementById('token').value;
        const rijndaelKey = token.split(':')[0];
        const rijndaelIV = token.split(':')[1];

        const params = {
            userId: values.userId,
            password: RijndaelService.encrypt(values.newPassword, rijndaelKey, rijndaelIV),
            token
        };
        this.props.resetPassword(params).then(() => {
            if (this.props.resetPasswordResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                }, 3000);
            } else {
                setSubmitting(false);
            }
        });
    };

    validateFormik = values => {
        const errors = {};

        if (!values.newPassword || values.newPassword.trim().length === 0) {
            errors.newPassword = LangHelper.formatString(
                LangHelper.getResource('FieldRequired'),
                LangHelper.getResource('NewPassword'));
        } else {
            if (values.newPassword.trim().length > 50) {
                errors.newPassword = LangHelper.formatString(
                    LangHelper.getResource('FieldMaxLength'),
                    LangHelper.getResource('NewPassword'),
                    50);
            }
        }

        if (!values.confirmPassword || values.confirmPassword.trim().length === 0) {
            errors.confirmPassword = LangHelper.formatString(
                LangHelper.getResource('FieldRequired'),
                LangHelper.getResource('ConfirmPassword'));
        } else {
            if (values.confirmPassword.trim().length > 50) {
                errors.confirmPassword = LangHelper.formatString(
                    LangHelper.getResource('FieldMaxLength'),
                    LangHelper.getResource('ConfirmPassword'),
                    50);
            } else {
                if (values.newPassword !== values.confirmPassword) {
                    errors.confirmPassword = "Password don't match";
                }
            }
        }

        return errors;
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
                        this.props.resetPasswordResult &&
                        <ResponseMessage isSuccessful={this.props.resetPasswordResult.isSuccessful}
                            message={this.props.resetPasswordResult.message || LangHelper.getResource('ResetPasswordSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3">{LangHelper.getResource('Username')}</Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" className="shadow-none" value={values.username} disabled />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3">{LangHelper.getResource('Code')}</Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" className="shadow-none" value={values.code} disabled />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3">{LangHelper.getResource('Nickname')}</Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" className="shadow-none" value={values.nickname} disabled />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3">{LangHelper.getResource('FullName')}</Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" className="shadow-none" value={values.fullName} disabled />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="newPassword">
                            {LangHelper.getResource('NewPassword')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="password" size="sm" value={values.newPassword}
                                name="newPassword" id="newPassword" className="shadow-none" autoComplete="off"
                                isInvalid={touched.newPassword && errors.newPassword}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="confirmPassword">
                            {LangHelper.getResource('ConfirmPassword')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="password" size="sm" value={values.confirmPassword}
                                name="confirmPassword" id="confirmPassword" className="shadow-none" autoComplete="off"
                                isInvalid={touched.confirmPassword && errors.confirmPassword}
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
        const { user } = this.props.resetPasswordModalDialog;
        const initialValues = {
            userId: user.userId,
            username: user.username,
            code: user.code,
            nickname: user.nickname,
            fullName: user.fullName,
            newPassword: '',
            confirmPassword: ''
        };
        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.handleCloseModalDialog}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('ResetPassword')}</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={initialValues}
                    validate={this.validateFormik}
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
    const { resetPasswordModalDialog, resetPasswordResult } = state;
    return { resetPasswordModalDialog, resetPasswordResult };
};

export default connect(mapStateToProps, mapDispatchToProps)(ResetPassword);