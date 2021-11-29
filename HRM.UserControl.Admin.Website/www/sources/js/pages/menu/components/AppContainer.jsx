import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { toast } from 'react-toastify';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import { Button } from 'react-bootstrap';
import Filter from './Filter';
import DataGrid from './DataGrid';
import Create from './Create';
import Update from './Update';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.getAllSites();
    }

    updateStatus = () => {
        Site.blockUI();
        const { menuItemId, isActive } = this.props.menuSelected;
        this.props.updateMenuStatus({ menuItemId, isActive: !isActive }).then(response => {
            Site.unblockUI();
            this.closeConfirmUpdateStatus();
            if (response.data.isSuccessful) {
                const message = isActive ? 'DeactivatedSuccessfully' : 'ActivatedSuccessfully';
                toast.success(LangHelper.getResource(message));
                this.props.getMenuById(this.props.params);
            } else {
                toast.error(response.data.message);
            }
        });
    };

    closeConfirmUpdateStatus = () => {
        this.props.setUpdateStatusModalDialog({ show: false });
        this.props.setMenuSelected(null);
    };

    render() {
        const { sites, menuSelected, createModalDialog, updateStatusModalDialog, updateMenuModalDialog  } = this.props;
        if(sites.length === 0){
            return null;
        }

        return (
            <div>
                <Filter />
                <DataGrid />
                {
                    createModalDialog.show &&
                    <Create />
                }
                {
                    updateMenuModalDialog.show && menuSelected &&
                    <Update />
                }
                {
                    updateStatusModalDialog.show &&
                    <SweetModalDialog
                        size='lg'
                        title={LangHelper.getResource('Confirmation')}
                        contents={
                            <div>
                                Are you sure you want to {menuSelected.isActive ? 'deactivate ' : 'active '}
                                <strong>{menuSelected.menuItemName}</strong>?
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
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { sites, menuSelected, params, createModalDialog, updateMenuModalDialog, updateStatusModalDialog} = state;
    return { sites, menuSelected, params, createModalDialog, updateMenuModalDialog, updateStatusModalDialog};
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);
