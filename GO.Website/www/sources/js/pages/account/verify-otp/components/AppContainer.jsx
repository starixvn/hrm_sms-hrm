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
    const [otp, setOtp] = useState(null);
    const [isVeridyOtp, setIsVeridyOtp] = useState(false);
    const [isShowMessageVeridyOtp, setIsShowMessageVeridyOtp] = useState(false);

    const verifyOtp = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Account/VerifyOTP');
        await DataService.postDataAsync(url, otp).then(response => {

        }).then(() => Site.unblockUI());
    };

    return (
        <>

        </>
    );
}

export default AppContainer;