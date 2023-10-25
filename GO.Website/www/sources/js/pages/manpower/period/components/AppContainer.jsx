import React, { Component } from 'react';
import Filter from './Filter';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';

import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import DataGrid from './DataGrid';
import Create from './Create';
import Update from './Update';

import { DefaultPageSize } from '@jsroot/common/constants';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import LangHelper from '@jsroot/common/langHelper';

export default class AppContainer extends Component {
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
            updateModalDialog: {
                show: false,
                data: null
            },
            deleteModalDialog: {
                show: false,
                name: '',
                periodId: null
            }
        };
    }

    getPeriods = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/GetPeriods');
        await DataService.postDataAsync(url, params).then(response => {
            this.setState({
                gridData: response.data
            });
        }).then(() => Site.unblockUI());
    };

    handleChangeFilter = value => {
        this.setState({ filter: value });
    };

    handleCreatePeriod = () => {
        this.setState({
            createModalDialog: { show: true }
        });
    };

    handleCloseCreateModalDialog = () => {
        this.setState({
            createModalDialog: { show: false }
        });
    };

    createPeriod = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/Create');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success(LangHelper.getResource('CreateSuccessfully'));
                this.setState({
                    createModalDialog: { show: false }
                });
                this.getPeriods(this.state.filter);
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    handleUpdatePeriod = period => {
        this.setState({
            updateModalDialog: {
                show: true,
                data: period
            }
        });
    };

    handleCloseUpdateModalDialog = () => {
        this.setState({
            updateModalDialog: { show: false }
        });
    };

    updatePeriod = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/Update');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success(LangHelper.getResource('UpdateSuccessfully'));

                this.setState({
                    updateModalDialog: { show: false }
                });
                this.getPeriods(this.state.filter);
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    updateStatus = async (periodId, isActive) => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/UpdateStatus');
        const params = { periodId, isActive };
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Update successfully!');
                this.getPeriods(this.state.filter);
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    handleDeletePeriod = period => {
        this.setState({
            deleteModalDialog: {
                show: true,
                name: period.name,
                periodId: period.periodId
            }
        });
    };

    handleCloseDeleteModalDialog = () => {
        this.setState({
            deleteModalDialog: { show: false }
        });
    };

    deletePeriod = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/Delete');
        const params = { periodId: this.state.deleteModalDialog.periodId };
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Delete successfully!');
                this.getPeriods(this.state.filter);
                this.handleCloseDeleteModalDialog();
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    render() {
        const { filter, gridData, createModalDialog, updateModalDialog, deleteModalDialog } = this.state;
        return (
            <div className="manpower-period">
                <Filter
                    onFilterChange={this.handleChangeFilter}
                    onSubmit={this.getPeriods}
                    onAddNew={this.handleCreatePeriod}
                />
                <DataGrid filter={filter}
                    gridData={gridData}
                    onFilterChange={this.handleChangeFilter}
                    getPeriods={this.getPeriods}
                    onUpdate={this.handleUpdatePeriod}
                    onChangeStatus={this.updateStatus}
                    onDelete={this.handleDeletePeriod}
                />
                {
                    createModalDialog.show &&
                    <Create onSubmit={this.createPeriod}
                        onClose={this.handleCloseCreateModalDialog}
                    />
                }
                {
                    updateModalDialog.show &&
                    <Update onSubmit={this.updatePeriod}
                        onClose={this.handleCloseUpdateModalDialog}
                        period={updateModalDialog.data}
                    />
                }
                {
                    deleteModalDialog.show &&
                    <SweetModalDialog
                        title={LangHelper.getResource('Confirmation')}
                        contents={<div>Are you sure you want to delete period of <strong>{deleteModalDialog.name}</strong>?</div>}
                        buttons={[
                            <Button variant="primary" size="sm" key="ok"
                                onClick={this.deletePeriod}
                            >
                                {LangHelper.getResource('OK')}
                            </Button>,
                            <Button variant="secondary" size="sm" key="cancel"
                                onClick={this.handleCloseDeleteModalDialog}
                            >
                                {LangHelper.getResource('Cancel')}
                            </Button>
                        ]}
                        onClose={this.handleCloseDeleteModalDialog}
                    />
                }
            </div>
        );
    }
}