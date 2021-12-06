import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button, Form } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { ActionCreators } from '../../actions';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import DataGrid from './DataGrid';
import AssignNewSite from './AssignNewSite';
import UpdateUserSiteRole from './UpdateUserSiteRole';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.getSiteByUserId(window.selectedUserId);
    }

    handleOpenAssignNewSiteModalDialog = () => {
        this.props.setAssignNewSiteModalDialog({ show: true });
    };

    updateUserSiteStatus = () => {
        Site.blockUI();
        const { siteId, isActive } = this.props.updateUserSiteStatusModalDialog.site;
        this.props.updateUserSiteStatus({ userId: window.selectedUserId, siteId, isActive: !isActive }).then(response => {
            Site.unblockUI()
            this.closeConfirmUpdateUserSiteStatus();
            if (response.data.isSuccessful) {
                const message = isActive ? 'DeactivatedSuccessfully' : 'ActivatedSuccessfully';
                toast.success(LangHelper.getResource(message));
                this.props.getSiteByUserId(window.selectedUserId);
            } else {
                toast.error(response.data.message);
            }
        });
    };

    closeConfirmUpdateUserSiteStatus = () => {
        this.props.setUpdateUserSiteStatusModalDialog({ show: false });
    };

    render() {
        const { assignNewSiteModalDialog, updateUserSiteRoleModalDialog, updateUserSiteStatusModalDialog } = this.props;
        return (
            <>
                {
                    window.hasHostPermissionOnUser &&
                    <Form className="form-filter">
                        <Form.Row bsPrefix="form-row">
                            <Form.Group>
                                <Button variant="primary" size="sm" onClick={this.handleOpenAssignNewSiteModalDialog}>
                                    {LangHelper.getResource('AssignNewSite')}
                                </Button>
                            </Form.Group>
                        </Form.Row>
                    </Form>
                }
                <DataGrid />
                {
                    assignNewSiteModalDialog.show &&
                    <AssignNewSite />
                }
                {
                    updateUserSiteRoleModalDialog.show &&
                    <UpdateUserSiteRole />
                }
                {
                    updateUserSiteStatusModalDialog.show &&
                    <SweetModalDialog
                        size='lg'
                        title={LangHelper.getResource('Confirmation')}
                        contents={
                            <div>
                                Are you sure you want to {updateUserSiteStatusModalDialog.site.isActive ? 'deactivate ' : 'active '}
                                <strong>{updateUserSiteStatusModalDialog.site.siteName}</strong>?
                            </div>
                        }
                        buttons={[
                            <Button variant="primary" size="sm" onClick={this.updateUserSiteStatus}>
                                {LangHelper.getResource('OK')}
                            </Button>,
                            <Button variant="secondary" size="sm" onClick={this.closeConfirmUpdateUserSiteStatus}>
                                {LangHelper.getResource('Cancel')}
                            </Button>
                        ]}
                        onClose={this.closeConfirmUpdateUserSiteStatus}
                    />
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { assignNewSiteModalDialog, updateUserSiteRoleModalDialog, updateUserSiteStatusModalDialog } = state;
    return { assignNewSiteModalDialog, updateUserSiteRoleModalDialog, updateUserSiteStatusModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);