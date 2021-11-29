import { createSlice } from '@reduxjs/toolkit';

const roleSlice = createSlice({
    name: 'role',
    initialState: {
        getAllRoleParams: {
            siteId: 0,
            isActive: null
        },
        sites: [],
        roles: [],
        createModalDialog: { show: false },
        createRoleResult: null,
        updateModalDialog: { show: false },
        updateRoleResult: null,
        updateStatusModalDialog: { show: false },
        confirmModalDialog: { show: false }
    },
    reducers: {
        setGetAllRoleParams: (state, action) => {
            state.getAllRoleParams = action.payload;
        },
        setSites: (state, action) => {
            state.sites = action.payload;
        },
        setRoles: (state, action) => {
            state.roles = action.payload;
        },
        setCreateModalDialog: (state, action) => {
            state.createModalDialog = action.payload;
        },
        setCreateRoleResult: (state, action) => {
            state.createRoleResult = action.payload;
        },
        setUpdateModalDialog: (state, action) => {
            state.updateModalDialog = action.payload;
        },
        setUpdateRoleResult: (state, action) => {
            state.updateRoleResult = action.payload;
        },
        setUpdateStatusModalDialog: (state, action) => {
            state.updateStatusModalDialog = action.payload;
        },
        setConfirmModalDialog: (state, action) => {
            state.confirmModalDialog = action.payload;
        }
    }
});

const { actions, reducer } = roleSlice;

export const {
    setGetAllRoleParams,
    setSites,
    setRoles,
    setCreateModalDialog,
    setCreateRoleResult,
    setUpdateModalDialog,
    setUpdateRoleResult,
    setUpdateStatusModalDialog,
    setConfirmModalDialog
} = actions;

export default reducer;