import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { toast } from 'react-toastify';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import { Button } from 'react-bootstrap';
import Filter from './Filter';
import DataGrid from './DataGrid';
import CreateRole from './Create';
import UpdateRole from './Update';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.getAllSite();
    }

    updateStatus = () => {
        Site.blockUI();
        const { siteId, roleId, isActive } = this.props.updateStatusModalDialog.role;
        this.props.updateRoleStatus({ siteId, roleId, isActive: !isActive }).then(response => {
            Site.unblockUI();
            this.closeConfirmUpdateStatus();
            if (response.data.isSuccessful) {
                const message = isActive ? 'DeactivatedSuccessfully' : 'ActivatedSuccessfully';
                toast.success(LangHelper.getResource(message));
                this.props.getRoleBySiteId(this.props.getRoleParams);
            } else {
                toast.error(response.data.message);
            }
        });
    };

    closeConfirmUpdateStatus = () => {
        this.props.setUpdateStatusModalDialog({ show: false });
    };

    render() {
        const { createModalDialog, updateModalDialog, updateStatusModalDialog } = this.props;
        return (
            <>
                <Filter />
                <DataGrid />
                {
                    createModalDialog.show &&
                    <CreateRole />
                }
                {
                    updateModalDialog.show &&
                    <UpdateRole />
                }
                {
                    updateStatusModalDialog.show &&
                    <SweetModalDialog
                        size='lg'
                        title={LangHelper.getResource('Confirmation')}
                        contents={
                            <div>
                                Are you sure you want to {updateStatusModalDialog.role.isActive ? 'deactivate ' : 'active '}
                                <strong>{updateStatusModalDialog.role.name}</strong>?
                            </div>
                        }
                        buttons={[
                            <Button variant="primary" size="sm" onClick={this.updateStatus}>
                                {LangHelper.getResource('OK')}
                            </Button>,
                            <Button variant="secondary" size="sm" onClick={this.closeConfirmUpdateStatus}>
                                {LangHelper.getResource('Cancel')}
                            </Button>
                        ]}
                        onClose={this.closeConfirmUpdateStatus}
                    />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getRoleParams, createModalDialog, updateModalDialog, updateStatusModalDialog } = state;
    return { getRoleParams, createModalDialog, updateModalDialog, updateStatusModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);