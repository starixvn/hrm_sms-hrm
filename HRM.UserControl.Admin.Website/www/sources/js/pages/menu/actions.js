import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
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
} from './menuSlice';

const getMenuById = id => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`menu/getById/${id}`);
    await DataService.getDataAsync(url).then(response => {
        dispatch(setMenu(response.data));
    }).then(() => Site.unblockUI());
};

const getFunctionById = id => async () => {
    const url = Site.resolveClientUrl(`function/GetById/${id}`);
    return await DataService.getDataAsync(url).then(response => response);
};

const getParentById = id => async() => {
    const url = Site.resolveClientUrl(`menu/getById/${id}`);
    return await DataService.getDataAsync(url).then(response => response);
};

const getAllSites = () => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('menu/getAllSites/');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setAllSites(response.data));
    }).then(() => Site.unblockUI());
};

const createMenu = params => async dispatch => {
    const url = Site.resolveClientUrl('menu/create');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setCreateMenuResult(response.data));
    });
};

const updateMenu = params => async dispatch => {
    const url = Site.resolveClientUrl('menu/update');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setUpdateMenuResult(response.data));
    });
};


const updateMenuStatus = params => async() => {
    const url = Site.resolveClientUrl('menu/updateStatus');
    return await DataService.postDataAsync(url, params).then(response => response);
};

export const ActionCreators = {
    getAllSites,
    setAllSites,
    setSiteParams,
    getMenuById,
    setMenu,
    getParentById,
    getFunctionById,
    setFunctions,
    setMenuSelected,
    setCreateModalDialog,
    createMenu,
    setCreateMenuResult,
    setUpdateModalDialog,
    updateMenu,
    setUpdateMenuResult,
    updateMenuStatus,
    setUpdateStatusModalDialog
};