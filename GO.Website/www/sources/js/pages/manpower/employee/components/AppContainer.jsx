import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import Filter from './Filter';
import DataGrid from './DataGrid';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import { DefaultPageSize } from '@jsroot/common/constants';

const AppContainer = () => {
    const [sites, setSites] = useState([]);
    const [filter, setFilter] = useState({
        keyword: '',
        siteId: null,
        statusId: 1,
        pageIndex: 1,
        pageSize: DefaultPageSize
    });
    const [gridData, setGridData] = useState(null);

    useEffect(() => {
        const getSites = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Manpower/Employee/GetSites');
            await DataService.getDataAsync(url).then(response => {
               setSites(response.data);
            }).then(() => Site.unblockUI());
        };

        getSites();
    }, []);

    const getEmployeeInfos = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Employee/GetEmployeeInfos');
        await DataService.postDataAsync(url, params).then(response => {
            setGridData(response.data);
        }).then(() => Site.unblockUI());
    };

    const handleChangeFilter = value => {
        setFilter(value);
    };

    const updateExclude = async (siteId, empId, isExcluded) => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Employee/UpdateExclude');
        const params = { siteId, empId, isExcluded };
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Update successfully!');
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    return (
        <div className="manpower-employee">
            <Filter sites={sites}
                onFilterChange={handleChangeFilter}
                getEmployeeInfos={getEmployeeInfos}
            />
            <DataGrid filter={filter}
                gridData={gridData}
                onFilterChange={handleChangeFilter}
                getEmployeeInfos={getEmployeeInfos}
                onChangeStatus={updateExclude}
            />
        </div>
    );
};

export default AppContainer;