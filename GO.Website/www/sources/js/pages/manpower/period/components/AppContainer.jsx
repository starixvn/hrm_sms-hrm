import React, { Component } from 'react';
import Filter from './Filter';
import DataService from '@jsroot/common/dataService';
import { toast } from 'react-toastify';
import DataGrid from './DataGrid';
import { DefaultPageSize } from '@jsroot/common/constants';
import Site from '@jsroot/common/site';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                IsActive: 1,
                pageIndex: 1,
                pageSize: DefaultPageSize
            },
            gridData: null
        };
    }

    getPeriodInfos = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/GetPeriodInfos');
        await DataService.postDataAsync(url, params).then(response => {
            this.setState({
                gridData: response.data
            });
        }).then(() => Site.unblockUI());
    };

    handleChangeFilter = value => {
        this.setState({ filter: value });
    };

    updateIsActive = async (periodId, isActive) => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/UpdateIsActive');
        const params = { periodId, isActive};
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Update successfully!');
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    deletePeriod = async periodId => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/Delete');
        const params = { periodId };
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Delete successfully!');
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    render() {
        const { filter, gridData } = this.state;

        return (
            <div className="manpower-report">
                <Filter
                    onFilterChange={this.handleChangeFilter}
                    getPeriodInfos={this.getPeriodInfos}
                />
                <DataGrid filter={filter}
                    gridData={gridData}
                    onFilterChange={this.handleChangeFilter}
                    getPeriodInfos={this.getPeriodInfos}
                    onChangeStatus={this.updateIsActive}
                    deletePeriod={this.deletePeriod}
                />
            </div>
        );
    }
}

export default AppContainer;