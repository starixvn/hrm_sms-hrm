import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { axiosError, resolveClientUrl, confirm, alert } from '../../../../shared/common.functions';
import Filter from './Filter';
import Table from './Table';
const AppContainer = () => {
    const [section, setSection] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });

    const getSection = params => {
        setFilterParams(params);
        axios.post(resolveClientUrl('Section/GetSection'), params).then(response => {
            setSection(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        getSection(filterParams);
    }, []);

    const handleDelete = id => {
        confirm('Are you sure you want to delete this section?', () => {
            axios.post(resolveClientUrl(`Section/Delete/${id}`)).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    getSection(filterParams);
                } else {
                    alert(data.message);
                }
            }).catch(axiosError);
        });
    }

    return (
        <Fragment>
            <Filter onSubmit={getSection} />
            <Table data={section} onDelete={handleDelete}/>
        </Fragment>
    )
};

export default AppContainer;