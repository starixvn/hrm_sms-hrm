import React, { PureComponent } from 'react';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import RowItem from './RowItem';
import { PAGING_SIZES } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';

export default class DataGrid extends PureComponent {
    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.filter };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.onFilterChange(params);
        this.props.getPeriods(params);
    };

    buildHeader = () => {
        return [
            <HeaderRow key="header-row1">
                <HeaderCell colWidth={3} rowSpan={2}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10} rowSpan={2}>{LangHelper.getResource('Name')}</HeaderCell>
                <HeaderCell colWidth={8} colSpan={2}>{LangHelper.getResource('UnitHead')}</HeaderCell>
                <HeaderCell colWidth={8} colSpan={2}>{LangHelper.getResource('Verifier')}</HeaderCell>
                <HeaderCell colWidth={8} colSpan={2}>{LangHelper.getResource('Approver')}</HeaderCell>
                <HeaderCell colWidth={4} rowSpan={2}>{LangHelper.getResource('Active')}</HeaderCell>
                {
                    window.hasHostPermissionOnPeriod &&
                    <HeaderCell colWidth={4} rowSpan={2}>{LangHelper.getResource('Action')}</HeaderCell>
                }
            </HeaderRow>,
            <HeaderRow key="header-row2">
                <HeaderCell colWidth={8}>{LangHelper.getResource('FromDate')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('ToDate')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('FromDate')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('ToDate')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('FromDate')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('ToDate')}</HeaderCell>
            </HeaderRow>
            ];
    };

    buildBody = () => {
        const { totalItems, data } = this.props.gridData;
        if (totalItems > 0) {
            const { pageIndex, pageSize } = this.props.filter;
            const startIndex = (pageIndex - 1) * pageSize + 1;

            return data.map((item, index) =>
                <RowItem
                    key={`row-${item.name}`}
                    period={item} rowIndex={startIndex + index}
                    onChangeStatus={this.props.onChangeStatus}
                    onUpdate={this.props.onUpdate}
                    onDelete={this.props.onDelete}
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