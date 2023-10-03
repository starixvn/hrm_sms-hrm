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
    const [messageError, setMessageError] = useState(null);

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
        if (setupOtp.otp !== null && setupOtp.otp !== '') {
            Site.blockUI();
            const url = Site.resolveClientUrl('Account/VerifySetupOTP');
            await DataService.postDataAsync(url, setupOtp).then(response => {
                setIsVeridyOtp(response.data);
                setIsShowMessageVeridyOtp(true);
                setMessageError(null);
                if (response.data) {
                    toast.success(LangHelper.getResource('MessageSuccessVerifySetupOtp'));
                    setTimeout(() => {
                        getAccountOTP();
                    }, 3000);
                } else {
                    toast.error(LangHelper.getResource('MessageErrorVerifySetupOtp'));
                }
            }).then(() => Site.unblockUI());
        }
        else {
            setIsVeridyOtp(false);
            setIsShowMessageVeridyOtp(false);
            setMessageError('The OTP code field is required.');
        }
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
                    messageError={messageError}
                   />
            }
            {
                changeSecretKeyModalDialog.show &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>{LangHelper.getResource('ContentReSetupOtp')}</div>}
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