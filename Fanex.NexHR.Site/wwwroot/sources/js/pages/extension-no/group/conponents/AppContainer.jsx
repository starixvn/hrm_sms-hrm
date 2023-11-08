import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';

import { resolveClientUrl, axiosError, confirm, alert } from '../../../../shared/common.functions';
import Table from './Table';
import Popup from './Popup';

export const AppContainer = () => {
    const [data, setData] = useState([]);
    const [single, setSingle] = useState(null);
    const [modalCreate, setModalCreate] = useState(false);
    const [modalEdit, setModalEdit] = useState(false);

    const handleGetAllEnternalGroup = () => {
        axios(resolveClientUrl('ExtensionNo/GetAllExternalGroup')).then(response => {
            setData(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetAllEnternalGroup();
    }, [])

    const handleCreateData = data => {
        axios.post(resolveClientUrl('ExtensionNo/CreateExternalGroup'), {
            teamName: data.TeamName.trim()
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                handleGetAllEnternalGroup();
                setModalCreate(false);
            }
        }).catch(axiosError);
    }

    const handleDeleteData = id => {
        confirm('Are you sure you want to delete this external group?', () => {
            axios.post(resolveClientUrl(`ExtensionNo/DeleteExternalGroup/${id}`)).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    alert('Delete successfully!');
                    handleGetAllEnternalGroup();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    }

    const handleGetDataById = (id , itemName) => {
        if (id) {
            setModalEdit(true);
            setSingle({
                id: id,
                teamName: itemName
            })
        }
    }

    const handleEditData = data => {
        axios.post(resolveClientUrl('ExtensionNo/EditExternalGroup'), {
            teamName: data.TeamName.trim(),
            teamId: data.TeamId
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                handleGetAllEnternalGroup();
                setModalEdit(false);
            }
        }).catch(axiosError);
    }

    return (
        <Fragment>
            <a className="btn btn-request" onClick={() => setModalCreate(true)}>
                <span className="fa fa-plus"></span> New External Group
            </a>
            <Popup
                show={modalCreate}
                onHide={() => setModalCreate(false)}
                onSubmit={handleCreateData}
                title='Add new'
            />

            <Popup
                show={modalEdit}
                onHide={() => setModalEdit(false)}
                onSubmit={handleEditData}
                data={single}
                title='Edit'
            />
            <Table data={data} onDelete={handleDeleteData} onEdit={handleGetDataById }/>
        </Fragment>
    )
};