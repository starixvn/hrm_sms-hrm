import React from 'react';
import { render, waitFor, renderHook, fireEvent } from '@testing-library/react';
import { screen } from '@testing-library/dom'
import '@testing-library/jest-dom';
import DataGrid from '../DataGrid';

describe('DataGrid', () => {
    const filter = {
        isActive: 1,
        pageIndex: 1,
        pageSize: 100
    };

    const gridData = {
        totalItems: 2,
        data: [
            {
                periodId: 1,
                isActive: true,
                name: 'test',
                settings: [
                    { periodId: 1, processId: 1, fromDate: "2023-09-12T00:00:00", toDate: "2023-10-12T00:00:00" },
                    { periodId: 1, processId: 2, fromDate: "2023-10-13T00:00:00", toDate: "2023-10-15T00:00:00" },
                    { periodId: 1, processId: 3, fromDate: "2023-10-16T00:00:00", toDate: "2023-10-18T00:00:00" }
                ],
                year: 1
            },
            {
                periodId: 2,
                isActive: true,
                name: 'test',
                settings: [
                    { periodId: 2, processId: 1, fromDate: "2023-09-12T00:00:00", toDate: "2023-10-12T00:00:00" },
                    { periodId: 2, processId: 2, fromDate: "2023-10-13T00:00:00", toDate: "2023-10-15T00:00:00" },
                    { periodId: 2, processId: 3, fromDate: "2023-10-16T00:00:00", toDate: "2023-10-18T00:00:00" }
                ],
                year: 2
            }
        ]
    }   
    it('renders correctly', () => {
        const onFilterChange = jest.fn();
        const getPeriods = jest.fn();
        const onUpdate = jest.fn();
        const onChangeStatus = jest.fn();
        const onDelete = jest.fn();

        expect(render(<DataGrid filter={filter}
            gridData={gridData}
            onFilterChange={onFilterChange}
            getPeriods={getPeriods}
            onUpdate={onUpdate}
            onChangeStatus={onChangeStatus}
            onDelete={onDelete}
        />)).toMatchSnapshot();
    });
});