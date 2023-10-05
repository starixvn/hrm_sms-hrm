import React, { useState } from 'react';
import { Button, Form } from 'react-bootstrap';

import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

const AppContainer = () => {
    const [otp, setOtp] = useState(null);
    const [isVerifyOtp, setIsVerifyOtp] = useState(false);
    const [isShowMessageVerifyOtp, setIsShowMessageVerifyOtp] = useState(false);
    const [messageError, setMessageError] = useState(null);

    const verifyOtp = async () => {
        if (otp !== null && otp !== '') {
            const url = Site.resolveClientUrl('Account/VerifyOTP');
            await DataService.postDataAsync(url, { otp: otp }).then(response => {
                setIsVerifyOtp(response.data);
                setIsShowMessageVerifyOtp(true);
                setMessageError(null);
                if (response.data) {
                    window.location = Site.resolveClientUrl('Home/Index');
                }
            });
        } else {
            setIsVerifyOtp(false);
            setIsShowMessageVerifyOtp(false);
            setMessageError('The OTP code field is required.');
        }
    };

    const reLogin = () => window.location = Site.resolveClientUrl('Auth/Login');

    return (
        <div>
            <div className="header">
                <a className="logo" href={Site.resolveClientUrl('home/index')}>
                    <img src={Site.resolveClientUrl('css/images/logo.svg')} alt="GO System" />
                </a>
            </div>
            <div className="block -otp-code-validation">
                <div className="circle-large">
                    <span className="icon-otp fa fa-shield"></span>
                </div>
                <h1 className="title">{LangHelper.getResource('OTPCodeValidation')}</h1>
                <div className="intro -otp">
                    <p>{LangHelper.getResource('IntroVerifyOtp')}</p>
                    <p className="text-special">
                        <span>{LangHelper.getResource('IntroSpecialVerifyOtp')}</span>
                    </p>
                </div>
                <div className="group">
                    {
                        !isVerifyOtp && isShowMessageVerifyOtp &&
                        <Form.Label className="msg-error">{LangHelper.getResource('MessageErrorVerifyOtp')}</Form.Label>
                    }
                    <Form.Control type="text" size="sm"
                        name="otp-code" className={`${!isVerifyOtp && isShowMessageVerifyOtp ? 'inValidOtp' : ''} form-control encrypt`} autoComplete="off"
                        maxLength="6" onChange={event => setOtp(event.target.value)}
                    />
                    {
                        messageError != null &&
                        <Form.Label className="msg-error">{messageError}</Form.Label>
                    }
                </div>
                <div className="group list-button">
                    <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={reLogin}>
                        {LangHelper.getResource('Cancel')}
                    </Button>
                    <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={verifyOtp}>
                        {LangHelper.getResource('ValidateOTP')}
                    </Button>
                </div>
            </div>
        </div>
    );
}

export default AppContainer;