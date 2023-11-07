import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Table from './Table';
import Popup from './Popup';

import { resolveClientUrl, axiosError, confirm, toastOptions } from '../../../shared/common.functions';

export const AppContainer = () => {
    const [createModal, setCreateModal] = useState(false);
    const [editModal, setEditModal] = useState(false);
    const [jobLevels, setJobLevels] = useState([]);
    const [jobLevelEdit, setJobLevelEdit] = useState([]);

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
        setEditModal(true);
        setJobLevelEdit({ id: jobLevel.id, name: jobLevel.name });
    };

    const handleCreate = values => {
        axios.post(resolveClientUrl(`JobLevel/Insert`), {
            name: values.name.trim(),
        })
            .then(response => {
                const { data } = response;
                setCreateModal(false);
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
                setEditModal(false);
                if (data.isSuccessful) {
                    toastr.success('Edit successfully!', '', toastOptions());
                    getJobLevels();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
    };

    const handleDelete = jobLevel => {
        confirm('Are you sure you want to delete?', () => {
            axios.post(resolveClientUrl('JobLevel/Delete'), {
                id: jobLevel.id
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    toastr.success('Deleted successfully!', '', toastOptions());
                    getJobLevels();
                } else {
                    toastr.error(data.message, '', toastOptions());
                }
            }).catch(axiosError);
        });
    };

    const handleCreateModal = () => {
        setCreateModal(!createModal);
    };

    const handleEditModal = () => {
        setEditModal(false);
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
                show={createModal}
                onHide={handleCreateModal}
                onSubmit={handleCreate}
                title='Add' />
            <Popup
                show={editModal}
                onHide={handleEditModal}
                onSubmit={handleEdit}
                data={jobLevelEdit}
                title='Edit' />
            <Table
                data={jobLevels}
                onEdit={handleEditInfo}
                onDelete={handleDelete} />
        </Fragment>
    );
};