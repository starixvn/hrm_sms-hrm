import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import Popup from './Popup';

import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';
import SweetModalDialog from '../../../shared/components/SweetModalDialog';

export const AppContainer = () => {

    const [jobLevels, setJobLevels] = useState([]);
    const [jobLevelEdit, setJobLevelEdit] = useState([]);
    const [modalDialog, setModalDialog] = useState({
        showConfirm: false
    });
    const [openPopup, setOpenPopup] = useState({
        openCreate: false,
        openEdit: false
    });

    const getJobLevels = () => {
        axios.post(resolveClientUrl('JobLevel/GetJobLevels'), {
        }).then(response => {
            setJobLevels(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        getJobLevels();
    }, []);

    const handleEditInfo = jobLevel => {
        setOpenPopup({ openEdit: true });
        setJobLevelEdit({ id: jobLevel.id, name: jobLevel.name });
    };

    const handleCreate = jobLevel => {
        axios.post(resolveClientUrl(`JobLevel/Insert`), {
            name: jobLevel.name.trim(),
        })
            .then(response => {
                const { data } = response;
                setOpenPopup({ openCreate: false });
                if (data.isSuccessful) {
                    toastr.success('Create successfully!', '', toastOptions());
                    getJobLevels();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
    };

    const handleEdit = jobLevel => {
        axios.post(resolveClientUrl(`JobLevel/Update`), {
            id: jobLevelEdit.id,
            name: jobLevel.name.trim(),
        })
            .then(response => {
                const { data } = response;
                setOpenPopup({ openEdit: false });
                if (data.isSuccessful) {
                    toastr.success('Edit successfully!', '', toastOptions());
                    getJobLevels();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
    };

    const handleDelete = jobLevel => {
        setModalDialog({
            showConfirm: true,
            jobLevel: jobLevel
        });
    };

    const handleCreateModal = () => {
        setOpenPopup({ openCreate: !openPopup.openCreate });
    };

    const handleEditModal = () => {
        setOpenPopup({ openEdit: false });
    };

    const handleConfirm = () => {
        setModalDialog({
            showConfirm: false
        });
        axios.post(resolveClientUrl('JobLevel/Delete'), {
            id: modalDialog.jobLevel.id
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
            {
                window.hasPermission === 'True' &&
                <div className="filter">
                    <button className="btn btn-request" type="button" onClick={handleCreateModal}>
                        <span className="fa fa-plus" /> New Job Level
                    </button>
                </div>
            }
            <Popup
                show={openPopup.openCreate}
                onHide={handleCreateModal}
                onSubmit={handleCreate}
                title='Add' />
            <Popup
                show={openPopup.openEdit}
                onHide={handleEditModal}
                onSubmit={handleEdit}
                data={jobLevelEdit}
                title='Edit' />
            <Table
                data={jobLevels}
                onEdit={handleEditInfo}
                onDelete={handleDelete} />
            <SweetModalDialog
                show={modalDialog.showConfirm}
                contents={<p>Are you sure you want to delete?</p>}
                onSubmit={handleConfirm}
                onHide={handleCancel} />
        </Fragment>
    );
};