import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl, confirm, alert } from '../../../../shared/common.functions';
import Table from './Table';
import Filter from './Filter';

const AppContainer = () => {
    const [departments, setDepartments] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });

    const handleGetDepartment = params => {
        setFilterParams(params);
        axios.post(resolveClientUrl('Department/GetDepartment'), params)
            .then(response => {
                setDepartments(response.data)
            }).catch(axiosError);
    };

    useEffect(() => {
        handleGetDepartment(filterParams);
    }, []);

    const handleDelete = department => {
        confirm('Are you sure you want to delete this department?', () => {
            axios.post(resolveClientUrl('Department/Delete'), {
                id: department.id
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    handleGetDepartment(filterParams);
                } else {
                    alert(data.message);
                }
            }).catch(axiosError);
        });
    };

    return (
        <Fragment>
            <Filter onSubmit={handleGetDepartment} />
            <Table data={departments} onDelete={handleDelete} />
        </Fragment>
    );
};

export default AppContainer;