import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setRoles,
    setUserRoleFilterParams,
    setUserRoleGridData,
    setUpdateUserRoleModalDialog
} from './roleAssignmentSlice';

const getRoles = () => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('Common/GetRoles');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setRoles(response.data));
    }).then(() => Site.unblockUI());
};

const getUserRoles = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('Admin/RoleAssignment/GetUserRoles');
    await DataService.postDataAsync(url, params).then(response => {
        dispatch(setUserRoleGridData(response.data));
    }).then(() => Site.unblockUI());
};

const assignRole = params => async dispatch => {
    const url = Site.resolveClientUrl(`Admin/RoleAssignment/AssignRole`);
    return await DataService.postDataAsync(url, params).then(response => response);
};

export const ActionCreators = {
    setRoles,
    setUserRoleFilterParams,
    setUserRoleGridData,
    setUpdateUserRoleModalDialog,
    getRoles,
    getUserRoles,
    assignRole
};