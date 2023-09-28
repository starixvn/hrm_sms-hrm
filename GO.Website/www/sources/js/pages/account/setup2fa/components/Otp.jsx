import React, { useEffect, useState } from 'react';
import QRCode from "react-qr-code";
import { Button, Form, Accordion } from 'react-bootstrap';


import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

const Otp = () => {
    const [changeSecretKeyModalDialog, setChangeSecretKeyModalDialog] = useState({ show: false });

    const handleClosechangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: false });
    }

    const handleOpenchangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: true });
    }

    return (
        <>
            <div className="otp">
                <p className="info">
                    The OTP will be required when you make log in to Go system. 
                </p>
                <p className="info">
                    You will need to register for the <strong>Mobile Authenticator App</strong> before enabling the OTP option.
                </p>
                <p className="info bold otp-info">Register Sophos Authenticator Mobile App <span className="fa fa-check-circle"></span></p>
                <p className="info">Click the button below to change the registered device from you secret key.</p>
                <Button variant="primary" size="sm" className="mr-3 btn-change-secret-key" type="submit" onClick={handleOpenchangeSecretKeyModalDialog}>
                    {LangHelper.getResource('ChangeSecretKey')}
                </Button>
            </div>
            {
                changeSecretKeyModalDialog.show &&
                <SweetModalDialog
                    title={LangHelper.getResource('ConfirmationChangeSecretKey')}
                    contents={<div>The system will no longer accept the secret key generated on the registered device. Do you want to proceed?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={handleClosechangeSecretKeyModalDialog}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={handleClosechangeSecretKeyModalDialog}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={handleClosechangeSecretKeyModalDialog}
                />
            }
        </>
    );
}

export default Otp;