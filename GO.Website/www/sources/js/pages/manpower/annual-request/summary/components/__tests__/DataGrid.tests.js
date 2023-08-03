import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import DataGrid from '../DataGrid';

import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';


describe('AppContainer', () => {
    beforeEach(() => {
        LangHelper.getResource = jest.fn(key => key);
        Site.resolveClientUrl = jest.fn(url => url);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    const request = [{
        requestId: 1,
        unitName: '1unitName',
        details: [{
            unitName: '1unitName', departmentName: 'departmentName1', sectionName: 'sectionName1', companyName: 'companyName1',
            expatCurrentHeadcount: 1, localCurrentHeadcount: 1, totalCurrentHeadcount: 2,
            expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
            expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
            expatAddition: 1, localAddition: 1, totalAddition: 2
        },
        {
            unitName: '1unitName', departmentName: 'departmentName1', sectionName: 'sectionName2', companyName: 'companyName1',
            expatCurrentHeadcount: 1, localCurrentHeadcount: 1, totalCurrentHeadcount: 2,
            expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
            expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
            expatAddition: 1, localAddition: 1, totalAddition: 2
        },
        {
            unitName: '1unitName', departmentName: 'departmentName2', sectionName: 'sectionName2', companyName: 'companyName2',
            expatCurrentHeadcount: 1, localCurrentHeadcount: 1, totalCurrentHeadcount: 2,
            expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
            expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
            expatAddition: 1, localAddition: 1, totalAddition: 2
        }]
    },
    {
        requestId: 2,
        unitName: '2unitName',
        details: [
            {
                unitName: '2unitName', departmentName: 'departmentName1', sectionName: 'sectionName1', companyName: 'companyName1',
                expatCurrentHeadcount: 1, localCurrentHeadcount: 1, totalCurrentHeadcount: 2,
                expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
                expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
                expatAddition: 1, localAddition: 1, totalAddition: 2
            },
            {
                unitName: '2unitName', departmentName: 'departmentName2', sectionName: 'sectionName2', companyName: 'companyName2',
                expatCurrentHeadcount: 1, localCurrentHeadcount: 1, totalCurrentHeadcount: 2,
                expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
                expatApprovedHeadcount: 1, localApprovedHeadcount: 1, totalApprovedHeadcount: 2,
                expatAddition: 1, localAddition: 1, totalAddition: 2
        }]
    }];
    const companies = ['companyName1', 'companyName2'];
    const unitTypes = ['1u', '2u'];
    const periodSelected = { year: 1 };

    it('renders correctly', () => {
        expect(render(<DataGrid requests={[]} companies={[]} unitTypes={[]} periodSelected={null}/>)).toMatchSnapshot();
    });

    test('table render successfully', () => {
        // ARRANGE
        render(<DataGrid requests={request} companies={companies} unitTypes={unitTypes} periodSelected={periodSelected} />);
        // ACT
        const table = screen.getByTestId('table-summary');
        // ASSERT
        expect(table.children.length).toBe(1);
        expect(table.querySelectorAll('tr').length).toBe(22);
        expect(table.querySelectorAll('td').length).toBe(327);
        expect(table.querySelectorAll('th').length).toBe(43);
    })
});