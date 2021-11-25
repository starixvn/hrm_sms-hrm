import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setGetAllUserParams,
    setUserGridData,
    setSites,
    setUserSites,
    setSiteRoles,
    setAvailableSites,
    setUserPermission,
    setCreateModalDialog,
    setCreateUserResult,
    setResetPasswordModalDialog,
    setResetPasswordResult,
    setUpdateUserSiteStatusModalDialog,
    setAssignNewSiteModalDialog,
    setAssignNewSiteResult,
    setUpdateUserSiteRoleModalDialog,
    setUpdateUserSiteRoleResult
} from './userSlice';

const getAllSite = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('common/getAllSite');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setSites(response.data));
    }).then(() => Site.unblockUI());
};

const getAllUser = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('user/getAll');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setUserGridData(response.data));
    }).then(() => Site.unblockUI());
};

const createUser = params => async dispatch => {
    const url = Site.resolveClientUrl('user/create');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setCreateUserResult(response.data));
    });
};

const resetPassword = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('user/resetPassword');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setResetPasswordResult(response.data));
    }).then(() => Site.unblockUI());
};

const getSiteByUserId = userId => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`user/getSiteByUserId/${userId}`);
    await DataService.getDataAsync(url).then(response => {
        dispatch(setUserSites(response.data));
    }).then(() => Site.unblockUI());
};

const getAvailableSiteByUserId = userId => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`user/getAvailableSiteByUserId/${userId}`);
    await DataService.getDataAsync(url).then(response => {
        dispatch(setAvailableSites(response.data));
    }).then(() => Site.unblockUI());
};

const getRoleBySiteId = siteId => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`common/getRoleBySiteId?siteId=${siteId}`);
    await DataService.getDataAsync(url).then(response => {
        dispatch(setSiteRoles(response.data));
    }).then(() => Site.unblockUI());
};

const assignSiteToUser = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`user/assignSite`);
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setAssignNewSiteResult(response.data));
    }).then(() => Site.unblockUI());
};

const updateUserSiteRole = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`user/updateUserSiteRole`);
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setUpdateUserSiteRoleResult(response.data));
    }).then(() => Site.unblockUI());
};

const getUserPermission = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`user/getUserPermission`);
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setUserPermission(response.data));
    }).then(() => Site.unblockUI());
};

const updateUserSiteStatus = params => async () => {
    const url = Site.resolveClientUrl('user/updateUserSiteStatus');
    return await DataService.postDataAsync(url, params).then(response => response);
};

export const ActionCreators = {
    setGetAllUserParams,
    setUserGridData,
    setSites,
    setUserSites,
    setSiteRoles,
    setAvailableSites,
    setUserPermission,
    setCreateModalDialog,
    setCreateUserResult,
    setResetPasswordModalDialog,
    setResetPasswordResult,
    setUpdateUserSiteStatusModalDialog,
    setAssignNewSiteModalDialog,
    setAssignNewSiteResult,
    setUpdateUserSiteRoleModalDialog,
    setUpdateUserSiteRoleResult,
    getAllSite,
    getAllUser,
    createUser,
    resetPassword,
    getSiteByUserId,
    getAvailableSiteByUserId,
    getRoleBySiteId,
    assignSiteToUser,
    updateUserSiteRole,
    updateUserSiteStatus,
    getUserPermission
};