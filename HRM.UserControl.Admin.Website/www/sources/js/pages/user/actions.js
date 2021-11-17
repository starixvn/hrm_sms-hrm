import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setGetAllUserParams,
    setUserGridData,
    setCreateModalDialog,
    setCreateUserResult,
    setResetPasswordModalDialog,
    setResetPasswordResult
} from './userSlice';

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

export const ActionCreators = {
    setGetAllUserParams,
    setUserGridData,
    setCreateModalDialog,
    setCreateUserResult,
    setResetPasswordModalDialog,
    setResetPasswordResult,
    getAllUser,
    createUser,
    resetPassword
};