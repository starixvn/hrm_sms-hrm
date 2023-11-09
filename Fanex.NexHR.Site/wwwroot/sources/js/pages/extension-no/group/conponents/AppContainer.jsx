import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import { resolveClientUrl, axiosError, toastOptions } from '../../../../shared/common.functions';
import Table from './Table';
import Popup from './Popup';
import SweetModalDialog from '../../../../shared/components/SweetModalDialog';

export const AppContainer = () => {
    const [externalGroup, setExternalGroup] = useState([]);
    const [externalGroupDetail, setExternalGroupDetail] = useState(null);
    const [popup, setPopup] = useState({
        modalCreate: false,
        modalEdit: false
    });
    const [dialog, setDialog] = useState({
        externalId: null,
        showConfirm: false
    });
    const getAllEnternalGroup = () => {
        axios(resolveClientUrl('ExtensionNo/GetAllExternalGroup')).then(response => {
            setExternalGroup(response.data)
        }).catch(axiosError);
    };
    useEffect(() => {
        getAllEnternalGroup();
    }, []);

    const handleCreate = external => {
        setPopup({ modalCreate: false });
        axios.post(resolveClientUrl('ExtensionNo/CreateExternalGroup'), {
            teamName: external.TeamName.trim()
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                getAllEnternalGroup();
                toastr.success('Created successfully!', '', toastOptions());
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };

    const handleDelete = id => {
        setDialog({ showConfirm: true, externalId: id });
    };
    const handleCancel = () => {
        setDialog({ showConfirm: false });
    };

    const handleConfirm = () => {
        setDialog({ showConfirm: false });
        axios.post(resolveClientUrl(`ExtensionNo/DeleteExternalGroup/${dialog.externalId}`)).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getAllEnternalGroup();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };
    const handleGetDataById = (id, itemName) => {
        setPopup({ modalEdit: true })
        setExternalGroupDetail({
            id: id,
            teamName: itemName
        })
    };
    const handleEdit = external => {
        setPopup({ modalEdit: false });
        axios.post(resolveClientUrl('ExtensionNo/EditExternalGroup'), {
            teamName: external.TeamName.trim(),
            teamId: external.TeamId
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Updated successfully!', '', toastOptions());
                getAllEnternalGroup();
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    };
    const handleCreateExternal = () => {
        setPopup({ modalCreate: true });
    };

    const handleCancelExternal = () => {
        setPopup({
            modalCreate: false,
            modalEdit: false
        });
    };
    return (
        <Fragment>
            <a className="btn btn-request" onClick={handleCreateExternal}>
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
                data={externalGroupDetail}
                title='Edit'
            />
            <Table data={externalGroup} onDelete={handleDelete} onEdit={handleGetDataById} />
            <SweetModalDialog
                show={dialog.showConfirm}
                contents={<p>Are you sure you want to delete this Extension?</p>}
                onSubmit={handleConfirm}
                onHide={handleCancel}
            />
        </Fragment>
    )
};