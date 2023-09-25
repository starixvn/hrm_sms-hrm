import React, { Component } from 'react';
import { Button, Form, Accordion } from 'react-bootstrap';

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
                    window.location = Site.resolveClientUrl('home/index');
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
            <div className="setup-2fa">
                <p className="intro">The OTP will be required when you make log in to HRP system. You will need to register for the <strong>Mobile Authenticator App</strong> before enabling the OTP.</p>
                <ul className="register-otp list-unstyled">
                    <li className="steps">
                        <h2 className="sub-title">Download Sophos Authenticator on your mobile phone</h2>
                        <div className="download-content">
                            <div className="logo-sophos">
                                <img src={Site.resolveClientUrl('css/images/otp/logo.png')} />
                            </div>
                            <div className="list-download">
                                <h3>Sophos Authenticator</h3>
                                <p><strong>Sophos Gmbh</strong></p>
                                <ul className="list-inline">
                                    <li>
                                        <a className="btn-download-ios" href="https://itunes.apple.com/vn/app/sophos-authenticator/id864224575?mt=8" target="_blank">
                                            <img src={Site.resolveClientUrl('css/images/otp/apple.png')} />
                                        </a>
                                    </li>
                                    <li>
                                        <a className="btn-download-android" href="https://play.google.com/store/apps/details?id=com.sophos.sophtoken&hl=en" target="_blank">
                                            <img src={Site.resolveClientUrl('css/images/otp/google.png')} />
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <p className="more-info">Other two-factor authenticator applications such as Google Authenticator, Microsoft Authenticator etc. are also supported.</p>
                        <p className="more-info-important">The Date & Time settings on your device must be set to Automatic.</p>
                    </li>
                    <li className="steps">
                        <h2 className="sub-title">Create an account by scanning this QR code with Sophos</h2>
                        <div className="qr-code">
                            <canvas id="setup-otp-qrcode" className="img"></canvas>
                            <p>The new account will be named after your username "<span className="text-username">xxx</span>"</p>
                        </div>

                        <div className="panel-group manualQRCode" id="manualQRCode" role="tablist" aria-multiselectable="true">
                            <div className="panel">
                                <Accordion>
                                    <div className="panel-heading" role="tab" id="headingQR">
                                        <h4 className="panel-title">
                                            <Accordion.Toggle as={Button} variant="link" eventKey="0">
                                                Unable to scan the QR code?
                                            </Accordion.Toggle>
                                        </h4>
                                    </div>
                                    <Accordion.Collapse eventKey="0">
                                        <div className="panel-body">
                                            <p>Please input the key below to the Manual Add page in Sophos then click Done (iOS) or Add (Android)</p>
                                            <p>
                                                Key: <span className="text-key" id="qrcodetext" data-qrcodetext="@Html.Raw(Model.QrCodeText)">@Model.TextCode</span>
                                            </p>
                                            <div className="example">
                                                <div className="col-left">
                                                    <img src={Site.resolveClientUrl('css/images/otp/manual-ios.png')} title="IOS" />
                                                </div>
                                                <div className="col-right">
                                                    <img src={Site.resolveClientUrl('css/images/otp/manual-android.png')} title="Android" />
                                                </div>
                                            </div>
                                        </div>
                                    </Accordion.Collapse>
                                    <div id="unableScan" className="panel-collapse collapse" role="tabpanel" aria-labelledby="headingQR">

                                    </div>
                                </Accordion>
                            </div>
                        </div>
                    </li>
                    <li className="steps">
                        <h2 className="sub-title">Enter the 6-digits code generated by Sophos into the textbox below and then click Verify to complete the registration.</h2>
                    </li>
                </ul>
            </div>
        );
    }
}