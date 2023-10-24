import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../../shared/common.functions';
import { Table } from './Table';
import { Popup } from './Popup';
export const AppContainer = () => {
    const [data, setData] = useState([]);
    const [single, setSingle] = useState(null);
    const [modalShow, setModalShow] = useState(false);
    const [modalEdit, setModalEdit] = useState(false);

    const handleGetAllEnternalGroup = () => {
        axios(resolveClientUrl('ExtensionNo/GetAllExternalGroup')).then(response => {
            const { data } = response;
            if (data) {
                setData(response.data)
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetAllEnternalGroup();
    }, [])

    const handleCreateData = (data) => {
        if (data) {
            axios.post(resolveClientUrl('ExtensionNo/CreateExternalGroup'), {
                teamName: data.TeamName.trim()
            }).then(response => {
                const { data } = response;
                if (data.isSuccessful) {
                    handleGetAllEnternalGroup();
                    setModalShow(false);
                }
            }).catch(axiosError);
        }
    }

    const handleDeleteData = (id) => {
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

    const handleGetDataById = (id) => {
        if (id) {
            setModalEdit(true);
            const newData = data.find(item => item.teamId === id);
            setSingle(newData)
        }
    }

    const handleEditData = (data) => {
        if (data) {
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
    }

    return (
        <Fragment>
            <a className="btn btn-request" onClick={() => setModalShow(true)}>
                <span className="fa fa-plus"></span> New External Group
            </a>
            <Popup
                show={modalShow}
                onHide={() => setModalShow(false)}
                onSubmit={handleCreateData}
            />
            <Popup
                show={modalEdit}
                onHide={() => setModalEdit(false)}
                onSubmit={handleEditData}
                data={single}
            />
            <Table data={data} handleDeleteData={handleDeleteData} handleGetDataById={handleGetDataById }/>
        </Fragment>
    )
};