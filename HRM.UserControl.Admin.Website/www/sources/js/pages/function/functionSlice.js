import { createSlice } from '@reduxjs/toolkit';

const functionSlice = createSlice({
    name: 'function',
    initialState: {
        functions: [],
        sites: [],
        params: null,
        createModalDialog: { show: false },
        createFunctionResult: null,
        updateModalDialog: { show: false },
        updateFunctionResult: null,
        updateStatusModalDialog: { show: false },
        confirmModalRegenerateKeyDialog: { show: false }
    },
    reducers: {
        setSiteParams: (state, action) => {
            state.params = action.payload;
        },
        setFunctions: (state, action) => {
            state.functions = action.payload;
        },
        setAllSites: (state, action) => {
            state.sites = action.payload;
        },
        setCreateModalDialog: (state, action) => {
            state.createModalDialog = action.payload;
        },
        setCreateFunctionResult: (state, action) => {
            state.createFunctionResult = action.payload;
        },
        setUpdateModalDialog: (state, action) => {
            state.updateModalDialog = action.payload;
        },
        setUpdateFunctionResult: (state, action) => {
            state.updateFunctionResult = action.payload;
        },
        setUpdateStatusModalDialog: (state, action) => {
            state.updateStatusModalDialog = action.payload;
        },
        setConfirmRegenerateKeyModalDialog: (state, action) => {
            state.confirmModalRegenerateKeyDialog = action.payload;
        }
    }
});

const { actions, reducer } = functionSlice;

export const {
    setSiteParams,
    setFunctions,
    setAllSites,
    setCreateModalDialog,
    setCreateFunctionResult,
    setUpdateModalDialog,
    setUpdateFunctionResult,
    setUpdateStatusModalDialog,
    setConfirmRegenerateKeyModalDialog
} = actions;

export default reducer;