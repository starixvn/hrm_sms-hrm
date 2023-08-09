import { createSlice } from '@reduxjs/toolkit';
import { DefaultPageSize } from '@jsroot/common/constants';

const roleAssignmentSlice = createSlice({
    name: 'roleAssignmentSlice',
    initialState: {
        roles: [],
        userRoleFilterParams: {
            keyword: '',
            statusId: null,
            pageIndex: 1,
            pageSize: DefaultPageSize
        },
        userRoleGridData: null,
        updateUserRoleModalDialog: { show: false }
    },
    reducers: {
        setRoles: (state, action) => {
            state.roles = action.payload;
        },
        setUserRoleFilterParams: (state, action) => {
            state.userRoleFilterParams = action.payload;
        },
        setUserRoleGridData: (state, action) => {
            state.userRoleGridData = action.payload;
        },
        setUpdateUserRoleModalDialog: (state, action) => {
            state.updateUserRoleModalDialog = action.payload;
        }
    }
});

const { actions, reducer } = roleAssignmentSlice;

export const {
    setRoles,
    setUserRoleFilterParams,
    setUserRoleGridData,
    setUpdateUserRoleModalDialog
} = actions;

export default reducer;