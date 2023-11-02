import React, { Fragment, useEffect , useState } from 'react';
import { Filter } from './Filter';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Table } from './Table';

export const AppContainer = () => {
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

    return (
        <Fragment>
            <Filter onSubmit={getUnits} />
            <Table data={units} />
        </Fragment>
    );
};