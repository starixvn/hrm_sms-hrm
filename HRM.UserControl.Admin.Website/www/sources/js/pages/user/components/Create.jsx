import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import LangHelper from '@jsroot/common/langHelper';
import RijndaelService from '@jsroot/common/rijndaelService';
import { validateFormik } from '../common.functions';

export class CreateUser extends Component {
    constructor(props) {
        super(props);
    }

    handleCloseModalDialog = () => {
        this.props.setCreateModalDialog({ show: false });
        if (this.props.createUserResult) {
            this.props.setCreateUserResult(null);
        }
    };

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const token = document.getElementById('token').value;
        const rijndaelKey = token.split(':')[0];
        const rijndaelIV = token.split(':')[1];

        const params = {
            username: RijndaelService.encrypt(values.username, rijndaelKey, rijndaelIV),
            password: RijndaelService.encrypt(values.password, rijndaelKey, rijndaelIV),
            nickname: values.nickname,
            fullName: values.fullName,
            email: values.email,
            token
        };
        this.props.createUser(params).then(() => {
            if (this.props.createUserResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.getAllUser(this.props.getAllUserParams);
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
                        this.props.createUserResult &&
                        <ResponseMessage isSuccessful={this.props.createUserResult.isSuccessful}
                            message={this.props.createUserResult.message || LangHelper.getResource('CreateUserSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="username">
                            {LangHelper.getResource('Username')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.username}
                                name="username" id="username"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.username && errors.username}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="password">
                            {LangHelper.getResource('Password')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="password" size="sm" value={values.password}
                                name="password" id="password" className="shadow-none" autoComplete="off"
                                isInvalid={touched.password && errors.password}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="nickname">
                            {LangHelper.getResource('Nickname')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.nickname}
                                name="nickname" id="nickname" className="shadow-none" autoComplete="off"
                                isInvalid={touched.nickname && errors.nickname}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="fullName">
                            {LangHelper.getResource('FullName')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.nickname}
                                name="fullName" id="fullName" className="shadow-none" autoComplete="off"
                                isInvalid={touched.fullName && errors.fullName}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="email">
                            {LangHelper.getResource('Email')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="email" size="sm" value={values.email}
                                name="email" id="email" className="shadow-none" autoComplete="off"
                                isInvalid={touched.email && errors.email}
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
            username: '',
            password: '',
            nickname: '',
            fullName: '',
            email: ''
        };
        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.handleCloseModalDialog}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('CreateUser')}</Modal.Title>
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

export default connect(mapStateToProps, mapDispatchToProps)(CreateUser);