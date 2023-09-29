import React from 'react';
import QRCode from "react-qr-code";
import { Button, Form, Accordion } from 'react-bootstrap';

import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';

const SetupOtp = ({verifySetupOtp, setupOtp, isShowMessageVeridyOtp, isVeridyOtp}) => {

    const renderSetup2Fa = () => {
        return <div className="setup-2fa">
            <p className="intro">The OTP will be required when you make log in to GO system. You will need to register for the <strong>Mobile Authenticator App</strong> before enabling the OTP.</p>
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
                        <QRCode
                            size={256}
                            className="img"
                            value={setupOtp.qrCodeText} />
                        <p>The new account will be named after your email "<span className="text-email">{setupOtp.email}</span>"</p>
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
                                            Key: <span className="text-key" id="qrcodetext">{setupOtp.textCode}</span>
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
                    <div className="panel-body">
                        <div className="manualVerifyOtpCode">
                            <Form.Row>
                            </Form.Row>
                            <Form.Row>
                                <Form.Label>{LangHelper.getResource('OTP Code')}</Form.Label>
                            </Form.Row>
                            <Form.Row>
                                <Form.Control type="text" size="sm"
                                    name="otp-code" className={`${!isVeridyOtp && isShowMessageVeridyOtp ? 'inValidOtp' : ''} otp-code`} id="otp-code" autoComplete="off"
                                    onChange={event => setupOtp.otp = event.target.value}
                                />
                                <Button variant="primary" size="sm" className="mr-3 btn-veridyOtpCode" type="submit" onClick={verifySetupOtp}>
                                    {LangHelper.getResource('Verify')}
                                </Button>
                            </Form.Row>
                        </div>
                    </div>
                </li>
            </ul>
        </div>;
    }

    return (
        <>
            {
                setupOtp !== null && renderSetup2Fa()
            }
        </>
    );
}

export default SetupOtp;