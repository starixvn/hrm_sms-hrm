import React, { PureComponent } from 'react';
import { Tabs, Tab } from 'react-bootstrap';

import ARMFDataGrid from './ARMFDataGrid';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
export default class AppContainer extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            gridData: null,
        };
    }

    getAnnualRequestByPeriodId = async () => {
        Site.blockUI();
        const url = Site.resolveClientUrl('ManpowerForecast/AnnualRequest/GetAnnualRequestByPeriodId');
        const params = { periodId: window.periodId };
        await DataService.postDataAsync(url, params).then(response => {
            this.setState({
                gridData: response.data
            });
        }).then(() => Site.unblockUI());
    };

    render() {
        const { gridData } = this.state;
        return (
            <div className="period-detail">
                <Tabs id="period-detail-tab">
                    <Tab eventKey="armf" title="ARMF" className="armf">
                        <div className="period-armf">
                            <ARMFDataGrid
                                getAnnualRequestByPeriodId={this.getAnnualRequestByPeriodId}
                                gridData={gridData} />
                        </div>
                    </Tab>
                </Tabs>
            </div>
        );
    }
}