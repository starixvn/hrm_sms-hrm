import React, { Fragment, useEffect, useState } from 'react';
import { Filter } from './Filter';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Table } from './Table';

export const AppContainer = () => {
    const [choose, setChoose] = useState('');
    const [departments, setDepartments] = useState([]);
    const handleGetDepartment = () => {
        axios.post(resolveClientUrl('Department/GetDepartment'), {
            isActive: choose
        }).then(response => {
            setDepartments(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetDepartment();
    }, [])

    const handleFilterForm = (e) => {
        e.preventDefault();
        handleGetDepartment();
    }
    return (
        <Fragment>
            <Filter setChoose={setChoose} choose={choose} submit={handleFilterForm} />
            <Table data={departments} handleGetDepartment={handleGetDepartment }/>
        </Fragment>
    )
};