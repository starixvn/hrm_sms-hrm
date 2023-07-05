import React, { PureComponent } from 'react';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';
import { AnnualManpowerForecastRequestStatus } from '@jsroot/common/constants';

export default class DataGrid extends PureComponent {
    componentDidMount() {
        this.props.getAnnualRequestByPeriodId();
    }

    buildHeader = () => {
        return 
            <HeaderRow>
                <HeaderCell colWidth={2}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('UnitHead')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Unit')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Verifier')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Approver')}</HeaderCell>
                <HeaderCell colWidth={5}>{LangHelper.getResource('Status')}</HeaderCell>
            </HeaderRow>;
    };

    getStatus = statusId => {
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
                 <Row>
                    <Cell className="text-center">{index}</Cell>
                    <Cell>{item.creator.nickName}</Cell>
                    <Cell>{item.unitName}</Cell>
                    <Cell>{item.verifierName}</Cell>
                    <Cell>{item.approverName}</Cell>
                    <Cell>{this.getStatus(item.statusId)}</Cell>
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
            <Table
                minWidth={720}
                maxWidth={document.getElementById('detail-app-container').clientWidth}
                autoWidth
                header={this.buildHeader()}
                body={this.buildBody()}
                containerPadding={0}
                adjustedHeight={10}
            />
        );
    }
}