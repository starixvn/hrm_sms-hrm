import React, { Fragment, useEffect, useState } from 'react';
import { Filter } from './Filter';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../shared/common.functions';
import { Table } from './Table';

export const AppContainer = () => {
    const [role, setRole] = useState('');
    const [functions, setFunctions] = useState([]);

    const handleGetFunctions = () => {
        axios.post(resolveClientUrl('Function/GetFunctions'), {
            roleId: role
        }).then(response => {
            setFunctions(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetFunctions();
    }, [])

    const handleFilterForm = (e) => {
        console.log(window.hasPermission);
        e.preventDefault();
        handleGetFunctions();
    }
    return (
        <Fragment>
            <Filter setRole={setRole} role={role} submit={handleFilterForm} />
            <Table data={functions} />
            {
                functions.isHost &&
                <div className="form-group">
                    <button type="submit" className="btn btn-primary" id="btnSubmit">Save</button>
                </div>
            }
        </Fragment>
    )
};