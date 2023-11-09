import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import ExtensionControl from './ExtensionControl';
import Table from './Table';

import { resolveClientUrl, axiosError, toastOptions } from '../../../../shared/common.functions';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

const AppContainer = () => {
    const [extensions, setExtensions] = useState([]);
    const [dialog, setDialog] = useState({
        employeeId: null,
        showConfirm: false
    });


    const getExtensionNo = () => {
        axios(resolveClientUrl('ExtensionNo/GetExtensionNo')).then(response => {
            setExtensions(response.data)
        }).catch(axiosError);
    };

    const handleDeleteExtention = empId => {
        setDialog({ showConfirm: true, employeeId: empId });
    };
    const handleCancel = () => {
        setDialog({ showConfirm: false });
    };

    const handleConfirm = () => {
        setDialog({ showConfirm: false });
        const empId = dialog.employeeId;
        axios.post(resolveClientUrl(`ExtensionNo/Delete/${empId}`)).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getExtensionNo();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    useEffect(() => {
        getExtensionNo();
    }, []);

    return (
        <Fragment>
            <ExtensionControl />
            <Table data={extensions} onDelete={handleDeleteExtention} />
            <SweetModalDialog
                show={dialog.showConfirm}
                contents={<p>Are you sure you want to delete this Extension?</p>}
                onSubmit={handleConfirm}
                onHide={handleCancel}
            />
        </Fragment>
    );
};

export default AppContainer;