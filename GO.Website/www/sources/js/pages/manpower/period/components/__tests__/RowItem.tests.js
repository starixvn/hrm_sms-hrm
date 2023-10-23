import React from 'react';
import { render, waitFor, renderHook, fireEvent } from '@testing-library/react';
import { screen } from '@testing-library/dom'
import '@testing-library/jest-dom';
import RowItem from '../RowItem';

describe('RowItem', () => {
    const period = {
        periodId: 1,
        isActive: true,
        name: 'test',
        settings: [
            { periodId: 1, processId: 1, fromDate: "2023-09-12T00:00:00", toDate: "2023-10-12T00:00:00" },
            { periodId: 1, processId: 2, fromDate: "2023-10-13T00:00:00", toDate: "2023-10-15T00:00:00" },
            { periodId: 1, processId: 3, fromDate: "2023-10-16T00:00:00", toDate: "2023-10-18T00:00:00" }
        ],
        year: 1
    }
    it('renders correctly', () => {
        expect(render(<RowItem rowIndex={1} period={period} onChangeStatus={jest.fn()} onDelete={jest.fn()} onUpdate={jest.fn()} />)).toMatchSnapshot();
    });

    it('click change status when permission true and period active true correctly', () => {
        const onChangeStatus = jest.fn();
        window.hasHostPermissionOnPeriod = true;
        render(<RowItem rowIndex={1} period={period} onChangeStatus={onChangeStatus} onDelete={jest.fn()} onUpdate={jest.fn()} />)
        fireEvent.click(screen.getAllByTestId('input-element')[0]);
        expect(onChangeStatus).toBeCalledWith(period.periodId, 0);
    });

    it('click change status when permission true and period active false correctly', () => {
        const onChangeStatus = jest.fn();
        window.hasHostPermissionOnPeriod = true;
        period.isActive = false;
        render(<RowItem rowIndex={1} period={period} onChangeStatus={onChangeStatus} onDelete={jest.fn()} onUpdate={jest.fn()} />)
        fireEvent.click(screen.getAllByTestId('input-element')[0]);
        expect(onChangeStatus).toBeCalledTimes(1);
    });

    it('click change status when permission false correctly', () => {
        const onChangeStatus = jest.fn();
        window.hasHostPermissionOnPeriod = false;
        render(<RowItem rowIndex={1} period={period} onChangeStatus={onChangeStatus} onDelete={jest.fn()} onUpdate={jest.fn()} />)
        fireEvent.click(screen.getAllByTestId('input-element')[0]);
        expect(onChangeStatus).toBeCalledTimes(0);
    });
});