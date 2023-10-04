import React, { Component } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../shared/common.functions';
export default class Login extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fields: {},
            errors: {},
            errMsg : "",
            isShowErr : false
        };
        
    }

    handleValidation() {
        let fields = this.state.fields;
        let errors = {};
        let formIsValid = true;
        if (!fields["username"]) {
            formIsValid = false;
            errors["username"] = "The Username field is required.";
        }

        if (!fields["password"]) {
            formIsValid = false;
            errors["password"] = "The Password field is required.";
        }

        this.setState({ errors: errors });
        return formIsValid;
    }


    HandleLogin = (e) => {
        e.preventDefault();

        if (this.handleValidation()) {
            axios.post(resolveClientUrl("Auth/Login"), {
                Username: this.state.fields['username'],
                Password: btoa(this.state.fields['password'])
            }).then(response => {
                const { data } = response;
                if (data.isSuccessfull) {
                    this.setState({
                        errMsg: "",
                        isShowErr: false
                    })
                    window.location.href = ("/");
                } else {
                    this.setState({
                        errMsg: data.errorMessage,
                        isShowErr: true
                    })
                }
            }).catch(axiosError);
        } else {
            this.setState({
                isShowErr: true
            })
        }
    }

    handleChange(field, e) {
        let fields = this.state.fields;
        fields[field] = e.target.value;
        this.setState({ fields });
    }
   
    render() {
        return (
            <form onSubmit={this.HandleLogin} id="formContainer">
                {
                    this.state.isShowErr && <div className="validation-summary-errors">
                        <ul>
                            <li>{this.state.errors["username"]}</li>
                            <li>{this.state.errors["password"]}</li>
                            <li>{this.state.errMsg}</li>
                        </ul>
                    </div>
                }
                <div className="textbox-outer">
                    <div className="username-icon">
                    </div>
                    <div className="username-field">
                        <input type="text" placeholder="Username" onChange={this.handleChange.bind(this, "username")} />
                    </div>
                </div>
                <div className="textbox-outer">
                    <div className="password-icon">
                    </div>
                    <div className="password-field">
                        <input type="password" placeholder="Password" onChange={this.handleChange.bind(this, "password")} />
                    </div>
                </div>
                <div className="signin-button">
                    <input type="submit" name="submit" className="button" value="LOGIN" />
                </div>
            </form>
        );
    }
}