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
    const [isShowMessageVerifyOtp, setIsShowMessageVerifyOtp] = useState(false);
    const [isVerifyOtp, setIsVerifyOtp] = useState(false);
    const [messageError, setMessageError] = useState(null);

    const handleCloseChangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: false });
    };

    const handleOpenChangeSecretKeyModalDialog = () => {
        setChangeSecretKeyModalDialog({ show: true });
    };


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
                setIsVerifyOtp(response.data);
                setIsShowMessageVerifyOtp(true);
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
        } else {
            setIsVerifyOtp(false);
            setIsShowMessageVerifyOtp(false);
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
                handleCloseChangeSecretKeyModalDialog();
                getAccountOTP();
                getSetupOtp();
            }
        }).then(() => Site.unblockUI());
    };

    const changeStatusOTP = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/UpdateStatusOTP');
        await DataService.postDataAsync(url, accountOTP).then(response => {
            if (response !== undefined) {
                accountOTP.enabled = !accountOTP.enabled;
            }
        }).then(() => Site.unblockUI());
    };

    useEffect(() => {
        const getAccountOTP = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Account/GetAccountOTP');
            await DataService.postDataAsync(url).then(response => {
                if (response != undefined) {
                    setAccountOTP(response.data);
                    if (response.data == null || !response.data.isRegistered) {
                        getSetupOtp();
                    }
                }
            }).then(() => Site.unblockUI());
        };

        getAccountOTP();
    }, []);

    return (
        <>
            {
                accountOTP !== null && accountOTP.isRegistered &&
                <Otp
                    accountOTP={accountOTP}
                    onChangeStatusOTP={changeStatusOTP}
                    onChangeSecretKey={handleOpenChangeSecretKeyModalDialog}
                />
            }
            {
                (accountOTP == null || !accountOTP.isRegistered) &&
                <SetupOtp
                    verifySetupOtp={verifySetupOtp}
                    setupOtp={setupOtp}
                    isShowMessageVerifyOtp={isShowMessageVerifyOtp}
                    isVerifyOtp={isVerifyOtp}
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
                            onClick={handleCloseChangeSecretKeyModalDialog}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={handleCloseChangeSecretKeyModalDialog}
                />
            }
        </>
    );
}

export default AppContainer;