import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setUnits,
    setSelectedUnit,
    setFlows,
    addFlowItem,
    removeFlowItem,
    setShowDeleteModalDialog
} from './flowSlice';

const getUnits = () => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('Manpower/Flow/GetUnits');
    await DataService.getDataAsync(url).then(response => {
        dispatch(setUnits(response.data));
    }).then(() => Site.unblockUI());
};

const getFlowByUnit = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('Manpower/Flow/GetByUnit');
    await DataService.getDataAsync(url, params).then(response => {
        dispatch(setFlows(response.data));
    }).then(() => Site.unblockUI());
};

const updateFlow = params => async () => {
    const url = Site.resolveClientUrl('Manpower/Flow/Update');
    return await DataService.postDataAsync(url, params).then(response => response);
};

const deleteFlow = params => async () => {
    const url = Site.resolveClientUrl('Manpower/Flow/Delete');
    return await DataService.postDataAsync(url, params).then(response => response);
};

export const ActionCreators = {
    getUnits,
    getFlowByUnit,
    setUnits,
    setSelectedUnit,
    setFlows,
    addFlowItem,
    removeFlowItem,
    setShowDeleteModalDialog,
    updateFlow,
    deleteFlow
};