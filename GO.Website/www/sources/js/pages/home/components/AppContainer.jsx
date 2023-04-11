import React, { Component } from 'react';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import HRM from './HRM';
import HRPP from './HRPP';
import EPA from './EPA';
import EPA360 from './EPA360';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            block: {
                hrm: false,
                hrpp: false,
                epa: false,
                epa360: false
            }
        };
    }

    async componentDidMount() {
        const url = Site.resolveClientUrl('Todolist/GetTodolistBlock');
        await DataService.getDataAsync(url).then(response => {
            this.setState({ block: response.data });
        });
    }

    render() {
        const block = this.state.block;

        return (
            <div className="card-container">
                {
                    block.hrm &&
                    <HRM />
                }
                {
                    block.hrpp &&
                    <HRPP />
                }
                {
                    block.epa &&
                    <EPA />
                }
                {
                    block.epa360 &&
                    <EPA360 />
                }
            </div>
        );
    }
}

export default AppContainer;