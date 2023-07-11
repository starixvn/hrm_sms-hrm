import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';

import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';

import LangHelper from '@jsroot/common/langHelper';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import { AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';

export default class DataGrid extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            requests: null
        };
    }

    componentDidMount() {
        this.getAnnualRequestByPeriodId();
    }

    getAnnualRequestByPeriodId = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/GetByPeriodId');
        const params = { periodId: window.periodId };
        await DataService.postDataAsync(url, params).then(response => {
            this.setState({
                requests: response.data
            });
        }).then(() => Site.unblockUI());
    };

    buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={3}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('UnitHead')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Unit')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Verifier')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Approver')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Status')}</HeaderCell>
                <HeaderCell colWidth={3}>{LangHelper.getResource('Action')}</HeaderCell>
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
        const requests = this.state.requests;
        if (requests.length > 0) {
            return requests.map((item, index) =>
                <Row key={item.requestId}>
                    <Cell className="text-center">{index + 1}</Cell>
                    <Cell>{`${item.creator.fullName} (${item.creator.nickname})`}</Cell>
                    <Cell>{item.unitName}</Cell>
                    <Cell>{item.verifier != null ? `${item.verifier.fullName} (${item.verifier.nickname})` : ''}</Cell>
                    <Cell>{item.approver != null ? `${item.approver.fullName} (${item.approver.nickname})` : ''}</Cell>
                    <Cell className="text-center">{this.renderStatus(item.statusId)}</Cell>
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

    render() {
        if (!this.state.requests) {
            return null;
        }

        return (
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
        );
    }
}