import React, { useEffect, useState } from 'react';
import { Tabs, Tab } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import UnitList from './UnitList';
import FlowSetting from './FlowSetting';
import Result from './Result';

import { ActionCreators } from '../actions';

const AppContainer = ({ units, getUnits }) => {
    const [activeKey, setActiveKey] = useState('setting');
    const [isFirstLoadResultTab, setFirstLoadResultTab] = useState(false);

    useEffect(() => {
        getUnits();
    }, []);

    const handleSelect = key => {
        setActiveKey(key);

        if (key === 'result' && !isFirstLoadResultTab) {
            setFirstLoadResultTab(true);
        }
    };

    return (
        <div className="manpower-flow">
            {
                !window.hasHostPermissionOnFlow &&
                <div className="result-settings">
                    <Result units={units} />
                </div>
            }
            {
                window.hasHostPermissionOnFlow &&
                <Tabs id="manpower-flow-tab" activeKey={activeKey} onSelect={handleSelect}>
                    <Tab eventKey="setting" title="Setting" className="setting">
                        <div className="flow-settings">
                            <UnitList />
                            <FlowSetting />
                        </div>
                    </Tab>
                    <Tab eventKey="result" title="Result">
                        {
                            isFirstLoadResultTab &&
                            <div className="result-settings">
                                <Result units={units} />
                            </div>
                        }
                    </Tab>
                </Tabs>
            }
        </div>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { units } = state;
    return { units };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);