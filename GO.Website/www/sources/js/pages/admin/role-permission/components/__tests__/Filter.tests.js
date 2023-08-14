import React from 'react';
import { render, fireEvent, screen, renderHook, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event'
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

    test('renders correctly', () => {
        expect(render(<Filter roles={[]} getRolePermission={jest.fn()} roleSelected={{ value: 0, label: null }} setRoleSelected={jest.fn()}/>)).toMatchSnapshot();
    });

    test('handle change role renders correctly', async () => {
        const roles = [{ roleId: 1, name: 'test1' }, { roleId: 2, name: 'test2' }];
        const getRolePermission = jest.fn;
        const roleSelected = { value: 1, label: 'test1' };
        const setRoleSelected = jest.fn;
        const { result } = renderHook(() => Filter({ roles, getRolePermission, roleSelected, setRoleSelected}));
        await waitFor(() => {
            const filter = result.current.props.children.props.children.props.children[1].props;
            filter.onChange({ value: 2, label: 'test2' });
        });
    });
});
