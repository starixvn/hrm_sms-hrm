import React, { Component } from 'react';
import Filter from './Filter';
import { Button } from 'react-bootstrap';
import DataService from '@jsroot/common/dataService';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import LangHelper from '@jsroot/common/langHelper';
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
            updateModalDialog: {
                show: false,
                data: null
            },
            updateResult: null,
            deleteModalDialog: {
                show: false,
                name: '',
                periodId: null
            }
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

    getPeriodInfosById = async params => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/GetPeriodInfosById');
        await DataService.postDataAsync(url, params).then(response => {
            this.setState({
                updateModalDialog: {
                    show: true,
                    data: response.data
                }
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

    deletePeriod = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Period/Delete');
        const params = { periodId: this.state.deleteModalDialog.periodId };
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Delete successfully!');
                this.getPeriodInfos(this.state.filter);
                this.closeModalDeleteDialog();
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
                this.getPeriodInfos(this.state.filter);
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

    handleModalDeleteDialog = params => {
        this.setState({
            deleteModalDialog: {
                show: params.show,
                name: params.name,
                periodId: params.periodId
            }
        });
    };

    closeModalDeleteDialog = () => {
        this.setState({
            deleteModalDialog: {
                show: false,
                name: '',
                periodId: null
            }
        });
    };

    render() {
        const { filter, gridData, createModalDialog, updateModalDialog, createResult, deleteModalDialog } = this.state;
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
                    handleModalDeleteDialog={this.handleModalDeleteDialog}
                    getPeriodInfosById={this.getPeriodInfosById}
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
                        data={updateModalDialog.data}
                    />
                }
                {
                    deleteModalDialog.show &&
                    <SweetModalDialog
                        title={LangHelper.getResource('Confirmation')}
                        contents={<div>Are you sure you want to delete flow of <strong>{deleteModalDialog.name}</strong>?</div>}
                        buttons={[
                            <Button variant="primary" size="sm" key="ok"
                                onClick={this.deletePeriod}
                            >
                                {LangHelper.getResource('OK')}
                            </Button>,
                            <Button variant="secondary" size="sm" key="cancel"
                                onClick={this.closeModalDeleteDialog}
                            >
                                {LangHelper.getResource('Cancel')}
                            </Button>
                        ]}
                        onClose={this.closeModalDeleteDialog}
                    />
                }
            </div>
        );
    }
}

export default AppContainer;