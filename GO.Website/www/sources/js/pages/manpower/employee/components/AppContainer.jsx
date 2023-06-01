import React, { Component } from 'react';
import { toast } from 'react-toastify';
import Filter from './Filter';
import DataGrid from './DataGrid';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import LangHelper from '@jsroot/common/langHelper';
import { DefaultPageSize } from '@jsroot/common/constants';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            sites: [],
            filter: {
                keyword: '',
                siteId: null,
                statusId: 1,
                pageIndex: 1,
                pageSize: DefaultPageSize
            },
            gridData: null
        };
    }

    async componentDidMount() {
        await this.getSites();
    }

    getSites = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Employee/GetSites');
        await DataService.getDataAsync(url).then(response => {
            this.setState({
                sites: response.data
            });
        }).then(() => Site.unblockUI());
    };

    getEmployeeInfos = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Employee/GetEmployeeInfos');
        await DataService.postDataAsync(url, params).then(response => {
            this.setState({
                gridData: response.data
            });
        }).then(() => Site.unblockUI());
    };

    handleChangeFilter = value => {
        this.setState({ filter: value });
    };

    updateExclude = async (siteId, empId, isExcluded) => {
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

    render() {
        const { filter, gridData, sites } = this.state;

        return (
            <div className="manpower-report">
                
                <Filter sites={sites}
                    onFilterChange={this.handleChangeFilter}
                    getEmployeeInfos={this.getEmployeeInfos}
                />
                <DataGrid filter={filter}
                    gridData={gridData}
                    onFilterChange={this.handleChangeFilter}
                    getEmployeeInfos={this.getEmployeeInfos}
                    onChangeStatus={this.updateExclude}
                />
            </div>
        );
    }
}

export default AppContainer;