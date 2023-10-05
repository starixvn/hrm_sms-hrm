import React from 'react';
import { Button, Accordion } from 'react-bootstrap';

import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';

const Otp = ({ accountOTP, onChangeStatusOTP, onChangeSecretKey }) => {
    return (
        <div className="otp">
            <div className="enable-otp">
                <p>{LangHelper.getResource('EnableOTP')}</p>
                <div className='custom-control custom-switch switch-enable-otp'>
                    <input
                        type='checkbox'
                        className='custom-control-input'
                        id='customSwitchesChecked'
                        defaultChecked={accountOTP.enabled}
                        onChange={onChangeStatusOTP}
                    />
                    <label className='custom-control-label' htmlFor='customSwitchesChecked'></label>
                </div>
            </div>
            <div>
                <p className="info info-header">
                    The OTP will be required when you make log in to Go system.
                </p>
                <p className="info">
                    You will need to register for the <strong>Mobile Authenticator App</strong> before enabling the OTP option.
                </p>
                <p className="info bold otp-info">Register Sophos Authenticator Mobile App <span className="fa fa-check-circle"></span></p>
                <p className="info">Click the button below to change the registered device from you secret key.</p>
                <div className="panel-group manualQRCode" id="manualQRCode" role="tablist" aria-multiselectable="true">
                    <div className="panel">
                        <Accordion>
                            <div className="panel-heading" role="tab" id="headingQR">
                                <h4 className="panel-title">
                                    <Accordion.Toggle as={Button} variant="link" eventKey="0">
                                        Recovery code
                                    </Accordion.Toggle>
                                </h4>
                            </div>
                            <Accordion.Collapse eventKey="0">
                                <div className="panel-body">
                                    <p>Please input the key below to the Manual Add page in Sophos then click Done (iOS) or Add (Android)</p>
                                    <p>
                                        Key: <span className="text-key" id="qrcodetext">{accountOTP.recoveryCode}</span>
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
                <Button variant="primary" size="sm" className="mr-3 btn-change-secret-key" type="submit" onClick={onChangeSecretKey}>
                    {LangHelper.getResource('ChangeSecretKey')}
                </Button>
            </div>
        </div>
    );
}

export default Otp;