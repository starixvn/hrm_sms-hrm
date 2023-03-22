import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setGetRoleParams,
    setSites,
    setRoles,
    setGetRolePermissionParams,
    setRolePermission,
    setCreateModalDialog,
    setCreateRoleResult,
    setUpdateModalDialog,
    setUpdateRoleResult,
    setUpdateStatusModalDialog,
    setConfirmModalDialog
} from './roleSlice';

const getAllSite = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('common/getAllSite');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setSites(response.data));
    }).then(() => Site.unblockUI());
};

const getRoleBySiteId = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('role/getBySiteId');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setRoles(response.data));
    }).then(() => Site.unblockUI());
};

const getRolePermission = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('role/getRolePermission');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setRolePermission(response.data));
    }).then(() => Site.unblockUI());
};

const createRole = params => async dispatch => {
    const url = Site.resolveClientUrl('role/create');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setCreateRoleResult(response.data));
    });
};

const updateRole = params => async dispatch => {
    const url = Site.resolveClientUrl('role/update');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setUpdateRoleResult(response.data));
    });
};

const updateRoleStatus = params => async () => {
    const url = Site.resolveClientUrl('role/updateStatus');
    return await DataService.postDataAsync(url, params).then(response => response);
};

const assignPermission = params => async () => {
    const url = Site.resolveClientUrl('role/assignPermission');
    return await DataService.postDataAsync(url, params).then(response => response);
};

export const ActionCreators = {
    setGetRoleParams,
    setRoles,
    setGetRolePermissionParams,
    setRolePermission,
    setCreateModalDialog,
    setCreateRoleResult,
    setUpdateModalDialog,
    setUpdateRoleResult,
    setUpdateStatusModalDialog,
    setConfirmModalDialog,
    getAllSite,
    getRoleBySiteId,
    getRolePermission,
    createRole,
    updateRole,
    updateRoleStatus,
    assignPermission
};