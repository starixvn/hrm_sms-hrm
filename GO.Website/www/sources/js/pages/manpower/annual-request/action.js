import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import {
    setUnits,
    setPeriod,
    setCompanies,
    setAnnualRequestFormDetails
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

export const ActionCreators = {
    setUnits,
    setPeriod,
    setCompanies,
    setAnnualRequestFormDetails,
    getDataForCreateAnnualRequest
};