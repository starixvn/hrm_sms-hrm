import React, { Component } from 'react';
import Filter from './Filter';
import DataService from '@jsroot/common/dataService';
import { toast } from 'react-toastify';
import DataGrid from './DataGrid';
import { DefaultPageSize } from '@jsroot/common/constants';
import Site from '@jsroot/common/site';
import Create from './Create';
import Update from './Update';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                IsActive: 1,
                pageIndex: 1,
                pageSize: DefaultPageSize
            },
            gridData: null,
            createModalDialog: { show: false },
            createResult: null,
            updateModalDialog: { show: false },
            updateResult: null
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
                this.getPeriodInfos(this.state.filter);
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    createPeriod = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/Create');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                this.setState({
                    createResult: null,
                    createModalDialog: { show: false }
                });
                toast.success('Create successfully!');
                this.getPeriodInfos(this.state.filter);
            } else {
                this.setState({
                    createResult: response.data
                });
            }
        }).then(() => Site.unblockUI());
    };

    updatePeriod = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/Update');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                this.setState({
                    updateResult: null,
                    updateModalDialog: { show: false }
                });
                toast.success('Update successfully!');
            } else {
                this.setState({
                    updateResult: response.data
                });
            }
        }).then(() => Site.unblockUI());
    };

    handleModalCreateDialog = isShow => {
        this.setState({
            createModalDialog: { show: isShow }
        });
    };

    handleModalUpdateDialog = isShow => {
        this.setState({
            updateModalDialog: { show: isShow }
        });
    };

    render() {
        const { filter, gridData, createModalDialog, updateModalDialog, createResult } = this.state;

        return (
            <div className="manpower-report">
                <Filter
                    onFilterChange={this.handleChangeFilter}
                    getPeriodInfos={this.getPeriodInfos}
                    handleModalCreateDialog={this.handleModalCreateDialog}
                />
                <DataGrid filter={filter}
                    gridData={gridData}
                    onFilterChange={this.handleChangeFilter}
                    getPeriodInfos={this.getPeriodInfos}
                    onChangeStatus={this.updateIsActive}
                    deletePeriod={this.deletePeriod}
                    handleModalUpdateDialog={this.handleModalUpdateDialog}
                />
                {
                    createModalDialog.show &&
                    <Create createPeriod={this.createPeriod}
                        handleModalCreateDialog={this.handleModalCreateDialog}
                        createResult={createResult}
                    />
                }
                {
                    updateModalDialog.show &&
                    <Update updatePeriod={this.updatePeriod}
                        handleModalUpdateDialog={this.handleModalUpdateDialog}
                    />
                }
            </div>
        );
    }
}

export default AppContainer;