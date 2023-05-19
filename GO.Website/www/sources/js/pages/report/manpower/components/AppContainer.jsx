import React, { Component } from 'react';
import { Button } from 'react-bootstrap';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import GroupByUnitGrid from './group-by-unit-grid/index';
import GroupByCountryGrid from './group-by-country-grid/index';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groupByCountryData: null,
            groupByUnitData: null
        };
    }

    async componentDidMount() {
        await this.getReportData();
    }

    getReportData = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('ManpowerReport/getData');
        await DataService.postDataAsync(url).then(response => {
            this.setState({
                groupByCountryData: response.data.groupByCountryData,
                groupByUnitData: response.data.groupByUnitData
            });
        }).then(() => Site.unblockUI());
    };

    exportExcel = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('ManpowerReport/exportExcel');
        await DataService.getFileAsync(url).then(response => {
            const fileName = response.headers['content-disposition'].split('filename=')[1].split(';')[0];
            Site.downloadFile(response.data, fileName);
        }).then(() => Site.unblockUI());
    };

    render() {
        const { groupByCountryData, groupByUnitData } = this.state;

        return (
            <div className="manpower-report">
                <ol className="breadcrumb">
                    <li className="breadcrumb-item active">Manpower Report</li>
                </ol>
                <Button variant="secondary" size="sm" className="btn-excel" type="button" onClick={this.exportExcel}>
                    Export to Excel
                </Button>
                {
                    groupByUnitData &&
                    <GroupByUnitGrid data={groupByUnitData} />
                }
                {
                    groupByCountryData &&
                    <GroupByCountryGrid data={groupByCountryData} />
                }
            </div>
        );
    }
}

export default AppContainer;