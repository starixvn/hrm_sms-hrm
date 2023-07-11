import React from 'react';
import { Tabs, Tab } from 'react-bootstrap';

import ARMF from './ARMF';

const AppContainer = () => {
    return (
        <div className="period-detail">
            <Tabs id="period-detail-tab">
                <Tab eventKey="armf" title="ARMF" className="armf">
                    <div className="period-armf">
                        <ARMF />
                    </div>
                </Tab>
            </Tabs>
        </div>
    );
};

export default AppContainer;