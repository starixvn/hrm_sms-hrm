import React, { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';

import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import SetupOtp from './SetupOtp';

const AppContainer = () => {
    const [accountOTP, setAccountOTP] = useState(null);
    const [changeSecretKeyModalDialog, setChangeSecretKeyModalDialog] = useState({ show: false });     

    const handleClosechangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: false });
    }

    const handleOpenchangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: true });
    }
    useEffect(() => {
        const getAccountOTP = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Account/GetAccountOTP');
            await DataService.postDataAsync(url).then(response => {
                setAccountOTP(response.data);
            }).then(() => Site.unblockUI());
        };

        getAccountOTP();
    }, []);

    const getAccountOTP = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/GetAccountOTP');
        await DataService.postDataAsync(url).then(response => {
            setAccountOTP(response.data);
        }).then(() => Site.unblockUI());
    };

    const resetSetupOtp = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/ResetSetupOtp');
        await DataService.postDataAsync(url, accountOTP).then(response => {
            if (response.data) {
                handleClosechangeSecretKeyModalDialog();
                getAccountOTP();
            }
        }).then(() => Site.unblockUI());
    };

    const changeStatusOTP = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/UpdateStatusOTP');
        await DataService.postDataAsync(url, accountOTP).then(response => {
        }).then(() => Site.unblockUI());
    };

    const renderOTP = () => {
        return <div className="otp">
            <div className="enable-otp">
                <p>Enable OTP Security</p>
                <div className='custom-control custom-switch switch-enable-otp'>
                    <input
                        type='checkbox'
                        className='custom-control-input'
                        id='customSwitchesChecked'
                        defaultChecked={accountOTP.enabled}
                        onChange={changeStatusOTP}
                    />
                    <label className='custom-control-label' htmlFor='customSwitchesChecked'></label>
                </div>
            </div>
            <div>
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
        </div>;
    }

    return (
        <>
            {
                accountOTP !== null && accountOTP.isOTPRegistered && renderOTP()
            }
            {
                (accountOTP == null || !accountOTP.isOTPRegistered) &&
                <SetupOtp getAccountOTP={getAccountOTP}/>
            }
            {
                changeSecretKeyModalDialog.show &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>The system will no longer accept the secret key generated on the registered device. Do you want to proceed?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={resetSetupOtp}
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

export default AppContainer;