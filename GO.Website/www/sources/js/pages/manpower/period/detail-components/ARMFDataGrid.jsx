import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';
import { AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';

export default class DataGrid extends PureComponent {
    componentDidMount() {
        this.props.getAnnualRequestByPeriodId();
    }

    buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={2}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('UnitHead')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Unit')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Verifier')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Approver')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Status')}</HeaderCell>
                {
                    window.hasHostPermissionOnPeriod &&
                    <HeaderCell colWidth={3}>{LangHelper.getResource('Action')}</HeaderCell>
                }
            </HeaderRow>
        );
    };

    renderStatus = statusId => {
        let status = '';
        switch (statusId) {
            case AnnualManpowerForecastRequestStatus.Pending:
                status = LangHelper.getResource('Pending');
                break;
            case AnnualManpowerForecastRequestStatus.Verified:
                status = LangHelper.getResource('Verified');
                break;
            case AnnualManpowerForecastRequestStatus.Approved:
                status = LangHelper.getResource('Approved');
                break;
        }

        return status;
    };

    buildBody = () => {
        const gridData = this.props.gridData;
        if (gridData.length > 0) {
            return gridData.map((item, index) =>
                <Row key={item.requestId}>
                    <Cell className="text-center">{index + 1}</Cell>
                    <Cell className="text-left">{`${item.creator.fullName} (${item.creator.nickname})`}</Cell>
                    <Cell className="text-left">{item.unitName}</Cell>
                    <Cell className="text-left">{`${item.verifierFullName} (${item.verifierNickName})`}</Cell>
                    <Cell className="text-left">{`${item.approverFullName} (${item.approverNickName})`}</Cell>
                    <Cell className="text-center">{this.renderStatus(item.statusId)}</Cell>
                    {
                        window.hasHostPermissionOnPeriod &&
                        <Cell className="text-center">
                            <a href={`/ManpowerForecast/AnnualRequest/Detail/${item.requestId}`}>
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
                    }
                </Row>
            );
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={5}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    render() {
        if (!this.props.gridData) {
            return null;
        }

        return (
            <div>
                <Table
                    minWidth={720}
                    maxWidth={document.getElementById('detail-app-container').clientWidth}
                    autoWidth
                    header={this.buildHeader()}
                    body={this.buildBody()}
                    isPaging={false}
                    containerPadding={0}
                    adjustedHeight={10}
                />
            </div>
        );
    }
}