import React, { Component } from 'react';
import { Scrollbars } from 'react-custom-scrollbars';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import HRM from './HRM';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            hrmtodolist: [],
            hrpptodolist: [],
            epatodolist: [],
            epa360todolist: []
        };
    }

    async componentDidMount() {
        const url = Site.resolveClientUrl('todolist/GetHrmTodolist');
        await DataService.getDataAsync(url).then(response => {
            this.setState({ hrmtodolist: response.data });
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
                            <Scrollbars autoHeight autoHeightMax={500}>
                                <HRM todolist={this.state.hrmtodolist} />
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
                            <div className="todolist-items">
                                <p className="todolist-item green-border-left">
                                    <span className="txt">
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
                            <div className="todolist-items">
                                <p className="todolist-item green-border-left">
                                    <span className="txt">
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
                            <div className="todolist-items">
                                <p className="todolist-item green-border-left">
                                    <span className="txt">
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