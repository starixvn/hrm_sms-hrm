import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';

import { resolveClientUrl, axiosError, alert } from '../../../../shared/common.functions';
import Table from './Table';
import Popup from './Popup';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

export const AppContainer = () => {
    const [data, setData] = useState([]);
    const [single, setSingle] = useState(null);
    const [popup, setPopup] = useState({
        modalCreate: false,
        modalEdit: false
    })

    const [dialog, setDialog] = useState({
        externalId : null,
        showConfirm: false
    });

    const handleGetAllEnternalGroup = () => {
        axios(resolveClientUrl('ExtensionNo/GetAllExternalGroup')).then(response => {
            setData(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetAllEnternalGroup();
    }, [])

    const handleCreate = data => {
        setPopup({ modalCreate: false })
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

    const handleDelete = id => {
        setDialog({ showConfirm: true, externalId: id });
    };
    const handleCancel = () => {
        setDialog({ showConfirm: false });
    };

    const handleConfirm = () => {
        setDialog({ showConfirm: false });
        const id = dialog.externalId;
        axios.post(resolveClientUrl(`ExtensionNo/DeleteExternalGroup/${id}`)).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                alert('Delete successfully!');
                handleGetAllEnternalGroup();
            } else {
                alert(data.Message);
            }
        }).catch(axiosError);
    }
    const handleGetDataById = (id, itemName) => {
        setPopup({ modalEdit : true })
        setSingle({
            id: id,
            teamName: itemName
        })
    }
    const handleEdit = data => {
        setPopup({ modalEdit: false });
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
    const handleCreateExternal = () => {
        setPopup({ modalCreate: true });
    }

    const handleCancelExternal = () => {
        setPopup({
            modalCreate: false,
            modalEdit: false
        });
    }
    return (
        <Fragment>
            <a className="btn btn-request" onClick={handleCreateExternal }>
                <span className="fa fa-plus"></span> New External Group
            </a>
            <Popup
                show={popup.modalCreate}
                onHide={handleCancelExternal}
                onSubmit={handleCreate}
                title='Add new'
            />

            <Popup
                show={popup.modalEdit}
                onHide={handleCancelExternal}
                onSubmit={handleEdit}
                data={single}
                title='Edit'
            />
            <Table data={data} onDelete={handleDelete} onEdit={handleGetDataById} />
            <SweetModalDialog
                show={dialog.showConfirm}
                message='Are you sure you want to delete this Extension?'
                onConfirm={handleConfirm}
                onCancel={handleCancel}
            />
        </Fragment>
    )
};