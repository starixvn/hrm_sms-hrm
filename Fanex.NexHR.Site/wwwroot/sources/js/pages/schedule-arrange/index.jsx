import 'jquery-ui-bundle/jquery-ui';

import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import Search from './Search';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            filter: {
                fromDate: '',
                toDate: '',
                groupIds: '',
                jobTitleIds: '',
                employee: { value: 0, label: '' }
            },
            schedules: [],
            employeeOptions: []
        };

        this.originalSchedules = [];
        this.groupOptions = [];
        this.jobTitleOptions = [];
        this.currentWeek = {
            fromDate: '',
            toDate: ''
        };
        this.isHost = document.getElementById('IsHost').checked;
        this.isSARole = document.getElementById('IsSARole').checked;
    }

    componentDidMount() {
        const promises = [
            axios(_global.root + 'ScheduleArrange/getFilter')
        ];

        Promise.all(promises)
            .then(([filterResponse]) => {
                this.groupOptions = filterResponse.data.groups.map(group => { group.checked = true; return group; });
                this.jobTitleOptions = filterResponse.data.jobTitles.map(job => { job.checked = true; return job; });
                const fromDate = moment(filterResponse.data.fromDate).format('DD MMM, YYYY');
                const toDate = moment(filterResponse.data.toDate).format('DD MMM, YYYY');

                this.currentWeek = { fromDate, toDate };

                const filter = Object.assign({}, this.state.filter, {
                    fromDate: fromDate,
                    toDate: toDate,
                    groupIds: this.groupOptions.map(group => group.id).join(','),
                    jobTitleIds: this.jobTitleOptions.map(job => job.id).join(',')
                });

                this.getSheduleArranges(filter);

                this.setState({ filter: filter });
            });
    }

    handleFilterChange = (name, value) => {
        this.setState({
            filter: Object.assign({}, this.state.filter, { [name]: value })
        });
    }

    getEmployees = query => {
        axios(_global.root + 'ScheduleArrange/GetEmployeesByShiftMonitor', {
            params: {
                query: query
            }
        }).then(response => {
            let employeeOptions = response.data.map(item => {
                return {
                    id: item.ownerId,
                    value: `${item.nickName} (${item.fullName})`
                };
            });

            this.setState({ employeeOptions: employeeOptions });
        });
    }

    getSheduleArranges = filter => {
        axios(_global.root + 'ScheduleArrange/ScheduleArrangeDetails', {
            params: {
                fromDate: filter.fromDate,
                toDate: filter.toDate,
                groupIds: filter.groupIds,
                jobTitleIds: filter.groupIds,
                employeeId: filter.employeeId === 0 ? null : filter.employeeId
            }
        }).then(response => {
            this.originalSchedules = response.data;
            this.setState({ schedules: response.data });
        });
    }

    render() {
        return (
            <Fragment>
                <Search
                    groupOptions={this.groupOptions}
                    jobTitleOptions={this.jobTitleOptions}
                    employeeOptions={this.state.employeeOptions}
                    filter={this.state.filter}
                    onSearchEmployees={this.getEmployees}
                    onChange={this.handleFilterChange}
                />
                <Table
                    isHost={this.isHost}
                    isSARole={this.isSARole}
                    fromDate={this.state.filter.fromDate}
                    toDate={this.state.filter.toDate}
                />
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));