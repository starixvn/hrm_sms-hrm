import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';

import Filter from './Filter';
import Table from './Table';

import { axiosError, resolveClientUrl, confirm, alert } from '../../../../shared/common.functions';

const AppContainer = () => {
    const [section, setSection] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });

    const getSections = params => {
        setFilterParams(params);
        axios.post(resolveClientUrl('Section/GetSections'), params).then(response => {
            setSection(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getSections(filterParams);
    }, []);

    const handleDelete = id => {
        confirm('Are you sure you want to delete this section?', () => {
            axios.post(resolveClientUrl(`Section/Delete/${id}`)).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    alert('Deleted successfully!');
                    getSections(filterParams);
                } else {
                    alert(data.message);
                }
            }).catch(axiosError);
        });
    };

    return (
        <Fragment>
            <Filter onSubmit={getSections} />
            <Table data={section} onDelete={handleDelete} />
        </Fragment>
    );
};

export default AppContainer;