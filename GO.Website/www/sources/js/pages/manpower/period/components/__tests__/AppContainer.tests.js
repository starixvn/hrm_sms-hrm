import { toast } from 'react-toastify';
import React from 'react';
import { render, waitFor, renderHook, fireEvent, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import AppContainer from '../AppContainer';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

describe('AppContainer', () => {
    beforeEach(() => {
        LangHelper.getResource = jest.fn(key => key);
        Site.resolveClientUrl = jest.fn(url => url);
        Site.blockUI = jest.fn(url => url);
        Site.unblockUI = jest.fn(url => url);
        toast.success = jest.fn();
        toast.error = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('renders correctly', () => {
        expect(render(<AppContainer />)).toMatchSnapshot();
    });

    it('data grid renders correctly', () => {
        const response = {
            data: {
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
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        render(<AppContainer />);

        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalled();
    });

    it('open popup create renders correctly', () => {
        const response = {
            data: {
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
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        window.hasHostPermissionOnPeriod = true;
        render(<AppContainer />);
        fireEvent.click(screen.getByText('AddNew'));

        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalled();
    });

    it('close popup create renders correctly', () => {
        const response = {
            data: {
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
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        window.hasHostPermissionOnPeriod = true;
        render(<AppContainer />);
        fireEvent.click(screen.getByText('AddNew'));
        fireEvent.click(screen.getByText('Cancel'));

        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalled();
    });

    it('open popup update renders correctly', async () => {
        const response = {
            data: {
                data: [{
                    periodId: 1,
                    isActive: true,
                    name: 'test',
                    settings: [
                        { periodId: 1, processId: 1, fromDate: "2023-09-12T00:00:00", toDate: "2023-10-12T00:00:00" },
                        { periodId: 1, processId: 2, fromDate: "2023-10-13T00:00:00", toDate: "2023-10-15T00:00:00" },
                        { periodId: 1, processId: 3, fromDate: "2023-10-16T00:00:00", toDate: "2023-10-18T00:00:00" }
                    ],
                    year: 1
                }],
                totalItems: 1
            }
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        window.hasHostPermissionOnPeriod = true;
        await waitFor(() => {
            render(<AppContainer />);
        });
        fireEvent.click(screen.getAllByTestId('update-period')[0]);
        fireEvent.click(screen.getByText('Cancel'));

        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalled();
    });

    it('open popup delete renders correctly', async () => {
        const response = {
            data: {
                data: [{
                    periodId: 1,
                    isActive: true,
                    name: 'test',
                    settings: [
                        { periodId: 1, processId: 1, fromDate: "2023-09-12T00:00:00", toDate: "2023-10-12T00:00:00" },
                        { periodId: 1, processId: 2, fromDate: "2023-10-13T00:00:00", toDate: "2023-10-15T00:00:00" },
                        { periodId: 1, processId: 3, fromDate: "2023-10-16T00:00:00", toDate: "2023-10-18T00:00:00" }
                    ],
                    year: 1
                }],
                totalItems: 1
            }
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        //  ACT
        window.hasHostPermissionOnPeriod = true;
        await waitFor(() => {
            render(<AppContainer />);
        });
        fireEvent.click(screen.getAllByTestId('delete-period')[0]);
        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalled();
    });
});