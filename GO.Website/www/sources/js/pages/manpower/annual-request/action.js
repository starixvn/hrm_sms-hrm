import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import { toast } from 'react-toastify';

import {
    setRequest,
    setDepartments,
    setPeriod,
    setCompanies,
    setJobLevels,
    setUnitSelected,
    setRequestDetails,
    addRequestDetailsItem,
    setSubmitModalDialog,
    setVerifyModalDialog,
    setApproveModalDialog,
    setResetModalDialog
} from './annualRequestSlice';

import LangHelper from '@jsroot/common/langHelper';

const getDataForCreateAnnualRequest = unitName => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/GetDataForCreateAnnualRequest?UnitName=' + unitName);
    await DataService.postDataAsync(url, unitName).then(response => {
        dispatch(setPeriod(response.data.period));
        dispatch(setRequestDetails(response.data.details));
        dispatch(setCompanies(response.data.details));
        dispatch(setJobLevels(response.data.details));
        dispatch(setDepartments(response.data.details));
    }).then(() => Site.unblockUI());
};

const createAnnualRequest = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/Create');
    await DataService.postDataAsync(url, params).then(response => {
        if (response.data.isSuccessful) {
            toast.success(params.MethodName + LangHelper.getResource('Successfully'));
        } else {
            toast.error(response.data.message);
        }
    }).then(() => Site.unblockUI());
};

const getAnnualRequestById = requestId => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl(`ManpowerForecast/AnnualRequest/GetById/${requestId}`);
    await DataService.postDataAsync(url, requestId).then(response => {
        const { request } = response.data;

        dispatch(setRequest(request));
        dispatch(setPeriod(request.period));
        dispatch(setRequestDetails(request.details));
        dispatch(setCompanies(request.details));
        dispatch(setJobLevels(request.details));
        dispatch(setDepartments(request.details));
        dispatch(setUnitSelected({
            value: request.details[0].unitName,
            label: request.details[0].unitName
        }));
    }).then(() => Site.unblockUI());
};

const updateStatus = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/UpdateStatus');
    await DataService.postDataAsync(url, params).then(response => {
        if (response.data.isSuccessful) {
            toast.success(params.MethodName + LangHelper.getResource('Successfully'));
        } else {
            toast.error(response.data.message);
        }
    }).then(() => Site.unblockUI());
};

const updateAnnualRequest = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/Update');
    await DataService.postDataAsync(url, params).then(response => {
        if (response.data.isSuccessful) {
            toast.success(params.MethodName + LangHelper.getResource('Successfully'));
        } else {
            toast.error(response.data.message);
        }
    }).then(() => Site.unblockUI());
};

export const ActionCreators = {
    setRequest,
    setDepartments,
    setPeriod,
    setCompanies,
    setJobLevels,
    setUnitSelected,
    setRequestDetails,
    addRequestDetailsItem,
    setSubmitModalDialog,
    setVerifyModalDialog,
    setApproveModalDialog,
    setResetModalDialog,
    getDataForCreateAnnualRequest,
    createAnnualRequest,
    getAnnualRequestById,
    updateStatus,
    updateAnnualRequest
};