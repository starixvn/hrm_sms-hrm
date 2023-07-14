import React, { useEffect, useState } from 'react';

import Filter from './Filter';
import DataGrid from './DataGrid';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';

const AppContainer = () => {
    const [periods, setPeriods] = useState([]);
    const [requests, setRequests] = useState([]);

    const getMemberRequests = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/GetMemberRequests');
        await DataService.postDataAsync(url, params).then(response => {
            setRequests(response.data);
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
        <div className="manpower-forecast my-annual-request">
            <Filter periods={periods} onSubmit={getMemberRequests} />
            <DataGrid requests={requests} />
        </div>
    );
};

export default AppContainer;