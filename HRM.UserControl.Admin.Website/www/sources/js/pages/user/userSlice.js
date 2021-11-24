import { createSlice } from '@reduxjs/toolkit';
import { DefaultPageSize } from '@jsroot/common/constants';

const userSlice = createSlice({
    name: 'user',
    initialState: {
        getAllUserParams: {
            pageIndex: 1,
            pageSize: DefaultPageSize
        },
        userGridData: null,
        userSites: [],
        availableSites: [],
        siteRoles: [],
        createModalDialog: { show: false },
        createUserResult: null,
        resetPasswordModalDialog: { show: false },
        resetPasswordResult: null,
        updateUserSiteStatusModalDialog: { show: false },
        assignNewSiteModalDialog: { show: false },
        assignNewSiteResult: null,
        updateUserSiteRoleModalDialog: { show: false },
        updateUserSiteRoleResult: null
    },
    reducers: {
        setGetAllUserParams: (state, action) => {
            state.getAllUserParams = action.payload;
        },
        setUserGridData: (state, action) => {
            state.userGridData = action.payload;
        },
        setUserSites: (state, action) => {
            state.userSites = action.payload;
        },
        setSiteRoles: (state, action) => {
            state.siteRoles = action.payload;
        },
        setAvailableSites: (state, action) => {
            state.availableSites = action.payload;
        },
        setCreateModalDialog: (state, action) => {
            state.createModalDialog = action.payload;
        },
        setCreateUserResult: (state, action) => {
            state.createUserResult = action.payload;
        },
        setResetPasswordModalDialog: (state, action) => {
            state.resetPasswordModalDialog = action.payload;
        },
        setResetPasswordResult: (state, action) => {
            state.resetPasswordResult = action.payload;
        },
        setUpdateUserSiteStatusModalDialog: (state, action) => {
            state.updateUserSiteStatusModalDialog = action.payload;
        },
        setAssignNewSiteModalDialog: (state, action) => {
            state.assignNewSiteModalDialog = action.payload;
        },
        setAssignNewSiteResult: (state, action) => {
            state.assignNewSiteResult = action.payload;
        },
        setUpdateUserSiteRoleModalDialog: (state, action) => {
            state.updateUserSiteRoleModalDialog = action.payload;
        },
        setUpdateUserSiteRoleResult: (state, action) => {
            state.updateUserSiteRoleResult = action.payload;
        }
    }
});

const { actions, reducer } = userSlice;

export const {
    setGetAllUserParams,
    setUserGridData,
    setUserSites,
    setSiteRoles,
    setAvailableSites,
    setCreateModalDialog,
    setCreateUserResult,
    setResetPasswordModalDialog,
    setResetPasswordResult,
    setUpdateUserSiteStatusModalDialog,
    setAssignNewSiteModalDialog,
    setAssignNewSiteResult,
    setUpdateUserSiteRoleModalDialog,
    setUpdateUserSiteRoleResult
} = actions;

export default reducer;