import React, { Fragment, useEffect, useState } from 'react';
import { Filter } from './Filter';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../shared/common.functions';
import { Table } from './Table';

export const AppContainer = () => {
    const [choose, setChoose] = useState('');
    const [functions, setFunctions] = useState([]);
    const handleGetFunctions = () => {
        axios.post(resolveClientUrl('Function/GetFunctions'), {
            roleId: null
        }).then(response => {
            setFunctions(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetFunctions();
    }, [])

    const handleFilterForm = (e) => {
        e.preventDefault();
        //handleGetSystemRoles();
    }
    return (
        <Fragment>
            <Filter setChoose={setChoose} choose={choose} submit={handleFilterForm} />
            <Table data={functions} />
            <div className="form-group">
                <button type="submit" className="btn btn-primary" id="btnSubmit">Save</button>
            </div>
        </Fragment>
    )
};