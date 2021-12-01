import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setSiteParams,
    setFunctions,
    setAllSites,
    setCreateModalDialog,
    setCreateFunctionResult,
    setUpdateModalDialog,
    setUpdateFunctionResult,
    setUpdateStatusModalDialog,
    setConfirmRegenerateKeyModalDialog 
} from './functionSlice';

const getFunctionById = id => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`function/getBySiteId/${id}`);
    await DataService.getDataAsync(url).then(response => {
        dispatch(setFunctions(response.data));
    }).then(() => Site.unblockUI());
};

const getParentById = id => async() => {
    const url = Site.resolveClientUrl(`function/getParentBySiteId/${id}`);
    return await DataService.getDataAsync(url).then(response => response);
};

const getAllSites = () => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('common/getAllSite/');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setAllSites(response.data));
    }).then(() => Site.unblockUI());
};

const createFunction = params => async dispatch => {
    const url = Site.resolveClientUrl('function/create');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setCreateFunctionResult(response.data));
    });
};

const updateFunction = params => async dispatch => {
    const url = Site.resolveClientUrl('function/update');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setUpdateFunctionResult(response.data));
    });
};


const updateFunctionStatus = params => async() => {
    const url = Site.resolveClientUrl('function/updateStatus');
    return await DataService.postDataAsync(url, params).then(response => response);
};

export const ActionCreators = {
    setSiteParams,
    getFunctionById,
    setFunctions,
    getParentById,
    getAllSites,
    setAllSites,
    setCreateModalDialog,
    createFunction,
    setCreateFunctionResult,
    setUpdateModalDialog,
    updateFunction,
    setUpdateFunctionResult,
    updateFunctionStatus,
    setUpdateStatusModalDialog,
    setConfirmRegenerateKeyModalDialog
};