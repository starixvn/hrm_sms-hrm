import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import Popup from './Popup';
import Filter from './Filter';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import SweetModalDialog from '../../../shared/components/SweetModalDialog';

const AppContainer = () => {
    const [jobTitles, setJobTitles] = useState([]);
    const [jobCategory, setJobCategory] = useState([]);
    const [filterParams, setFilterParams] = useState({ jobTitle: '' });
    const [jobTitleEdit, setJobTitleEdit] = useState([]);
    const [modalDialog, setModalDialog] = useState({
        showConfirm: false
    });
    const [openPopup, setOpenPopup] = useState({
        openCreate: false,
        openEdit: false
    });

    const getJobTitles = params => {
        setFilterParams(params);
        axios.post(resolveClientUrl('JobTitle/GetJobTitles'), params)
            .then(response => {
                setJobTitles(response.data)
            }).catch(axiosError);
    };

    const getJobCategory = () => {
        axios.post(resolveClientUrl('JobTitle/GetJobCategory'), {
        }).then(response => {
            setJobCategory(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getJobTitles(filterParams);
        getJobCategory();
    }, []);

    const handleEditInfo = jobTitle => {
        setOpenPopup({ openEdit: true });
        setJobTitleEdit({
            id: jobTitle.id,
            name: jobTitle.name,
            categoryId: jobTitle.categoryId
        });
    };

    const handleCreate = jobTitle => {
        axios.post(resolveClientUrl(`JobTitle/Insert`), {
            name: jobTitle.name.trim(),
            categoryId: jobTitle.categoryId ? jobTitle.categoryId : jobCategory[0].value
        })
            .then(response => {
                const { data } = response;
                setOpenPopup({ openCreate: false });
                if (data.isSuccessful) {
                    toastr.success('Create successfully!', '', toastOptions());
                    getJobTitles();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
    };

    const handleEdit = jobTitle => {
        axios.post(resolveClientUrl(`JobTitle/Update`), {
            id: jobTitleEdit.id,
            name: jobTitle.name.trim(),
            categoryId: jobTitle.categoryId
        })
            .then(response => {
                const { data } = response;
                setOpenPopup({ openEdit: false });
                if (data.isSuccessful) {
                    toastr.success('Edit successfully!', '', toastOptions());
                    getJobTitles();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
    };

    const handleCreateModal = () => {
        setOpenPopup({ openCreate: !openPopup.openCreate });

    };

    const handleEditModal = () => {
        setOpenPopup({ openEdit: false });

    };

    const handleDelete = jobTitle => {
        setModalDialog({
            showConfirm: true,
            jobTitleDelete: jobTitle
        });
    };

    const handleConfirm = () => {
        setModalDialog({
            showConfirm: false
        });
        axios.post(resolveClientUrl('JobTitle/Delete'), {
            id: modalDialog.jobTitleDelete.id
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getJobLevels();
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
            <Filter
                onSubmit={getJobTitles}
                setOpenPopup={handleCreateModal} />
            <Popup
                show={openPopup.openCreate}
                onHide={handleCreateModal}
                onSubmit={handleCreate}
                title='Add'
                jobCategory={jobCategory} />
            <Popup
                show={openPopup.openEdit}
                onHide={handleEditModal}
                onSubmit={handleEdit}
                title='Edit'
                jobCategory={jobCategory}
                data={jobTitleEdit} />
            <Table
                data={jobTitles}
                onEdit={handleEditInfo}
                onDelete={handleDelete} />
            <SweetModalDialog
                show={modalDialog.showConfirm}
                message="Are you sure you want to delete?"
                onConfirm={handleConfirm}
                onCancel={handleCancel} />
        </Fragment>
    );
};

export default AppContainer;