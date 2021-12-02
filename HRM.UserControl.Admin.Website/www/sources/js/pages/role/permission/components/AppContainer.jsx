import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { ActionCreators } from '../../actions';
import Filter from './Filter';
import DataGrid from './DataGrid';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

export class AppContainer extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.getAllSite();
    }

    handleSubmit = () => {
        Site.blockUI();
        const { siteId, roleId } = this.props.getRolePermissionParams;
        const permissions = this.props.rolePermission.permissions;
        this.props.assignPermission({ siteId, roleId, permissions }).then(response => {
            Site.unblockUI();
            if (response.data.isSuccessful) {
                toast.success(LangHelper.getResource('UpdatePermissionSuccessfully'));
            } else {
                toast.error(response.data.message);
            }
        });
    };

    render() {
        const { sites, roles, rolePermission } = this.props;
        return (
            <>
                <Filter />
                <DataGrid />
                {
                    window.hasHostPermissionOnRole &&
                    sites.length > 0 && roles.length > 0 &&
                    rolePermission && rolePermission.functions.length > 0 &&
                    <Button variant="primary" size="sm" className="mt-3" type="submit" onClick={this.handleSubmit}>
                        {LangHelper.getResource('Submit')}
                    </Button>
                }
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { sites, roles, getRolePermissionParams, rolePermission } = state;
    return { sites, roles, getRolePermissionParams, rolePermission };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);