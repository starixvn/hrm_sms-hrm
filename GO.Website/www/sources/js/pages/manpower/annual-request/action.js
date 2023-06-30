import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import { toast } from 'react-toastify';

import {
    setUnits,
    setPeriod,
    setCompanies,
    setJobLevels,
    setUnitSelected,
    setAnnualRequestFormDetails,
    addAnnualRequestFormDetailsItem
} from './/annualRequestSlice';

import LangHelper from '@jsroot/common/langHelper';

const getDataForCreateAnnualRequest = unitName => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/GetDataForCreateAnnualRequest?UnitName=' + unitName);
    await DataService.postDataAsync(url, unitName).then(response => {
        dispatch(setPeriod(response.data.period));
        dispatch(setAnnualRequestFormDetails(response.data.details));
        dispatch(setCompanies(response.data.details));
        dispatch(setJobLevels(response.data.details));
        dispatch(setUnits(response.data.units));
    }).then(() => Site.unblockUI());
};

const createAnnualRequest = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/Create');
    await DataService.postDataAsync(url, params).then(response => {
        if (response.data.isSuccessful) {
            toast.success(LangHelper.getResource('CreateSuccessfully'));
        } else {
            toast.error(response.data.message);
        }
    }).then(() => Site.unblockUI());
};

export const ActionCreators = {
    setUnits,
    setPeriod,
    setCompanies,
    setJobLevels,
    setUnitSelected,
    setAnnualRequestFormDetails,
    addAnnualRequestFormDetailsItem,
    getDataForCreateAnnualRequest,
    createAnnualRequest
};