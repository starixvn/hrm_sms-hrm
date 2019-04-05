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
        this.shiftOptions = [];
        this.currentWeek = {
            fromDate: '',
            toDate: ''
        };
        this.selectedWeek = {
            fromDate: moment().day('Monday').format('DD MMM, YYYY'),
            toDate: moment().day('Sunday').format('DD MMM, YYYY')
        };
        this.isHost = document.getElementById('IsHost').checked;
        this.isSARole = document.getElementById('IsSARole').checked;
    }

    componentDidMount() {
        const promises = [
            axios(_global.root + 'ScheduleArrange/getFilter'),
            axios(_global.root + 'Calendar/GetShift', {
                params: {
                    query: '',
                    isShowOff: true
                }
            })
        ];

        Promise.all(promises)
            .then(([filterResponse, shiftResponse]) => {
                this.groupOptions = filterResponse.data.groups.map(group => { group.checked = true; return group; });
                this.jobTitleOptions = filterResponse.data.jobTitles.map(job => { job.checked = true; return job; });
                this.shiftOptions = shiftResponse.data.map(shift => { return { id: shift.Id, code: shift.Code }; });
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
        const fromDate = moment(filter.fromDate).format('DD MMM, YYYY');
        const toDate = moment(filter.toDate).format('DD MMM, YYYY');

        axios(_global.root + 'ScheduleArrange/ScheduleArrangeDetails', {
            params: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: filter.groupIds,
                jobTitleIds: filter.jobTitleIds,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value
            }
        }).then(response => {
            this.selectedWeek = { fromDate, toDate };

            this.originalSchedules = response.data.map(schedule => {
                schedule.defaultShift = {
                    id: schedule.mondaySchedule.shiftMultipleId,
                    code: schedule.mondaySchedule.shiftMultipleCode
                };
                schedule.isPublic = schedule.mondaySchedule.isPublic;
                schedule.selectWeekDays = {
                    mondaySchedule: false,
                    tuesdaySchedule: false,
                    wednesdaySchedule: false,
                    thursdaySchedule: false,
                    fridaySchedule: false,
                    saturdaySchedule: false,
                    sundaySchedule: false
                };

                return schedule;
            });
            this.setState({ schedules: this.originalSchedules });
        });
    }

    handleGetSheduleArranges = () => {
        this.getSheduleArranges(this.state.filter);
    }

    handleChangeSelectWeekDay = (employeeId, scheduleAttrName, isChecked) => {
        let schedules = this.state.schedules.slice();
        const index = schedules.findIndex(item => item.employeeId === employeeId);

        if (index >= 0) {
            schedules[index] = Object.assign({}, schedules[index], {
                selectWeekDays: { ...schedules[index].selectWeekDays, [scheduleAttrName]: isChecked }
            });

            this.setState({ schedules });
        }
    }

    handleChangeSelectWeekDays = (scheduleAttrName, isChecked) => {
        let schedules = this.state.schedules.slice();
        const totalItems = schedules.length;

        if (scheduleAttrName !== null) {
            for (let index = 0; index < totalItems; index++) {
                schedules[index] = Object.assign({}, schedules[index], {
                    selectWeekDays: { ...schedules[index].selectWeekDays, [scheduleAttrName]: isChecked }
                });
            }
        } else {
            for (let index = 0; index < totalItems; index++) {
                schedules[index] = Object.assign({}, schedules[index], {
                    selectWeekDays: {
                        mondaySchedule: isChecked,
                        tuesdaySchedule: isChecked,
                        wednesdaySchedule: isChecked,
                        thursdaySchedule: isChecked,
                        fridaySchedule: isChecked,
                        saturdaySchedule: isChecked,
                        sundaySchedule: isChecked
                    }
                });
            }
        }

        this.setState({ schedules });
    }

    handleChangeShift = (employeeId, attrName, shift) => {
        let schedules = this.state.schedules.slice();
        const index = schedules.findIndex(item => item.employeeId === employeeId);

        if (index >= 0) {
            let schedule = { ...schedules[index] };
            if (shift !== null) {
                const isOffDay = shift.id === -1;

                if (isOffDay) {
                    if (!schedule[attrName].isHoliday) {
                        schedule[attrName].isOffDay = true;
                        schedule[attrName].shift = null;
                    } else {
                        schedule[attrName].shift = null;
                    }
                } else {
                    schedule[attrName].isOffDay = false;
                    schedule[attrName].shift = { ...schedule[attrName].shift, id: shift.id, code: shift.code };
                }
            } else {
                schedule[attrName].isOffDay = false;
                schedule[attrName].shift = null;
            }

            schedules[index] = schedule;

            this.setState({ schedules });
        }
    }

    handleChangeDefaultShift = (employeeId, shift) => {
        let schedules = this.state.schedules.slice();
        const index = schedules.findIndex(item => item.employeeId === employeeId);

        if (index >= 0) {
            schedules[index] = Object.assign({}, schedules[index], { defaultShift: shift });

            this.setState({ schedules });
        }
    }

    handleChangePublics = (employeeId, isChecked) => {
        let schedules = this.state.schedules.slice();

        if (employeeId !== null) {
            const index = schedules.findIndex(item => item.employeeId === employeeId);

            if (index >= 0) {
                schedules[index] = Object.assign({}, schedules[index], { isPublic: isChecked });

                this.setState({ schedules });
            }
        } else {
            const totalItems = schedules.length;
            for (let index = 0; index < totalItems; index++) {
                schedules[index] = Object.assign({}, schedules[index], { isPublic: isChecked });
            }

            this.setState({ schedules });
        }
    }

    handleGetPreviousWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate).add(-7, 'days').format('DD MMM, YYYY');
        const toDate = moment(this.selectedWeek.toDate).add(-7, 'days').format('DD MMM, YYYY');
        const filter = Object.assign({}, this.state.filter, {
            fromDate: fromDate,
            toDate: toDate
        });

        this.getSheduleArranges(filter);

        this.setState({ filter: filter });
    }

    handleGetNextWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate).add(7, 'days').format('DD MMM, YYYY');
        const toDate = moment(this.selectedWeek.toDate).add(7, 'days').format('DD MMM, YYYY');
        const filter = Object.assign({}, this.state.filter, {
            fromDate: fromDate,
            toDate: toDate
        });

        this.getSheduleArranges(filter);

        this.setState({ filter: filter });
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
                    onSubmit={this.handleGetSheduleArranges}
                />
                <Table
                    isHost={this.isHost}
                    isSARole={this.isSARole}
                    fromDate={this.selectedWeek.fromDate}
                    toDate={this.selectedWeek.toDate}
                    data={this.state.schedules}
                    shiftOptions={this.shiftOptions}
                    onPrevious={this.handleGetPreviousWeekSchedule}
                    onNext={this.handleGetNextWeekSchedule}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                    onChangeSelectWeekDays={this.handleChangeSelectWeekDays}
                    onChangeShift={this.handleChangeShift}
                    onChangeDefaultShift={this.handleChangeDefaultShift}
                    onChangePublics={this.handleChangePublics}
                />
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));