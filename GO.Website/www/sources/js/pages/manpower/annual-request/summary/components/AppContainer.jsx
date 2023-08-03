import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

import Filter from './Filter';
import DataGrid from './DataGrid';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = () => {
    const [periodSelected, setPeriodSelected] = useState(null);
    const [periods, setPeriods] = useState([]);
    const [requests, setRequests] = useState([]);
    const [companies, setCompanies] = useState([]);
    const [unitTypes, setUnitTypes] = useState([]);

    const getCompanies = requests => {
        const company = [];
        requests.map(request => {
            request.details.map(detail => {
                if (company.indexOf(detail.companyName) === -1) {
                    company.push(detail.companyName);
                }
            });
        });
        setCompanies(company);
    };

    const getUnitTypes = requests => {
        const unitTypes = [];
        requests.map(request => {
            if (unitTypes.findIndex(x => x.slice(0, 2) === request.unitName.slice(0, 2)) === -1) {
                unitTypes.push(request.unitName.slice(0, 2));
            }
        });
        setUnitTypes(unitTypes);
    };

    const getMemberRequests = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/GetMemberRequestSummary');
        setPeriodSelected(periods.find(x => x.periodId === params.periodId));
        await DataService.postDataAsync(url, params).then(response => {
            setRequests(response.data);
            getCompanies(response.data);
            getUnitTypes(response.data);
        }).then(() => Site.unblockUI());
    };

    const updateStatus = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/UpdateStatusMultiRequest');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success(params.methodName + LangHelper.getResource('Successfully'));
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    useEffect(() => {
        const getPeriods = async () => {
            const url = Site.resolveClientUrl('Common/GetManpowerPeriods');
            await DataService.postDataAsync(url).then(response => {
                setPeriods(response.data);
            });
        };
        getPeriods();
    }, []);

    return (
        <div className="manpower-forecast">
            <Filter periods={periods}
                onSubmit={getMemberRequests}
                requests={requests}
                updateStatus={updateStatus}
            />
            <DataGrid requests={requests}
                companies={companies}
                unitTypes={unitTypes}
                periodSelected={periodSelected}
            />
        </div>
    );
};

export default AppContainer;