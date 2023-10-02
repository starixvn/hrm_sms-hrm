import React, { Component } from 'react';
import { Button, Form, Spinner } from 'react-bootstrap';
import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import RijndaelService from '@jsroot/common/rijndaelService';

export default class AppContainer extends Component {
    constructor(props) {
        super(props);
        this.state = {
            username: '',
            password: '',
            errors: {},
            result: null,
            isSubmiting: false,
            type: 'password'
        };
    }

    handleChange = (fieldname, value) => {
        this.setState({ [fieldname]: value });
    };

    validate = () => {
        const { username, password } = this.state;
        const errors = {};
        if (username.trim().length === 0) {
            errors.username = LangHelper.formatString(LangHelper.getResource('FieldRequired'), LangHelper.getResource('Username'));
        }

        if (password.trim().length === 0) {
            errors.password = LangHelper.formatString(LangHelper.getResource('FieldRequired'), LangHelper.getResource('Password'));
        }

        this.setState({ errors });
        return Object.keys(errors).length === 0;
    };

    handleSubmit = async event => {
        event.preventDefault();
        const isValid = this.validate();
        if (isValid) {
            this.setState({ isSubmiting: true });
            const { username, password } = this.state;
            const token = document.getElementById('token').value;
            const [rijndaelKey, rijndaelIV] = token.split(':');
            const model = {
                username: RijndaelService.encrypt(username, rijndaelKey, rijndaelIV),
                password: RijndaelService.encrypt(password, rijndaelKey, rijndaelIV),
                token: token
            };
            const url = Site.resolveClientUrl('auth/login');
            await DataService.postDataAsync(url, model).then(response => {
                if (response.data.isSuccessful) {
                    window.location = Site.resolveClientUrl('account/verifyotp');
                } else {
                    this.setState({ result: response.data, isSubmiting: false });
                }
            });
        }
    };

    showPassword = () => {
        this.setState(({ type }) => ({
            type: type === 'text' ? 'password' : 'text'
        }));
    };

    render() {
        const { username, password, errors, result } = this.state;
        return (
            <div className="login">
                <div className="login-main">
                    <div className="container">
                        <div className="login-flex">
                            <div className="login-left">
                                <img className="img-res" src={Site.resolveClientUrl('css/images/login/login-gosystem.png')} alt="user login" />
                            </div>
                            <div className="login-form">
                                <Form noValidate onSubmit={this.handleSubmit}>
                                    <div className="login-logo">
                                        <img alt="logo" src={Site.resolveClientUrl('css/images/login/logo-gosystem.svg')} className="img-res" />
                                    </div>
                                    {
                                        result && !result.issuccessful &&
                                        <div className="text-danger">{result.message}</div>
                                    }
                                    <div className="group">
                                        <Form.Control type="text" size="sm" isInvalid={errors && errors.username}
                                            className="form-control" placeholder={LangHelper.getResource('Username')}
                                            value={username} onChange={event => this.handleChange('username', event.target.value)}
                                        />
                                        <i className="fa fa-user" />
                                        <span className="error">{LangHelper.getResource('NoEmpty')}</span>
                                    </div>
                                    <div className="group">
                                        <Form.Control type={this.state.type} size="sm" isInvalid={errors && errors.password}
                                            className="form-control" placeholder={LangHelper.getResource('Password')}
                                            value={password} onChange={event => this.handleChange('password', event.target.value)}
                                        />
                                        <i className="fa fa-lock " />
                                        <span className={`fa ${this.state.type === 'password' ? 'fa-eye' : 'fa-eye-slash'}`} onClick={this.showPassword} />
                                        <span className="error">{LangHelper.getResource('NoEmpty')}</span>
                                    </div>
                                    <div className="group group-login">
                                        <Button className='btn-login' type="submit" variant="primary" size="sm" style={{ width: '100%' }}
                                            onClick={this.handleSubmit}
                                            disabled={this.state.isSubmiting}
                                        >
                                            {LangHelper.getResource('Login')}
                                            {
                                                this.state.isSubmiting &&
                                                <Spinner className="ml-3"
                                                    as="span"
                                                    animation="border"
                                                    role="status"
                                                    aria-hidden="true"
                                                />
                                            }
                                        </Button>
                                    </div>
                                </Form>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}