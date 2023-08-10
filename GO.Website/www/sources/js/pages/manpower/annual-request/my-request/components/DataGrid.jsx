import React from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';

import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';

import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import { getAnnualRequestStatus } from '../../../common.funcs';

const DataGrid = ({ requests }) => {
    const buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={3}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Period')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Unit')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Verifier')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Approver')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Status')}</HeaderCell>
                <HeaderCell colWidth={3}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    const buildBody = () => {
        if (requests.length > 0) {
            return requests.map((item, index) =>
                <Row key={item.requestId}>
                    <Cell className="text-center">{index + 1}</Cell>
                    <Cell>{item.period.name}</Cell>
                    <Cell>{item.unitName}</Cell>
                    <Cell>{item.verifier != null ? `${item.verifier.fullName} (${item.verifier.nickname})` : ''}</Cell>
                    <Cell>{item.approver != null ? `${item.approver.fullName} (${item.approver.nickname})` : ''}</Cell>
                    <Cell className="text-center">{getAnnualRequestStatus(item.statusId)}</Cell>
                    <Cell className="text-center">
                        <a href={Site.resolveClientUrl(`ManpowerForecast/AnnualRequest/Detail/${item.requestId}`)}>
                            <OverlayTrigger placement="auto" trigger={['focus', 'hover']}
                                overlay={
                                    <Tooltip id={`tooltip-request-${item.requestId}`}>
                                        Detail
                                </Tooltip>
                                }
                            >
                                <i className="icon-edit fa fa-search" />
                            </OverlayTrigger>
                        </a>
                    </Cell>
                </Row>
            );
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={7}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    return (
        <div>
            <div className="table-title d-flex justify-content-between align-items-center">
                <a className="mr-3 btn btn-primary btn-sm" href={Site.resolveClientUrl('ManpowerForecast/AnnualRequest/Create')}>
                    {LangHelper.getResource('Create')}
                </a>
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