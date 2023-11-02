import React, { Fragment, useEffect , useState } from 'react';
import axios from 'axios';

import Filter from './Filter';
import Table from './Table';

import { axiosError, resolveClientUrl, confirm, alert } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [units, setUnits] = useState([]);

    const getUnits = (status) => {
        axios.post(resolveClientUrl('Unit/GetUnits'), {
            isActive: status
        }).then(response => {
            setUnits(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        getUnits();
    }, [])

    const handleDelete = unit => {
        confirm('Are you sure you want to delete this unit?', () => {
            axios.post(resolveClientUrl('Unit/Delete'), {
                unitId: unit.id
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    alert('Delete successfully!');
                    getUnits();
                } else {
                    alert(data.Message);
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