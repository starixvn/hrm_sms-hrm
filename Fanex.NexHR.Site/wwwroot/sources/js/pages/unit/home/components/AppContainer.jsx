import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import toastr from 'toastr';

import Filter from './Filter';
import Table from './Table';

import { axiosError, resolveClientUrl, toastOptions } from '../../../../shared/common.functions';
import ConfirmPopup from '../../../../shared/components/ConfirmPopup';

const AppContainer = () => {
    const [filterParams, setFilterParams] = useState({ isActive: true });
    const [units, setUnits] = useState([]);

    const [dialog, setDialog] = useState({
        unit: null,
        showConfirm: false
    });

    const getUnits = params => {
        setFilterParams(params);

        axios.post(resolveClientUrl('Unit/GetUnits'), params)
            .then(response => {
                setUnits(response.data)
            }).catch(axiosError);
    };

    useEffect(() => {
        getUnits(filterParams);
    }, []);

    const handleDelete = unit => {
        setDialog({ showConfirm: true, unit: unit });
    };

    const handleCancel = () => {
        setDialog({ showConfirm: false });
    };

    

    const handleConfirm = () => {
        setDialog({ showConfirm: false });
        axios.post(resolveClientUrl('Unit/Delete'), {
            unitId: dialog.unit.id
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Deleted successfully!', '', toastOptions());
                getUnits(filterParams);
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    }
    return (
        <Fragment>
            <Filter onSubmit={getUnits} />
            <Table data={units} onDelete={handleDelete} />
            <ConfirmPopup
                show={dialog.showConfirm}
                message='Are you sure you want to delete this unit?'
                onConfirm={handleConfirm}
                onCancel={handleCancel}
            />
        </Fragment>
    );
};

export default AppContainer;