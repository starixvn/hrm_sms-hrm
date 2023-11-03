import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import ExtensionControl from './ExtensionControl';
import Table from './Table';

import { resolveClientUrl, axiosError, confirm, toastOptions } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [extensions, setExtensions] = useState([]);

    const getExtensionNo = () => {
        axios(resolveClientUrl('ExtensionNo/GetExtensionNo')).then(response => {
            setExtensions(response.data)
        }).catch(axiosError);
    };

    const handleDeleteExtention = empId => {
        confirm('Are you sure you want to delete this extension number?', () => {
            axios.post(resolveClientUrl(`ExtensionNo/Delete/${empId}`)).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success('Deleted successfully!', '', toastOptions());
                    getExtensionNo();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        });
    };

    useEffect(() => {
        getExtensionNo();
    }, []);

    return (
        <Fragment>
            <ExtensionControl />
            <Table data={extensions} onDelete={handleDeleteExtention} />
        </Fragment>
    );
};

export default AppContainer;