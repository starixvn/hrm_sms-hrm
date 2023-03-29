import React, { Component } from 'react';
import { Scrollbars } from 'react-custom-scrollbars';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import HRM from './HRM';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            hrmtodoist: [],
            hrpptodoist: [],
            epatodoist: [],
            epa360todoist: []
        };
    }

    async componentDidMount() {
        const url = Site.resolveClientUrl('home/GetHrmTodoist');
        await DataService.getDataAsync(url).then(response => {
            this.setState({ hrmtodoist: response.data });
        });
    }

    render() {
        return (
            <div className="card-container">
                <div className="card">
                    <div className="card-content">
                        <div className="card-title">
                            <span className="txt">HRM</span>
                        </div>
                        <div className="card-body">
                            <Scrollbars autoHeight>
                                <HRM todoist={this.state.hrmtodoist} />
                            </Scrollbars>
                        </div>
                    </div>
                </div>
                <div className="card">
                    <div className="card-content">
                        <div className="card-title">
                            <span className="txt">HRPP</span>
                        </div>
                        <div className="card-body">
                            <div className="todoist-items">
                                <p className="todoist-item green-border-left">
                                    <span>
                                        Nothing to do
                                    </span>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="card">
                    <div className="card-content">
                        <div className="card-title">
                            <span className="txt">EPA</span>
                        </div>
                        <div className="card-body">
                            <div className="todoist-items">
                                <p className="todoist-item green-border-left">
                                    <span>
                                        Nothing to do
                                    </span>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="card">
                    <div className="card-content">
                        <div className="card-title">
                            <span className="txt">360° EPA</span>
                        </div>
                        <div className="card-body">
                            <div className="todoist-items">
                                <p className="todoist-item green-border-left">
                                    <span>
                                        Nothing to do
                                    </span>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default AppContainer;