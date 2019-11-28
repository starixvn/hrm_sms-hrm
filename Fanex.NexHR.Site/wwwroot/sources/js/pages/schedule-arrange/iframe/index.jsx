import 'jquery-ui-bundle/jquery-ui';
import '../../../jquery-weekpicker';

import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { axiosError, blockUI, unblockUI, resolveClientUrl } from '../../../shared/common.functions';
import buildData from '../../../shared/components/react-tree-multiselect/buildData';
import { DateFormat } from '../../../shared/constants';
import Search from '../Search';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        const fromDate = moment().day('Monday').format(DateFormat),
            toDate = moment(fromDate).add(6, 'days').format(DateFormat);

        this.state = {
            filter: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: '',
                employee: { value: 0, label: '' }
            },
            schedules: [],
            employeeOptions: []
        };

        this.groupOptions = [];
        this.currentWeek = { fromDate, toDate };
        this.selectedWeek = { fromDate, toDate };
    }

    componentDidMount() {
        axios(resolveClientUrl('department/GetHierarchy'))
            .then(response => {
                this.groupOptions = response.data;
                const { floatData } = buildData(response.data);
                const filter = { ...this.state.filter, groupIds: floatData.filter(item => item.isLeaf).map(item => item.id).join(',') };

                this.getWeeklyShedules(filter);

                this.setState({ filter: filter });
            }).catch(axiosError);
    }

    handleFilterChange = (name, value) => {
        this.setState({ filter: { ...this.state.filter, [name]: value } });
    }

    getEmployees = query => {
        axios(resolveClientUrl('ScheduleArrange/GetEmployees'), {
            params: {
                groupIds: this.state.filter.groupIds,
                query: query,
                isIFrame: true
            }
        }).then(response => {
            const employeeOptions = response.data.map(item => {
                return {
                    id: item.employeeId,
                    value: `${item.nickname} (${item.fullName})`
                };
            });

            this.setState({ employeeOptions: employeeOptions });
        }).catch(axiosError);
    }

    getWeeklyShedules = filter => {
        const fromDate = moment(filter.fromDate).format(DateFormat);
        const toDate = moment(filter.toDate).format(DateFormat);

        blockUI();

        axios(resolveClientUrl('ScheduleArrange/GetWeeklySchedule'), {
            params: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: filter.groupIds,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value,
                isIFrame: true
            }
        }).then(response => {
            this.selectedWeek = { fromDate, toDate };

            this.setState({
                filter: { ...this.state.filter, fromDate, toDate },
                schedules: response.data.schedules
            });
        }).catch(axiosError).then(() => unblockUI());
    }

    handleGetWeeklyShedules = isCurrentWeek => {
        const filter = isCurrentWeek
            ? { ...this.state.filter, fromDate: this.currentWeek.fromDate, toDate: this.currentWeek.toDate }
            : this.state.filter;
        this.setState({ filter });
        this.getWeeklyShedules(filter);
    }

    handleGetPreviousWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate).add(-7, 'days').format(DateFormat);
        const toDate = moment(this.selectedWeek.toDate).add(-7, 'days').format(DateFormat);
        const filter = { ...this.state.filter, fromDate, toDate };
        this.setState({ filter });
        this.getWeeklyShedules(filter);
    }

    handleGetNextWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate).add(7, 'days').format(DateFormat);
        const toDate = moment(this.selectedWeek.toDate).add(7, 'days').format(DateFormat);
        const filter = { ...this.state.filter, fromDate, toDate };
        this.setState({ filter });
        this.getWeeklyShedules(filter);
    }

    render() {
        const { schedules } = this.state;

        return (
            <Fragment>
                <Search
                    showExportExcel={false}
                    groupOptions={this.groupOptions}
                    employeeOptions={this.state.employeeOptions}
                    filter={this.state.filter}
                    onSearchEmployees={this.getEmployees}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleGetWeeklyShedules}
                />
                <Table
                    fromDate={this.selectedWeek.fromDate} toDate={this.selectedWeek.toDate}
                    data={schedules}
                    onPrevious={this.handleGetPreviousWeekSchedule}
                    onNext={this.handleGetNextWeekSchedule}
                />
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));