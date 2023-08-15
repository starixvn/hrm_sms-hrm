import React from 'react';
import { render, screen, renderHook } from '@testing-library/react';
import '@testing-library/jest-dom';
import DataGrid from '../DataGrid';

import LangHelper from '@jsroot/common/langHelper';


describe('AppContainer', () => {
    beforeEach(() => {
        LangHelper.getResource = jest.fn(key => key);
        window.hasHostPermission = true;
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    const rolePermission = {
        functions: [{
            functionId: 1,
            functionName: "test",
            functionKey: "test",
            siteId: 9,
            isActive: true,
            parentId: null
        },
        {
            functionId: 2,
            functionName: "test2",
            functionKey: "test2",
            siteId: 9,
            isActive: true,
            parentId: 1
        },
        {
            functionId: 3,
            functionName: "test3",
            functionKey: "test3",
            siteId: 9,
            isActive: true,
            parentId: 1
        }],
        permissions: [{
            functionId: 1,
            functionName: "test1",
            functionKey: "test1",
            isRead: true,
            isHost: false
        },
        {
            functionId: 2,
            functionName: "test2",
            functionKey: "test2",
            isRead: true,
            isHost: true
        },
        {
            functionId: 3,
            functionName: "test3",
            functionKey: "test3",
            isRead: true,
            isHost: true
        }],
        hierarchyFunctions: [{
            functionId: 1,
            functionName: "test1",
            functionKey: "test1",
            siteId: 9,
            isActive: true,
            parentId: null,
            level: 0,
            childs: [{
                functionId: 2,
                functionName: "test2",
                functionKey: "test2",
                siteId: 9,
                isActive: true,
                parentId: 1,
                level: 1
            }, {
                functionId: 3,
                functionName: "test3",
                functionKey: "test3",
                siteId: 9,
                isActive: true,
                parentId: 1,
                level: 1
            }]
        }]
    }

    it('renders correctly', () => {
        expect(render(<DataGrid rolePermission={null} setRolePermission={jest.fn()} />)).toMatchSnapshot();
    });

    it('render with role permission is null return null', () => {
        const { result } = renderHook(() => DataGrid({ rolePermission: null, setRolePermission: jest.fn() }));
        expect(result.current).toEqual(null);
    });

    it('render with role permission is not null return successfully', () => {
        // ACT
        render(<DataGrid rolePermission={rolePermission} setRolePermission={jest.fn()} />);
        const table = screen.getByTestId('table-role-permission');
        // ASSERT
        expect(table.children.length).toBe(1);
        expect(table.querySelectorAll('tr').length).toBe(7);
        expect(table.querySelectorAll('td').length).toBe(15);
        expect(table.querySelectorAll('th').length).toBe(4);
    });
});