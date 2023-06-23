import React, { useEffect, useState } from 'react';

import DataGrid from './DataGrid';

import DataService from '@jsroot/common/dataService';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

const AppContainer = () => {
    const [requests, setRequests] = useState([]);

    useEffect(() => {
        const getRequests = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Manpower/AnnualRequest/GetMyRequest');
            await DataService.postDataAsync(url).then(response => {
                setRequests(response.data);
            }).then(() => Site.unblockUI());
        };

        getRequests();
    }, []);

    return (
        <div className="manpower-forecast my-annual-request">
            <DataGrid requests={requests} />
        </div>
    );
};

export default AppContainer;