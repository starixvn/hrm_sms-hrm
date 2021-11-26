import { createSlice } from '@reduxjs/toolkit';

const menuSlice = createSlice({
    name: 'menu',
    initialState: {
        params: null,
        menus: [],
        functions: [],
        sites: [],
        menuSelected: null,
        createModalDialog: { show: false },
        createMenuResult: null,
        updateMenuModalDialog: { show: false },
        updateMenuResult: null,
        updateStatusModalDialog: { show: false }
    },
    reducers: {
        setSiteParams: (state, action) => {
            state.params = action.payload;
        },
        setMenu: (state, action) => {
            state.menus = action.payload;
        },
        setFunctions: (state, action) => {
            state.functions = action.payload;
        },
        setAllSites: (state, action) => {
            state.sites = action.payload;
        },
        setMenuSelected: (state, action) => {
            state.menuSelected = action.payload;
        },
        setCreateModalDialog: (state, action) => {
            state.createModalDialog = action.payload;
        },
        setCreateMenuResult: (state, action) => {
            state.createMenuResult = action.payload;
        },
        setUpdateModalDialog: (state, action) => {
            state.updateMenuModalDialog = action.payload;
        },
        setUpdateMenuResult: (state, action) => {
            state.updateMenuResult = action.payload;
        },
        setUpdateStatusModalDialog: (state, action) => {
            state.updateStatusModalDialog = action.payload;
        }
    }
});

const { actions, reducer } = menuSlice;

export const {
    setSiteParams,
    setMenu,
    setFunctions,
    setAllSites,
    setMenuSelected,
    setCreateModalDialog,
    setCreateMenuResult,
    setUpdateModalDialog,
    setUpdateMenuResult,
    setUpdateStatusModalDialog
} = actions;

export default reducer;