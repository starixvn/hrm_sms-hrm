import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../../shared/common.functions';
import { Filter } from './Filter';
import { Table } from './Table';
export const AppContainer = () => {
    const [choose, setChoose] = useState('');
    const [section, setSection] = useState([]);

    const hanleGetSection = () => {
        axios.post(resolveClientUrl('Section/GetSection'), {
            isActive: choose
        }).then(response => {
            setSection(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        hanleGetSection();
    }, [])

    const handleFilterForm = (e) => {
        e.preventDefault();
        hanleGetSection();
    }
    return (
        <Fragment>
            <Filter setChoose={setChoose} choose={choose} submit={handleFilterForm} />
            <Table data={section} hanleGetSection={hanleGetSection}/>
        </Fragment>
    )
};