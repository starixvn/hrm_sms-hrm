import React from 'react';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import RowItem from './RowItem';
import { PAGING_SIZES } from '@jsroot/common/constants';
import LangHelper from '@jsroot/common/langHelper';

const DataGrid = ({ filter, gridData, getEmployeeInfos, onFilterChange, onChangeStatus }) => {
    const onPaging = (pageIndex, pageSize) => {
        const params = {
            ...filter,
            pageIndex,
            pageSize
        };

        onFilterChange(params);
        getEmployeeInfos(params);
    };

    const buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Company')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Username')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Code')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Nickname')}</HeaderCell>
                <HeaderCell colWidth={12}>{LangHelper.getResource('FullName')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('JobLevel')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Category')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Unit')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Department')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Section')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Team')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Include')}</HeaderCell>
            </HeaderRow>
        );
    };

    const buildBody = () => {
        const { totalItems, data } = gridData;
        if (totalItems > 0) {
            const { pageIndex, pageSize } = filter;
            const startIndex = (pageIndex - 1) * pageSize + 1;
            return data.map((item, index) =>
                <RowItem
                    key={`row-${item.siteId}-${item.empId}`}
                    employee={item} index={startIndex + index}
                    onChangeStatus={onChangeStatus}
                />
            );
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={13}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    if (!gridData) {
        return null;
    }

    const { pageIndex, pageSize } = filter;
    const { totalItems } = gridData;
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
            header={buildHeader()}
            body={buildBody()}
            isPaging
            pageOption={pagingOption}
            onPaging={onPaging}
            containerPadding={0}
            adjustedHeight={10}
        />
    );
};

export default DataGrid;