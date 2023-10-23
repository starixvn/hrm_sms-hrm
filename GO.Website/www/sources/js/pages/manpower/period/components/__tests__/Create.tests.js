import React from 'react';
import { render, waitFor, renderHook, fireEvent } from '@testing-library/react';
import { screen } from '@testing-library/dom'
import '@testing-library/jest-dom';
import Create from '../Create';

describe('Create', () => {
    it('renders correctly', () => {
        expect(render(<Create />)).toMatchSnapshot();
    });

    it('handle submit correctly', () => {
        const onSubmit = jest.fn();
        const onClose = jest.fn();
        render(<Create onSubmit={onSubmit} onClose={onClose}/>);

        fireEvent.click(screen.getByText('Save'));
    });
});