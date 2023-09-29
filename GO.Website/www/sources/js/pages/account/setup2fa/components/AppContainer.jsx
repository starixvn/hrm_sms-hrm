import React, { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';

import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import SetupOtp from './SetupOtp';
import Otp from './Otp';

const AppContainer = () => {
    const [accountOTP, setAccountOTP] = useState(null);
    const [changeSecretKeyModalDialog, setChangeSecretKeyModalDialog] = useState({ show: false });     
    const [setupOtp, setSetupOtp] = useState(null);
    const [isShowMessageVeridyOtp, setIsShowMessageVeridyOtp] = useState(false);
    const [isVeridyOtp, setIsVeridyOtp] = useState(false);

    const handleClosechangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: false });
    }

    const handleOpenchangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: true });
    }
    useEffect(() => {
        const getSetupOtp = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Account/SetupOtp');
            await DataService.postDataAsync(url).then(response => {
                setSetupOtp(response.data);
            }).then(() => Site.unblockUI());
        };

        const getAccountOTP = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Account/GetAccountOTP');
            await DataService.postDataAsync(url).then(response => {
                setAccountOTP(response.data);
                if (response.data == null || !response.data.isOTPRegistered) {
                    getSetupOtp();
                }
            }).then(() => Site.unblockUI());
        };

        getAccountOTP();
    }, []);

    const getSetupOtp = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/SetupOtp');
        await DataService.postDataAsync(url).then(response => {
            setSetupOtp(response.data);
        }).then(() => Site.unblockUI());
    };

    const verifySetupOtp = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/VerifySetupOTP');
        await DataService.postDataAsync(url, setupOtp).then(response => {
            setIsVeridyOtp(response.data);
            setIsShowMessageVeridyOtp(true);
            if (response.data) {
                toast.success('Your Authenticator App is verified.');
                setTimeout(() => {
                    getAccountOTP();
                }, 3000);
            } else {
                toast.error('Your OTP is invalid or expired.');
            }
        }).then(() => Site.unblockUI());
    };

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
                getSetupOtp();
            }
        }).then(() => Site.unblockUI());
    };

    const changeStatusOTP = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/UpdateStatusOTP');
        await DataService.postDataAsync(url, accountOTP).then(response => {
        }).then(() => Site.unblockUI());
    };

    return (
        <>
            {
                accountOTP !== null && accountOTP.isOTPRegistered &&
                <Otp
                    accountOTP={accountOTP}
                    changeStatusOTP={changeStatusOTP}
                    handleOpenchangeSecretKeyModalDialog={handleOpenchangeSecretKeyModalDialog}
                />
            }
            {
                (accountOTP == null || !accountOTP.isOTPRegistered) &&
                <SetupOtp
                    verifySetupOtp={verifySetupOtp}
                    setupOtp={setupOtp}
                    isShowMessageVeridyOtp={isShowMessageVeridyOtp}
                    isVeridyOtp={isVeridyOtp}
                   />
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