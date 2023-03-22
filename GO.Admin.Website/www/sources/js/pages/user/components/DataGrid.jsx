import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Dropdown } from 'react-bootstrap';
import { ActionCreators } from '../actions';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import Site from '@jsroot/common/site';
import { PAGING_SIZES } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';

export class DataGrid extends Component {
    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.getAllUserParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGetAllUserParams(params);
        this.props.getAllUser(params);
    };

    buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Username')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Code')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Nickname')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('FullName')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('Email')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Status')}</HeaderCell>
                <HeaderCell colWidth={6}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildBody = () => {
        const { totalItems } = this.props.userGridData;
        if (totalItems > 0) {
            const { pageIndex, pageSize } = this.props.getAllUserParams;
            const startIndex = (pageIndex - 1) * pageSize + 1;
            return this.props.userGridData.data.map((item, index) => this.renderRow(item, startIndex + index));
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

    handleOpenResetPasswordModalDialog = user => {
        this.props.setResetPasswordModalDialog({ show: true, user });
    };

    renderActionCell = user => {
        const customDropdown = React.forwardRef(({ onClick }, ref) => (
            <span ref={ref} onClick={event => onClick(event)} className="fa fa-cog"></span>
        ));

        return <Dropdown>
            <Dropdown.Toggle as={customDropdown} id="dropdown-basic" />
            <Dropdown.Menu popperConfig={{ strategy: 'fixed' }}>
                {
                    window.hasHostPermissionOnUser &&
                    <>
                        <Dropdown.Item onClick={() => this.handleOpenResetPasswordModalDialog(user)}>
                            {LangHelper.getResource('ResetPassword')}
                        </Dropdown.Item>
                        <Dropdown.Divider />
                    </>
                }
                <Dropdown.Item href={Site.resolveClientUrl(`user/assignSite/${user.userId}`)}>
                    {LangHelper.getResource('ViewSites')}
                </Dropdown.Item>
                <Dropdown.Divider />
                <Dropdown.Item href={Site.resolveClientUrl(`user/permission/${user.userId}`)}>
                    {LangHelper.getResource('ViewPermission')}
                </Dropdown.Item>
                <Dropdown.Divider />
                <Dropdown.Item href={Site.resolveClientUrl(`user/usermapping/${user.userId}`)}>
                    {LangHelper.getResource('ViewMapping')}
                </Dropdown.Item>
            </Dropdown.Menu>
        </Dropdown>;
    };

    renderRow = (user, index) => (
        <Row key={`row-${index}`}>
            <Cell className="text-center">{index}</Cell>
            <Cell>{user.username}</Cell>
            <Cell>{user.code}</Cell>
            <Cell>{user.nickname}</Cell>
            <Cell>{user.fullName}</Cell>
            <Cell>{user.email}</Cell>
            <Cell className="text-center">{this.getStatus(user)}</Cell>
            <Cell className="text-center action-cell">{this.renderActionCell(user)}</Cell>
        </Row>
    );

    render() {
        if (!this.props.userGridData) {
            return null;
        }

        const { pageIndex, pageSize } = this.props.getAllUserParams;
        const { totalItems } = this.props.userGridData;
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
    const { getAllUserParams, userGridData } = state;
    return { getAllUserParams, userGridData };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);