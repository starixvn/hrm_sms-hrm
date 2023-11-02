import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';

import Filter from './Filter';
import Table from './Table';

import { axiosError, resolveClientUrl, confirm, alert } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [units, setUnits] = useState([]);

    const getUnits = params => {
        setFilterParams(params);

        axios.post(resolveClientUrl('Unit/GetUnits'), params)
            .then(response => {
                setUnits(response.data)
            }).catch(axiosError);
    }

    useEffect(() => {
        getUnits(filterParams);
    }, []);

    const handleDelete = unit => {
        confirm('Are you sure you want to delete this unit?', () => {
            axios.post(resolveClientUrl('Unit/Delete'), {
                unitId: unit.id
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    getUnits(filterParams);
                } else {
                    alert(data.message);
                }
            }).catch(axiosError);
        });
    };

    return (
        <Fragment>
            <Filter onSubmit={getUnits} />
            <Table data={units} onDelete={handleDelete} />
        </Fragment>
    );
};

export default AppContainer;