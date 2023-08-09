import React from 'react';
import { render, fireEvent, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import Filter from '../Filter';

import LangHelper from '@jsroot/common/langHelper';

describe('Filter', () => {
    beforeAll(() => {
        LangHelper.getResource = jest.fn(key => key);
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    it('renders correctly', () => {
        expect(render(<Filter roles={[]} getRolePermission={jest.fn()} roleSelected={{ value: 0, label: null }} setRoleSelected={jest.fn()}/>)).toMatchSnapshot();
    });
});
