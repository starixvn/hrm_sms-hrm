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
        const onSubmit = jest.fn();
        const requests = [{ currentProcessId: 2, requestId: 1 }, { currentProcessId: 3, requestId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        const elementPeriod = screen.getByText('Period');
        const elementUnitType = screen.getByText('UnitType');


        // ASSERT
        expect(elementPeriod).toBeInTheDocument();
        expect(elementUnitType).toBeInTheDocument();
    })

    test('submit filter period and unittype select box render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [{ currentProcessId: 2, requestId: 1 }, { currentProcessId: 3, requestId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        fireEvent.click(screen.getByText('Submit'));

        // ASSERT
        expect(onSubmit).toHaveBeenCalled();
    })

    test('click button reset then click button cancel render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [{ currentProcessId: 2, requestId: 1 }, { currentProcessId: 3, requestId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        fireEvent.click(screen.getByText('Reset'));
        fireEvent.click(screen.getByText('Cancel'));
        const elementReset = screen.getByText('Reset');

        // ASSERT
        expect(elementReset).toBeInTheDocument();
    })

    test('click button reset then click button x render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [{ currentProcessId: 2, requestId: 1 }, { currentProcessId: 3, requestId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        fireEvent.click(screen.getByText('Reset'));
        fireEvent.click(screen.getByText('×'));
        const elementReset = screen.getByText('Reset');

        // ASSERT
        expect(elementReset).toBeInTheDocument();
    })

    test('click button reset then click button ok render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [{ currentProcessId: 2, requestId: 1 }, { currentProcessId: 3, requestId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        const elementPeriod = screen.getByText('Period');
        const elementUnitType = screen.getByText('UnitType');
        fireEvent.click(screen.getByText('Reset'));
        fireEvent.click(screen.getByText('OK'));

        // ASSERT
        expect(elementPeriod).toBeInTheDocument();
        expect(elementUnitType).toBeInTheDocument();
    })

    test('click button verify then click button cancel render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [
            { currentProcessId: 2, requestId: 1, statusId: 1 },
            { currentProcessId: 2, requestId: 2, statusId: 1 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        fireEvent.click(screen.getByText('Verify'));
        fireEvent.click(screen.getByText('Cancel'));
        const elementVerify = screen.getByText('Verify');

        // ASSERT
        expect(elementVerify).toBeInTheDocument();
    })

    test('click button verify then click button x render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [
            { currentProcessId: 2, requestId: 1, statusId: 1 },
            { currentProcessId: 2, requestId: 2, statusId: 1 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        fireEvent.click(screen.getByText('Verify'));
        fireEvent.click(screen.getByText('×'));
        const elementVerify = screen.getByText('Verify');

        // ASSERT
        expect(elementVerify).toBeInTheDocument();
    })

    test('click button verify then click button OK render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [
            { currentProcessId: 2, requestId: 1, statusId: 1 },
            { currentProcessId: 2, requestId: 2, statusId: 1 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        const elementPeriod = screen.getByText('Period');
        const elementUnitType = screen.getByText('UnitType');
        fireEvent.click(screen.getByText('Verify'));
        fireEvent.click(screen.getByText('OK'));

        // ASSERT
        expect(elementPeriod).toBeInTheDocument();
        expect(elementUnitType).toBeInTheDocument();
    })

    test('click button approve then click button cancel render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [
            { currentProcessId: 3, requestId: 1, statusId: 2 },
            { currentProcessId: 3, requestId: 2, statusId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        fireEvent.click(screen.getByText('Approve'));
        fireEvent.click(screen.getByText('Cancel'));
        const elementApprove = screen.getByText('Approve');

        // ASSERT
        expect(elementApprove).toBeInTheDocument();
    })

    test('click button approve then click button x render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [
            { currentProcessId: 3, requestId: 1, statusId: 2 },
            { currentProcessId: 3, requestId: 2, statusId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        fireEvent.click(screen.getByText('Approve'));
        fireEvent.click(screen.getByText('×'));
        const elementApprove = screen.getByText('Approve');

        // ASSERT
        expect(elementApprove).toBeInTheDocument();
    })

    test('click button approve then click button OK render successfully', () => {
        const periods = [{ periodId: 1, name: 'test' }];
        const onSubmit = jest.fn();
        const requests = [
            { currentProcessId: 3, requestId: 1, statusId: 2 },
            { currentProcessId: 3, requestId: 2, statusId: 2 }];
        const updateStatus = jest.fn();
        const isHost = true;
        // ARRANGE
        render(<Filter periods={periods} onSubmit={onSubmit} requests={requests} updateStatus={updateStatus} isHost={isHost} />);

        // ACT
        const elementPeriod = screen.getByText('Period');
        const elementUnitType = screen.getByText('UnitType');
        fireEvent.click(screen.getByText('Approve'));
        fireEvent.click(screen.getByText('OK'));

        // ASSERT
        expect(elementPeriod).toBeInTheDocument();
        expect(elementUnitType).toBeInTheDocument();
    })
});
