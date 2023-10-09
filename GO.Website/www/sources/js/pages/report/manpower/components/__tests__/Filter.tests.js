import React from 'react';
import { render, fireEvent, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import Filter from '../Filter';

import LangHelper from '@jsroot/common/langHelper';

describe('Filter', () => {
    beforeAll(() => {
        LangHelper.getResource = jest.fn(key => key);
        window.hasHostPermissionOnSummaryRequest = true;
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    it('renders correctly', () => {
        expect(render(<Filter periods={[]} onSubmit={jest.fn()} requests={[]} updateStatus={jest.fn()} isHost={true}/>)).toMatchSnapshot();
    });

    test('filter period and unittype select box list [] render successfully', () => {
        // ARRANGE
        render(<Filter periods={[]} onSubmit={jest.fn()} requests={[]} updateStatus={jest.fn()} isHost={true} />);

        // ACT
        const elementPeriod = screen.getByText('Period');
        const elementUnitType = screen.getByText('UnitType');

        // ASSERT
        expect(elementPeriod).toBeInTheDocument();
        expect(elementUnitType).toBeInTheDocument();
    })

    test('filter period and unittype select box render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const periodSelected = { year: 0 };
        const onSubmit = jest.fn();
        const exportExcel = jest.fn();
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} exportExcel={exportExcel} periodSelected={periodSelected} />);

        // ACT
        const elementPeriod = screen.getByText('Period');
        const elementUnitType = screen.getByText('UnitType');

        // ASSERT
        expect(elementPeriod).toBeInTheDocument();
        expect(elementUnitType).toBeInTheDocument();
    })

    test('submit filter period and unittype select box render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const periodSelected = { year: 0 };
        const onSubmit = jest.fn();
        const exportExcel = jest.fn();
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} exportExcel={exportExcel} periodSelected={periodSelected} />);

        // ACT
        fireEvent.click(screen.getByText('Submit'));

        // ASSERT
        expect(onSubmit).toHaveBeenCalled();
    })
});
