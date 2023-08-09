import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';

import { ActionCreators } from '../actions';
import { PAGING_SIZES } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';

export class DataGrid extends Component {
    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.userRoleFilterParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setUserRoleFilterParams(params);
        this.props.getUserRoles(params);
    };

    buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Username')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Code')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Nickname')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('FullName')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('Roles')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Status')}</HeaderCell>
                {
                    window.hasHostPermissionOnRoleAssignment &&
                    <HeaderCell colWidth={6}>{LangHelper.getResource('Action')}</HeaderCell>
                }
            </HeaderRow>
        );
    };

    buildBody = () => {
        const { totalItems } = this.props.userRoleGridData;
        if (totalItems > 0) {
            const { pageIndex, pageSize } = this.props.userRoleFilterParams;
            const startIndex = (pageIndex - 1) * pageSize + 1;
            return this.props.userRoleGridData.data.map((item, index) => this.renderRow(item, startIndex + index));
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={10}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    getStatus = user => {
        let status = LangHelper.getResource('Active');
        if (!user.isActive) {
            if (user.isDeleted) {
                status = LangHelper.getResource('Deleted');
            } else {
                status = LangHelper.getResource('Inactive');
            }
        }

        return status;
    };

    handleUpdateRole = user => {
        this.props.setUpdateUserRoleModalDialog({
            show: true,
            user
        });
    };

    renderRow = (user, index) => (
        <Row key={`row-${index}`}>
            <Cell className="text-center">{index}</Cell>
            <Cell>{user.username}</Cell>
            <Cell>{user.code}</Cell>
            <Cell>{user.nickname}</Cell>
            <Cell>{user.fullName}</Cell>
            <Cell>{user.roles.map(role => role.name).join(', ')}</Cell>
            <Cell className="text-center">{this.getStatus(user)}</Cell>
            {
                window.hasHostPermissionOnRoleAssignment &&
                <Cell className="text-center action-cell">
                    <a className="icon-edit fa fa-edit" onClick={() => this.handleUpdateRole(user)} />
                </Cell>
            }
        </Row>
    );

    render() {
        if (!this.props.userRoleGridData) {
            return null;
        }

        const { pageIndex, pageSize } = this.props.userRoleFilterParams;
        const { totalItems } = this.props.userRoleGridData;
        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: totalItems,
            PageList: PAGING_SIZES
        };
        return (
            <Table
                minWidth={720}
                maxWidth={document.getElementById('app-container').clientWidth}
                autoWidth
                header={this.buildHeader()}
                body={this.buildBody()}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={10}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { userRoleFilterParams, userRoleGridData } = state;
    return { userRoleFilterParams, userRoleGridData };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);