import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import DataGrid from '../DataGrid';

import LangHelper from '@jsroot/common/langHelper';


describe('AppContainer', () => {
    beforeEach(() => {
        LangHelper.getResource = jest.fn(key => key);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('renders correctly', () => {
        expect(render(<DataGrid rolePermission={null} setRolePermission={jest.fn()} />)).toMatchSnapshot();
    });
});