import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import { toast } from 'react-toastify';

import {
    setUnits,
    setPeriod,
    setCompanies,
    setAnnualRequestFormDetails,
    addAnnualRequestFormDetailsItem
} from './/annualRequestSlice';

const getDataForCreateAnnualRequest = () => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('Manpower/AnnualRequest/GetDataForCreateAnnualRequest');
    await DataService.postDataAsync(url).then(response => {
        dispatch(setPeriod(response.data.period));
        dispatch(setAnnualRequestFormDetails(response.data.annualRequestFormDetails));
        dispatch(setCompanies(response.data.companies));
        dispatch(setUnits(response.data.units));
    }).then(() => Site.unblockUI());
};

const createAnnualRequest = params => async dispatch => {
    Site.blockUI();
    const url = Site.resolveClientUrl('Manpower/AnnualRequest/Create');
    await DataService.postDataAsync(url, params).then(response => {
        if (response.data.isSuccessful) {
            toast.success('Update successfully!');
        } else {
            toast.error(response.data.message);
        }
    }).then(() => Site.unblockUI());
};

export const ActionCreators = {
    setUnits,
    setPeriod,
    setCompanies,
    setAnnualRequestFormDetails,
    addAnnualRequestFormDetailsItem,
    getDataForCreateAnnualRequest,
    createAnnualRequest
};