import { createSlice } from '@reduxjs/toolkit';

const siteSlice = createSlice({
    name: 'site',
    initialState: {
        getAllSiteParams: {
            isActive: null
        },
        sites: [],
        createModalDialog: { show: false },
        createSiteResult: null,
        updateModalDialog: { show: false },
        updateSiteResult: null,
        updateStatusModalDialog: { show: false },
        confirmModalDialog: { show: false }
    },
    reducers: {
        setGetAllSiteParams: (state, action) => {
            state.getAllSiteParams = action.payload;
        },
        setSites: (state, action) => {
            state.sites = action.payload;
        },
        setCreateModalDialog: (state, action) => {
            state.createModalDialog = action.payload;
        },
        setCreateSiteResult: (state, action) => {
            state.createSiteResult = action.payload;
        },
        setUpdateModalDialog: (state, action) => {
            state.updateModalDialog = action.payload;
        },
        setUpdateSiteResult: (state, action) => {
            state.updateSiteResult = action.payload;
        },
        setUpdateStatusModalDialog: (state, action) => {
            state.updateStatusModalDialog = action.payload;
        },
        setConfirmModalDialog: (state, action) => {
            state.confirmModalDialog = action.payload;
        }
    }
});

const { actions, reducer } = siteSlice;

export const {
    setGetAllSiteParams,
    setSites,
    setCreateModalDialog,
    setCreateSiteResult,
    setUpdateModalDialog,
    setUpdateSiteResult,
    setUpdateStatusModalDialog,
    setConfirmModalDialog
} = actions;

export default reducer;