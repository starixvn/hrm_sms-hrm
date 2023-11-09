import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import Filter from './Filter';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

const AppContainer = () => {
    const [departments, setDepartments] = useState([]);
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [modalDialog, setModalDialog] = useState({
        showConfirm: false
    });

    const getDepartments = params => {
        setFilterParams(params);
        axios.post(resolveClientUrl('Department/GetDepartments'), params)
            .then(response => {
                setDepartments(response.data)
            }).catch(axiosError);
    };

    useEffect(() => {
        getDepartments(filterParams);
    }, []);

    const handleDelete = department => {
        setModalDialog({
            showConfirm: true,
            department: department
        });
    };

    const handleConfirm = () => {
        setModalDialog({
            showConfirm: false
        });
        axios.post(resolveClientUrl('Department/Delete'), {
            id: modalDialog.department.id
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getDepartments(filterParams);
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleCancel = () => {
        setModalDialog({
            showConfirm: false
        });
    };

    return (
        <Fragment>
            <Filter onSubmit={getDepartments} />
            <Table data={departments} onDelete={handleDelete} />
            <SweetModalDialog
                show={modalDialog.showConfirm}
                contents="Are you sure you want to delete this department?"
                onSubmit={handleConfirm}
                onHide={handleCancel} />
        </Fragment>
    );
};

export default AppContainer;