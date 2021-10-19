import React, { Component } from 'react';
import { Button, Form, Spinner } from 'react-bootstrap';
import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import RijndaelService from '@jsroot/common/rijndaelService';

class AppContainer extends Component {
    constructor(props) {
        super(props);
        this.state = {
            username: '',
            password: '',
            errors: {},
            result: null,
            isSubmiting: false
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
            const rijndaelKey = token.split(':')[0];
            const rijndaelIV = token.split(':')[1];
            const model = {
                username: RijndaelService.encrypt(username, rijndaelKey, rijndaelIV),
                password: RijndaelService.encrypt(password, rijndaelKey, rijndaelIV),
                token: token
            };
            const url = Site.resolveClientUrl('auth/login');
            await DataService.postDataAsync(url, model).then(response => {
                if (response.data.isSuccessful) {
                    window.location = Site.resolveClientUrl('home/index');
                } else {
                    this.setState({ result: response.data, isSubmiting: false });
                }
            });
        }
    };

    render() {
        const { username, password, errors, result } = this.state;
        return (
            <Form noValidate onSubmit={this.handleSubmit}>
                <div className="logo">
                    <img alt="logo" src={Site.resolveClientUrl('css/images/logo.svg')} alt="User Control" />
                </div>
                <img className="image-visual" src={Site.resolveClientUrl('css/images/login/keyvisual.png')} alt="user login" />
                {
                    result && !result.isSuccessful &&
                    <div className="group group--error"><div className="text-danger text-center">{result.message}</div></div>
                }
                <div className="group">
                    <span className="fa fa-user" />
                    <Form.Control type="text" size="sm" isInvalid={errors && errors.username}
                        className="form-control" placeholder={LangHelper.getResource('Username')}
                        value={username} onChange={event => this.handleChange('username', event.target.value)}
                    />
                </div>
                <div className="group">
                    <i className="fa fa-lock" />
                    <Form.Control type="password" size="sm" isInvalid={errors && errors.password}
                        className="form-control" placeholder={LangHelper.getResource('Password')}
                        value={password} onChange={event => this.handleChange('password', event.target.value)}
                    />
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
        );
    }
}

export default AppContainer;