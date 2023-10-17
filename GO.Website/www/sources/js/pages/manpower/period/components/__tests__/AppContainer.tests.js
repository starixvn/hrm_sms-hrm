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
            data: []
        };
        const promise = Promise.resolve(response);
        DataService.postDataAsync = jest.fn(() => promise);
        // ACT
        await waitFor(() => {
            render(<AppContainer />);
        });

        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalled();
    })

    test('get member requests render successfully', async () => {
        // ARRANGE
        const response = {
            data: [{
                periodId: 2,
                name: 'tes',
                year: 2
            }]
        };
        const responseRequest = {
            data: [
                {
                    requestId: 2, unitName: 'test2', statusId: 1, currentProcessId: 1,
                    details: [
                        {
                            requestId: 1, siteId: 1, siteName: 'test1', companyName: 'CompanyName1', unitName: 'UnitName1',
                            departmentName: 'DepartmentName1', sectionName: 'SectionName1'
                        },
                        {
                            requestId: 1, siteId: 2, siteName: 'test2', companyName: 'CompanyName2', unitName: 'UnitName2',
                            departmentName: 'DepartmentName2', sectionName: 'SectionName2'
                        }
                    ]
                }
            ]
        };

        const promise = Promise.resolve(response);
        const promiseRequest = Promise.resolve(responseRequest);
        DataService.postDataAsync = jest.fn();
        DataService.postDataAsync.mockReturnValueOnce(promise).mockReturnValue(promiseRequest);
        // ACT
        const { result } = renderHook(() => AppContainer());
        await waitFor(() => {
            const filter = result.current.props.children[0].props;
            filter.onSubmit({
                periodId: 1,
                unitType: 'te'
            });
        });

        // ASSERT
        expect(DataService.postDataAsync).toHaveBeenCalledTimes(2);
    });
});