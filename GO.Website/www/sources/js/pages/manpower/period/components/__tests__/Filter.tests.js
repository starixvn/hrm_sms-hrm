import React from 'react';
import { render, waitFor, renderHook, fireEvent } from '@testing-library/react';
import { screen } from '@testing-library/dom'
import '@testing-library/jest-dom';
import Filter from '../Filter';

describe('Filter', () => {
    it('renders correctly', () => {
        const onFilterChange = jest.fn();
        const onSubmit = jest.fn();
        const onAddNew = jest.fn();
        expect(render(<Filter onSubmit={onSubmit} onFilterChange={onFilterChange} onAddNew={onAddNew} />)).toMatchSnapshot();
    });

    it('handle Submit correctly', () => {
        const onFilterChange = jest.fn();
        const onSubmit = jest.fn();
        const onAddNew = jest.fn();
        render(<Filter onSubmit={onSubmit} onFilterChange={onFilterChange} onAddNew={onAddNew} />);

        fireEvent.click(screen.getByText('Submit'));
        expect(onFilterChange).toBeCalledTimes(1);
        expect(onSubmit).toBeCalledTimes(2);
    });

    it('handle add new correctly', () => {
        const onFilterChange = jest.fn();
        const onSubmit = jest.fn();
        const onAddNew = jest.fn();
        window.hasHostPermissionOnPeriod = true;
        render(<Filter onSubmit={onSubmit} onFilterChange={onFilterChange} onAddNew={onAddNew} />);

        fireEvent.click(screen.getByText('AddNew'));
        expect(onAddNew).toBeCalledTimes(1);
    });
});