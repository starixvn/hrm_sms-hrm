import React, { Fragment, useState, useEffect } from 'react';
import { Button } from 'react-bootstrap';
import { Table } from './Table';
import { Popup } from './Popup';
import axios from 'axios';
import { resolveClientUrl, axiosError, alert, confirm } from '../../../shared/common.functions';

export const AppContainer = () => {
    const [createModal, setCreateModal] = useState(false);
    const [editModal, setEditModal] = useState(false);
    const [jobLevel, setJobLevel] = useState([]);
    const [jobLevelEdit, setJobLevelEdit] = useState([]);

    const handleGetJobLevel = () => {
        axios.post(resolveClientUrl('JobLevel/GetJobLevel'), {
        }).then(response => {
            setJobLevel(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetJobLevel();
    }, []);

    const handleEditInfo = (id, name) => {
        setEditModal(true);
        setJobLevelEdit({ id: id, name: name });
    };

    const handleCreate = (values) => {
        axios.post(resolveClientUrl(`JobLevel/Insert`), {
            name: values.name.trim(),
        })
            .then(response => {
                const { data } = response;
                setCreateModal(false);
                if (data.IsSuccessful) {
                    alert('Create successfully!');
                    handleGetJobLevel();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
    };

    const handleEdit = (values) => {
        axios.post(resolveClientUrl(`JobLevel/Update`), {
            id: jobLevelEdit.id,
            name: values.name.trim(),
        })
            .then(response => {
                const { data } = response;
                setEditModal(false);
                if (data.IsSuccessful) {
                    alert('Edit successfully!');
                    handleGetJobLevel();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
    };

    const handleDelete = (id) => {
        confirm('Are you sure you want to delete?', () => {
            axios.post(resolveClientUrl('JobLevel/Delete'), {
                id: id
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    alert('Delete successfully!');
                    handleGetJobLevel();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    }

    return (
        <Fragment>
            {window.hasPermission === 'True' &&
                <div className="filter">
                    <Button className="btn btn-request" type="button" onClick={() => setCreateModal(true)}>
                        <span className="fa fa-plus" /> New Job Level
                    </Button>
                </div>
            }
            <Popup
                show={createModal}
                onHide={() => setCreateModal(false)}
                onSubmit={handleCreate}
                title='Add'
            />
            <Popup
                show={editModal}
                onHide={() => setEditModal(false)}
                onSubmit={handleEdit}
                data={jobLevelEdit}
                title='Edit' />
            <Table
                data={jobLevel}
                handleEditInfo={handleEditInfo}
                handleDelete={handleDelete} />
        </Fragment>
    )
};