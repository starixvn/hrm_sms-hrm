import React from 'react';
import { render, waitFor, renderHook, fireEvent } from '@testing-library/react';
import { screen } from '@testing-library/dom'
import '@testing-library/jest-dom';
import Update from '../Update';

describe('Update', () => {
    const period = {
        periodId: 1,
        name: 'test',
        settings: [
            { periodId: 1, processId: 1, fromDate: "2023-09-12T00:00:00", toDate: "2023-10-12T00:00:00" },
            { periodId: 1, processId: 2, fromDate: "2023-10-13T00:00:00", toDate: "2023-10-15T00:00:00" },
            { periodId: 1, processId: 3, fromDate: "2023-10-16T00:00:00", toDate: "2023-10-18T00:00:00" }
        ],
        year: 1
    }

    it('renders correctly', () => {
        expect(render(<Update period={period} />)).toMatchSnapshot();
    });

    it('handle update period correctly', () => {
        const onSubmit = jest.fn();
        render(<Update period={period} onSubmit={onSubmit} />);

        fireEvent.click(screen.getByText('Save'));
    });
});