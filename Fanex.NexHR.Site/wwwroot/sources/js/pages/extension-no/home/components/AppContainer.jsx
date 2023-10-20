import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
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

    useEffect(() => {
        getExtensionNo();
    }, [])
    return (
        <Fragment>
            <ExtensionControl />
            <Table data={data}/>
        </Fragment>
    )
};