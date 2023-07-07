import React, { useState } from 'react';
import dayjs from 'dayjs';

import { Cell, Row } from '@jsroot/components/react-grid/index';

import { DateFormat } from '@jsroot/common/constants';
import Site from '@jsroot/common/site';

const RowItem = ({ rowIndex, period, onChangeStatus, onDelete, onUpdate }) => {
    const [isActive, setStatus] = useState(period.isActive);

    const handleChangeStatus = () => {
        if (window.hasHostPermissionOnPeriod) {
            setStatus(!isActive);
            onChangeStatus(period.periodId, !isActive ? 1 : 0);
        }
    };

    const handleDeletePeriod = () => onDelete(period);

    const handleUpdatePeriod = () => onUpdate(period);

    const renderProcessSetting = () => {
        const cells = [];
        period.settings.map(item => {
            cells.push(
                <Cell key={`${item.periodId}-${item.processId}-fromdate`} className="text-center">
                    {dayjs(item.fromDate).format(DateFormat)}
                </Cell>
            );
            cells.push(
                <Cell key={`${item.periodId}-${item.processId}-todate`} className="text-center">
                    {dayjs(item.toDate).format(DateFormat)}
                </Cell>
            );
        });

        return cells;
    };

    return (
        <Row>
            <Cell className="text-center">{rowIndex}</Cell>
            <Cell>{period.name}</Cell>
            {renderProcessSetting()}
            <Cell className="text-center">
                <input type='checkbox'
                    checked={isActive} disabled={!window.hasHostPermissionOnPeriod}
                    onChange={handleChangeStatus}
                />
            </Cell>
            <Cell className="text-center">
                <a href={Site.resolveClientUrl(`Manpower/Period/Detail/${period.periodId}`)} className="icon-edit fa fa-search" />
                {
                    window.hasHostPermissionOnPeriod &&
                    <>
                        <a className="icon-edit fa fa-edit" onClick={handleUpdatePeriod} />
                        <a className="icon-delete fa fa-trash" onClick={handleDeletePeriod} />
                    </>
                }
            </Cell>
        </Row>
    );
};

export default RowItem;