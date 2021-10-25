import React, { Component, Fragment } from 'react';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
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
                <HeaderCell colWidth={10}>{LangHelper.getResource('Nickname')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('FullName')}</HeaderCell>
                <HeaderCell colWidth={15}>{LangHelper.getResource('Email')}</HeaderCell>
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

    renderRow = (item, index) => (
        <Row key={`row-${index}`}>
            <Cell className="text-center">{index}</Cell>
            <Cell>{item.username}</Cell>
            <Cell>{item.nickname}</Cell>
            <Cell>{item.fullName}</Cell>
            <Cell>{item.email}</Cell>
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
            <Fragment>
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
            </Fragment>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getAllUserParams, userGridData } = state;
    return { getAllUserParams, userGridData };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);