import React, { useState } from 'react';
import { Cell, Row } from '@jsroot/components/react-grid/index';
import dayjs from 'dayjs';

const RowItem = ({ period, onChangeStatus, deletePeriod, handleModalUpdateDialog }) => {
    const [isActive, setStatus] = useState(period.isActive);
    const dateProcessSettings = [];
    period.settings.map(item => {
        dateProcessSettings.push(<Cell key={item.fromDate} className="text-center">{dayjs(item.fromDate).format('DD MMM,YYYY')}</Cell>);
        dateProcessSettings.push(<Cell key={item.toDate} className="text-center">{dayjs(item.toDate).format('DD MMM,YYYY')}</Cell>);
    });
    const handleChangeStatus = () => {
        if (window.hasHostPermissionOnEmployee) {
            setStatus(!isActive);
            onChangeStatus(period.periodId, !isActive ? 1 : 0);
        }
    };
    const handleDeletePeriod = () => {
        if (window.hasHostPermissionOnEmployee) {
            deletePeriod(period.periodId);
        }
    };
    const handleUpdatePeriod = () => {
        if (window.hasHostPermissionOnEmployee) {
            handleModalUpdateDialog(true);
        }
    };
    return (
        <Row>
            <Cell className="text-center">{period.rowNum}</Cell>
            <Cell className="text-center">{period.name}</Cell>
            {dateProcessSettings}
            <Cell className="text-center">
                <input type='checkbox'
                    checked={isActive} disabled={!window.hasHostPermissionOnEmployee}
                    onChange={handleChangeStatus}
                />
            </Cell>
            <Cell className="text-center">
                <a className="icon-edit fa fa-edit" onClick={handleUpdatePeriod} ></a>
                <a className="icon-delete fa fa-trash" onClick={handleDeletePeriod}></a>
            </Cell>
        </Row>
    );
};

export default RowItem;