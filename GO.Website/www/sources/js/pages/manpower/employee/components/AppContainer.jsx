import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';

import Filter from './Filter';
import DataGrid from './DataGrid';

import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import { DefaultPageSize } from '@jsroot/common/constants';

const AppContainer = () => {
    const [companies, setCompanies] = useState([]);
    const [filter, setFilter] = useState({
        keyword: '',
        companyId: null,
        statusId: 1,
        pageIndex: 1,
        pageSize: DefaultPageSize
    });
    const [gridData, setGridData] = useState(null);
    const [sectionsTree, setSectionsTree] = useState(null);

    useEffect(() => {
        const getSites = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Manpower/Employee/GetCompanies');
            await DataService.getDataAsync(url).then(response => {
                setCompanies(response.data);
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

    const getSectionInfos = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Employee/GetSectionInfos');
        await DataService.getDataAsync(url, params).then(response => {
            setSectionsTree(response.data);
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
                getEmployeeInfos(filter);
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    const exportExcel = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Employee/ExportExcel');
        await DataService.getFileAsync(url, params).then(response => {
            const fileName = response.headers['content-disposition'].split('filename=')[1].split(';')[0];
            Site.downloadFile(response.data, fileName);
        }).then(() => Site.unblockUI());
    };

    return (
        <div className="manpower-employee">
            <Filter
                sectionsTree={sectionsTree}
                companies={companies}
                onFilterChange={handleChangeFilter}
                getEmployeeInfos={getEmployeeInfos}
                getSectionInfos={getSectionInfos}
                exportExcel={exportExcel}
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