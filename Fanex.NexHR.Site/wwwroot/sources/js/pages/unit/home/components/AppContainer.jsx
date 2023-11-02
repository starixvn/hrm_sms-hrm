import React, { Fragment, useEffect, useState } from 'react';
import { Filter } from './Filter';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Table } from './Table';

export const AppContainer = () => {
    const [choose, setChoose] = useState('');
    const [units, setUnits] = useState([]);

    const getUnits = () => {
        axios.post(resolveClientUrl('Unit/GetUnits'), {
            isActive: choose
        }).then(response => {
            setUnits(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        getUnits();
    }, [])

    const handleFilterForm = (event) => {
        event.preventDefault();
        getUnits();
    };

    return (
        <Fragment>
            <Filter setChoose={setChoose} choose={choose} submit={handleFilterForm} />
            <Table data={units} />
        </Fragment>
    );
};