import React, { useState } from 'react';
import { Cell, Row } from '@jsroot/components/react-grid/index';
import dayjs from 'dayjs';

const RowItem = ({ period, onChangeStatus, deletePeriod }) => {
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
                <a className="icon fa fa-edit" ></a>
                <a className="icon fa fa-trash" onClick={handleDeletePeriod}></a>
            </Cell>
        </Row>
    );
};

export default RowItem;