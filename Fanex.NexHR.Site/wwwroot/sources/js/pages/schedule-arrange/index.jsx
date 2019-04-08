import 'jquery-ui-bundle/jquery-ui';

import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import { cloneDeep, isEqual, unset } from 'lodash';

import AlertModal from '../../shared/components/AlertModal';
import ShiftMultipleModal from './ShiftMultipleModal';
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
            employeeOptions: [],
            showShiftMultipleModal: false,
            alertModal: {
                show: false,
                message: ''
            }
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

        window.addEventListener('beforeunload', this.checkDirtyData);
    }

    componentWillUnmount() {
        window.removeEventListener('beforeunload', this.checkDirtyData);
    }

    checkDirtyData = event => {
        let isDirtyData = false;
        let schedules = cloneDeep(this.state.schedules);
        let originalSchedules = cloneDeep(this.originalSchedules);
        const totalItems = schedules.length;

        for (let index = 0; index < totalItems; index++) {
            if (!this.isDirtyObject(originalSchedules[index], schedules[index])) {
                isDirtyData = true;
                break;
            }
        }

        if (!isDirtyData) {
            return undefined;
        }

        var confirmationMessage = 'It looks like you have been editing something. '
            + 'If you leave before saving, your changes will be lost.';

        (event || window.event).returnValue = confirmationMessage;

        return confirmationMessage;
    }

    isDirtyObject = (schedule, originalSchedule) => {
        unset(originalSchedule, 'selectWeekDays');
        unset(schedule, 'selectWeekDays');

        return isEqual(originalSchedule, schedule);
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
            this.setState({ schedules: cloneDeep(this.originalSchedules) });
        });
    }

    handleGetSheduleArranges = () => {
        this.getSheduleArranges(this.state.filter);
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
            let schedule = cloneDeep(schedules[index]);

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

    handleChangeOffNoshift = () => {
        let schedules = this.state.schedules.slice();
        let selectedSchedules = schedules
            .filter(schedule => Object.keys(schedule.selectWeekDays).filter(objectKey => schedule.selectWeekDays[objectKey]).length > 0)
            .map(schedule => schedule.employeeId);
        const totalItems = selectedSchedules.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                let schedule = cloneDeep(schedules.find(item => item.employeeId === selectedSchedules[index]));
                const selectWeekDays = schedule.selectWeekDays;

                Object.keys(selectWeekDays).forEach(objectKey => {
                    if (selectWeekDays[objectKey] && !schedule[objectKey].isHoliday) {
                        schedule[objectKey].isOffDay = true;
                        schedule[objectKey].shift = null;
                    }
                });

                schedules[index] = schedule;
            }

            this.setState({ schedules });
        } else {
            this.setState({
                alertModal: {
                    show: true,
                    message: 'Please choose at least one day to off!'
                }
            });
        }
    }

    handleChangeOffdayOT = () => {
        let schedules = this.state.schedules.slice();
        let selectedSchedules = schedules
            .filter(schedule => Object.keys(schedule.selectWeekDays).filter(objectKey => schedule.selectWeekDays[objectKey]).length > 0)
            .map(schedule => schedule.employeeId);
        const totalItems = selectedSchedules.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                let schedule = cloneDeep(schedules.find(item => item.employeeId === selectedSchedules[index]));
                const selectWeekDays = schedule.selectWeekDays;
                let isChanged = false;

                Object.keys(selectWeekDays).forEach(objectKey => {
                    if (selectWeekDays[objectKey] && !schedule[objectKey].isHoliday && schedule[objectKey].shift !== null) {
                        schedule[objectKey].isOffDay = true;
                        isChanged = true;
                    }
                });

                if (isChanged) {
                    schedules[index] = schedule;
                }
            }

            this.setState({ schedules });
        } else {
            this.setState({
                alertModal: {
                    show: true,
                    message: 'Please choose at least one day to off!'
                }
            });
        }
    }

    handleChangeWorkday = () => {
        let schedules = this.state.schedules.slice();
        let selectedSchedules = schedules
            .filter(schedule => Object.keys(schedule.selectWeekDays).filter(objectKey => schedule.selectWeekDays[objectKey]).length > 0)
            .map(schedule => schedule.employeeId);
        const totalItems = selectedSchedules.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                let schedule = cloneDeep(schedules.find(item => item.employeeId === selectedSchedules[index]));
                const selectWeekDays = schedule.selectWeekDays;
                let isChanged = false;

                Object.keys(selectWeekDays).forEach(objectKey => {
                    if (selectWeekDays[objectKey] && !schedule[objectKey].isHoliday && schedule[objectKey].isOffDay) {
                        schedule[objectKey].isOffDay = false;
                        isChanged = true;
                    }
                });

                if (isChanged) {
                    schedules[index] = schedule;
                }
            }

            this.setState({ schedules });
        } else {
            this.setState({
                alertModal: {
                    show: true,
                    message: 'Please choose at least one day to work!'
                }
            });
        }
    }

    handleShiftMultiple = () => {
        let schedules = this.state.schedules.slice();
        let selectedSchedules = schedules
            .filter(schedule => Object.keys(schedule.selectWeekDays).filter(objectKey => schedule.selectWeekDays[objectKey]).length > 0)
            .map(schedule => schedule.employeeId);
        const totalItems = selectedSchedules.length;

        if (totalItems > 0) {
            this.setState({ showShiftMultipleModal: true });
        } else {
            this.setState({
                alertModal: {
                    show: true,
                    message: 'Please choose at least one day to continue!'
                }
            });
        }
    }

    handleChangeMultipleShift = (shift, isOffday) => {
        let schedules = this.state.schedules.slice();
        let selectedSchedules = schedules
            .filter(schedule => Object.keys(schedule.selectWeekDays).filter(objectKey => schedule.selectWeekDays[objectKey]).length > 0)
            .map(schedule => schedule.employeeId);
        const totalItems = selectedSchedules.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                let schedule = cloneDeep(schedules.find(item => item.employeeId === selectedSchedules[index]));
                const selectWeekDays = schedule.selectWeekDays;
                let isChanged = false;

                Object.keys(selectWeekDays).forEach(objectKey => {
                    if (selectWeekDays[objectKey] && !schedule[objectKey].isHoliday) {
                        schedule[objectKey].isOffDay = isOffday;
                        schedule[objectKey].shift = shift;

                        isChanged = true;
                    }
                });

                if (isChanged) {
                    schedules[index] = schedule;
                }
            }

            this.setState({ schedules });
        }

        this.handleCloseShiftMultipleModal();
    }

    handleSave = () => {
        let scheduleArranges = [];
        let shiftMultiples = [];
        let schedules = cloneDeep(this.state.schedules);
        let originalSchedules = cloneDeep(this.originalSchedules);
        const totalItems = schedules.length;

        for (let index = 0; index < totalItems; index++) {
            if (!this.isDirtyObject(originalSchedules[index], schedules[index])) {
                Object.keys(schedules[index].selectWeekDays).forEach(objectKey => {
                    scheduleArranges.push({
                        employeeId: schedules[index].employeeId,
                        workDate: schedules[index][objectKey].workDate,
                        shiftId: schedules[index][objectKey].shift !== null ? schedules[index][objectKey].shift.id : '',
                        isHoliday: schedules[index][objectKey].isHoliday,
                        isOffDay: schedules[index][objectKey].isOffDay,
                        isPublic: schedules[index].isPublic
                    });
                });
            }
        }

        console.log(scheduleArranges);
    }

    handleCloseShiftMultipleModal = () => {
        this.setState({ showShiftMultipleModal: false });
    }

    handleCloseAlertModal = () => {
        this.setState({
            alertModal: {
                show: false,
                message: ''
            }
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
                    onChangeOffNoShift={this.handleChangeOffNoshift}
                    onChangeOffdayOT={this.handleChangeOffdayOT}
                    onChangeWorkday={this.handleChangeWorkday}
                    onShiftMultiple={this.handleShiftMultiple}
                    onSave={this.handleSave}
                />
                {
                    this.state.showShiftMultipleModal &&
                    <ShiftMultipleModal show={this.state.showShiftMultipleModal}
                        onHide={this.handleCloseShiftMultipleModal}
                        shiftOptions={this.shiftOptions}
                        onSubmit={this.handleChangeMultipleShift}
                    />
                }
                {
                    this.state.alertModal.show &&
                    <AlertModal show={this.state.alertModal.show}
                        message={this.state.alertModal.message}
                        onHide={this.handleCloseAlertModal}
                    />
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));