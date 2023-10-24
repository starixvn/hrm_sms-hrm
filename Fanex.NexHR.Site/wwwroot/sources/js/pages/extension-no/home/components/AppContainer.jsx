import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../../shared/common.functions';
import { ExtensionControl } from './ExtensionControl';
import { Table } from './Table';

export const AppContainer = () => {
    const [data, setData] = useState([]);

    const getExtensionNo = () => {
        axios(resolveClientUrl('ExtensionNo/GetExtensionNo')).then(response => {
            const { data } = response;
            if (data) {
                setData(response.data)  
            }
        }).catch(axiosError);
    }

    const handleDeleteExtention = (id) => {
        confirm('Are you sure you want to delete this extension number?', () => {
            axios.post(resolveClientUrl(`ExtensionNo/Delete/${id}`)).then(response => {
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
            <Table data={data} handleDeleteRow={ handleDeleteExtention }/>
        </Fragment>
    )
};