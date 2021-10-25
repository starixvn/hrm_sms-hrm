import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setGetAllUserParams,
    setUserGridData,
    setCreateModalDialog,
    setCreateUserResult
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

export const ActionCreators = {
    setGetAllUserParams,
    setUserGridData,
    setCreateModalDialog,
    setCreateUserResult,
    getAllUser,
    createUser
};