import { toast } from 'react-toastify';
import React from 'react';
import { render, waitFor, renderHook } from '@testing-library/react';
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

    test('useEffect render successfully', async () => {
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
});