import React, { Fragment, useEffect, useState } from 'react';
import { Filter } from './Filter';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../shared/common.functions';
import { Table } from './Table';

export const AppContainer = () => {
    const [roleId, setRoleId] = useState('');
    const [data, setData] = useState([]);

    const handleGetFunctions = () => {
        axios.post(resolveClientUrl('Function/GetFunctions'), {
            roleId: roleId
        }).then(response => {
            setData(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetFunctions();
    }, [roleId]);

    return (
        <Fragment>
            <Filter setRoleId={setRoleId} roleId={roleId} />
            <Table data={data} />
        </Fragment>
    )
};