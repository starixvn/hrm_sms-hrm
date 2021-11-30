import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setGetAllSiteParams,
    setSites,
    setCreateModalDialog,
    setCreateSiteResult,
    setUpdateModalDialog,
    setUpdateSiteResult,
    setSelectedSite,
    setUpdateStatusModalDialog,
    setConfirmModalDialog
} from './siteSlice';

const getAllSite = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('common/getAllSite');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setSites(response.data));
    }).then(() => Site.unblockUI());
};

const getSiteById = siteId => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`site/getById/${siteId}`);
    await DataService.getDataAsync(url).then(response => {
        dispatch(setSelectedSite(response.data));
    }).then(() => Site.unblockUI());
};

const createSite = params => async dispatch => {
    const url = Site.resolveClientUrl('site/create');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setCreateSiteResult(response.data));
    });
};

const updateSite = params => async dispatch => {
    const url = Site.resolveClientUrl('site/update');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setUpdateSiteResult(response.data));
    });
};

const updateSiteStatus = params => async () => {
    const url = Site.resolveClientUrl('site/updateStatus');
    return await DataService.postDataAsync(url, params).then(response => response);
};

const generateToken = () => async () => {
    const url = Site.resolveClientUrl('site/generateToken');
    return await DataService.postDataAsync(url).then(response => response);
};

export const ActionCreators = {
    setGetAllSiteParams,
    setSites,
    setCreateModalDialog,
    setCreateSiteResult,
    setUpdateModalDialog,
    setUpdateSiteResult,
    setSelectedSite,
    setUpdateStatusModalDialog,
    setConfirmModalDialog,
    getAllSite,
    createSite,
    updateSite,
    getSiteById,
    updateSiteStatus,
    generateToken
};