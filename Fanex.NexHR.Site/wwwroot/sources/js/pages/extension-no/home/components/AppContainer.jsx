import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm } from '../../../../shared/common.functions';
import ExtensionControl from './ExtensionControl';
import { Table } from './Table';

const AppContainer = () => {
    const [extension, setExtension] = useState([]);

    const getExtensionNo = () => {
        axios(resolveClientUrl('ExtensionNo/GetExtensionNo')).then(response => {
            setExtension(response.data)
        }).catch(axiosError);
    }

    const handleDeleteExtention = empId => {
        confirm('Are you sure you want to delete this extension number?', () => {
            axios.post(resolveClientUrl(`ExtensionNo/Delete/${empId}`)).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    getExtensionNo();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        });
        
    }
    useEffect(() => {
        getExtensionNo();
    }, [])
    return (
        <Fragment>
            <ExtensionControl />
            <Table data={extension} onDelete={handleDeleteExtention}/>
        </Fragment>
    )
};


export default AppContainer;