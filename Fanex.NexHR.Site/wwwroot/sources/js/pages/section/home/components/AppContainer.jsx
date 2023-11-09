import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Filter from './Filter';
import Table from './Table';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

const AppContainer = () => {
    const [section, setSection] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [dialog, setDialog] = useState({
        section : null,
        showConfirm: false
    });

    const getSections = params => {
        setFilterParams(params);
        axios.post(resolveClientUrl('Section/GetSections'), params).then(response => {
            setSection(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getSections(filterParams);
    }, []);

    const handleDelete = section => {
        setDialog({ showConfirm: true, section: section });
    };

    const handleCancel = () => {
        setDialog({ showConfirm: false });
    };


    const handleConfirm = () => {
        setDialog({ showConfirm: false });
        const id = dialog.section;
        axios.post(resolveClientUrl(`Section/Delete/${id}`)).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getSections(filterParams);
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    return (
        <Fragment>
            <Filter onSubmit={getSections} />
            <Table data={section} onDelete={handleDelete} />
            <SweetModalDialog
                show={dialog.showConfirm}
                contents='Are you sure you want to delete this Section?'
                onSubmit={handleConfirm}
                onHide={handleCancel}
            />
        </Fragment>
    );
};

export default AppContainer;