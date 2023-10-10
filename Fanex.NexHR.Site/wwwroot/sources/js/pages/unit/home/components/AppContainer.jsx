import React, { Fragment, useEffect, useState } from 'react';
import { Filter } from './Filter';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Table } from './Table';

export const AppContainer = () => {
    const [choose, setChoose] = useState('');
    const [units, setUnits] = useState([]);
    const hanleGetUnit = () => {
        axios.post(resolveClientUrl('Unit/GetUnit'), {
            isActive: choose
        }).then(response => {
            setUnits(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        hanleGetUnit();
    }, [])

    const handleFilterForm = (e) => {
        e.preventDefault();
        hanleGetUnit();
    }
    return (
        <Fragment>
            <Filter setChoose={setChoose} choose={choose} submit={handleFilterForm} />
            <Table data={units} hanleGetUnit={ hanleGetUnit }/>
        </Fragment>
    )
};