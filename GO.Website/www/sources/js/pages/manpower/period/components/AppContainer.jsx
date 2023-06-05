import React, { Component } from 'react';
import Filter from './Filter';
import DataService from '@jsroot/common/dataService';
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
                />
            </div>
        );
    }
}

export default AppContainer;