import LangHelper from '@jsroot/common/langHelper';
import { AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';

export const getAnnualRequestStatus = statusId => {
    let status = '';
    switch (statusId) {
        case AnnualManpowerForecastRequestStatus.Pending:
            status = LangHelper.getResource('Pending');
            break;

        case AnnualManpowerForecastRequestStatus.Verified:
            status = LangHelper.getResource('Verified');
            break;

        case AnnualManpowerForecastRequestStatus.Approved:
            status = LangHelper.getResource('Approved');
            break;
    }

    return status;
};

export const renderNumberOfHeadcount = number => number !== 0 ? number : '';