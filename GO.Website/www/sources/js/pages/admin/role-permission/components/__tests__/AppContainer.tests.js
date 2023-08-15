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
        window.hasHostPermission = true;
        toast.success = jest.fn();
        toast.error = jest.fn();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    test('renders correctly', () => {
        expect(render(<AppContainer />)).toMatchSnapshot();
    });

    test('useEffect get roles then render successfully', async () => {
        // ARRANGE
        const response = {
            data: [{
                roleId: 1,
                name: 'test'
            }]
        };
        const promise = Promise.resolve(response);
        DataService.getDataAsync = jest.fn(() => promise);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        // ASSERT
        expect(DataService.getDataAsync).toHaveBeenCalled();
    });

    test('get role permission then render successfully', async () => {
        // ARRANGE
        const response = { data: [{ roleId: 1, name: 'test' }] };
        const responseRolePermission = { data: [] };
        const promise = Promise.resolve(response);
        const promiseRolePermission = Promise.resolve(responseRolePermission);
        DataService.getDataAsync = jest.fn();
        DataService.getDataAsync.mockReturnValueOnce(promise).mockReturnValueOnce(promise).mockReturnValue(promiseRolePermission);
        // ACT
        let appContainer;
        await waitFor(() => {
            appContainer = renderHook(() => AppContainer());
        });
        const filter = appContainer.result.current.props.children.props.children[0].props;
        await filter.getRolePermission;
        // ASSERT
        expect(DataService.getDataAsync).toHaveBeenCalledTimes(1);
    });

    test('assign permission then render successfully', async () => {
        // ARRANGE
        const response = { data: [{ roleId: 1, name: 'test' }] };
        const responseRolePermission = { data: [] };
        const promise = Promise.resolve(response);
        const promiseRolePermission = Promise.resolve(responseRolePermission);
        DataService.getDataAsync = jest.fn();
        DataService.getDataAsync.mockReturnValueOnce(promise).mockReturnValueOnce(promise).mockReturnValue(promiseRolePermission);
        // ACT
        let appContainer;
        await waitFor(() => {
            appContainer = renderHook(() => AppContainer());
        });
        const submit = appContainer.result.current.props.children.props.children[2].props;
        console.log(submit);
        // ASSERT
        //expect(DataService.getDataAsync).toHaveBeenCalledTimes(1);
    });
});