import React from 'react';
import { render, fireEvent, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import '@testing-library/jest-dom';
import AppContainer from '../AppContainer';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

describe('Filter', () => {
    beforeAll(() => {
        LangHelper.getResource = jest.fn(key => key);
        Site.resolveClientUrl = jest.fn(url => url);
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    test('filter period and unittype select box list [] render successfully', async () => {
        // ARRANGE
        const response = {
            data: []
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });
        const elementPeriod = screen.getByText('Per iod');

        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalled();
    })
});

