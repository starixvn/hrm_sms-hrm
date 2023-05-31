import React, { Component } from 'react';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import RowItem from './RowItem';
import { PAGING_SIZES } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';

export default class DataGrid extends Component {
    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.filter };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.onFilterChange(params);
        this.props.getEmployeeInfos(params);
    };

    buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('SiteName')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Username')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Code')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Nickname')}</HeaderCell>
                <HeaderCell colWidth={12}>{LangHelper.getResource('FullName')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Local')} / {LangHelper.getResource('Expat')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Unit')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Department')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Section')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Team')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Include')}</HeaderCell>
            </HeaderRow>
        );
    };

    buildBody = () => {
        const { totalItems, data } = this.props.gridData;
        if (totalItems > 0) {
            const { pageIndex, pageSize } = this.props.filter;
            const startIndex = (pageIndex - 1) * pageSize + 1;
            return data.map((item, index) =>
                <RowItem
                    key={`row-${item.siteId}-${item.empId}`}
                    employee={item} index={startIndex + index}
                    onChangeStatus={this.props.onChangeStatus}
                />
            );
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={10}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    render() {
        if (!this.props.gridData) {
            return null;
        }

        const { pageIndex, pageSize } = this.props.filter;
        const { totalItems } = this.props.gridData;
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