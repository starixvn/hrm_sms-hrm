import React from 'react';
import { Button, Form } from 'react-bootstrap';

import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';

const DataGrid = ({ requests }) => {
    const buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Period')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Status')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    const buildBody = () => {
        if (requests.length > 0) {
            return requests.map((item, index) =>
                <Row key={item.requestId}>
                    <Cell className="text-center">{index}</Cell>
                    <Cell className="text-center" />
                    <Cell className="text-center" />
                    <Cell className="text-center" />
                </Row>
            );
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={4}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    return (
        <div>
            <div className="table-title d-flex justify-content-between align-items-center">
                <Button variant="primary" size="sm" className="mr-3" type="submit">
                    {LangHelper.getResource('Create')}
                </Button>
            </div>
            <Table
                minWidth={720}
                maxWidth={document.getElementById('app-container').clientWidth}
                autoWidth
                header={buildHeader()}
                body={buildBody()}
                isPaging={false}
                containerPadding={0}
                adjustedHeight={10}
            />
        </div>
    );
};

export default DataGrid;