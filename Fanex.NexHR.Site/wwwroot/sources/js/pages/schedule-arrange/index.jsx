import 'jquery-ui-bundle/jquery-ui';

import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { toast } from 'react-toastify';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { cloneDeep, isEqual, unset } from 'lodash';

import { blockUI, unblockUI } from '../../shared/common.functions';
import ToastLayout from '../../shared/layouts/ToastLayout';
import AlertModal from '../../shared/components/AlertModal';
import ConfirmRefreshDataModal from './ConfirmRefreshDataModal';
import ShiftMultipleModal from './ShiftMultipleModal';
import Search from './Search';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        let fromDate = moment().day('Monday').format('DD MMM, YYYY');
        let toDate = moment(fromDate).add(6, 'days').format('DD MMM, YYYY');

        this.state = {
            filter: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: '',
                employee: { value: 0, label: '' }
            },
            schedules: [],
            employeeOptions: [],
            showShiftMultipleModal: false,
            showConfirmRefreshDataModal: false,
            alertModal: {
                show: false,
                message: ''
            },
            showRemarkPopover: false,
            isOrderMode: false,
            showConfirmSwitchOrderModeModal: false
        };

        this.originalSchedules = [];
        this.groupOptions = [];
        this.shiftOptions = [];
        this.currentWeek = { fromDate, toDate };
        this.selectedWeek = { fromDate, toDate };
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
                this.shiftOptions = shiftResponse.data.map(shift => { return { id: shift.Id, code: shift.Code, name: shift.Name }; });

                const filter = Object.assign({}, this.state.filter, {
                    groupIds: this.groupOptions.map(group => group.id).join(',')
                });

                this.getWeeklyShedules(filter);

                this.setState({ filter: filter });
            });

        window.addEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    componentWillUnmount() {
        window.removeEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    onLeavingPageEventListener = event => {
        if (!this.isDirtyData()) {
            return undefined;
        }

        const confirmationMessage = 'It looks like you have been editing something. '
            + 'If you leave before saving, your changes will be lost.';

        (event || window.event).returnValue = confirmationMessage;

        return confirmationMessage;
    }

    isDirtyData = () => {
        let isDirty = false;
        let schedules = cloneDeep(this.state.schedules);
        let originalSchedules = cloneDeep(this.originalSchedules);
        const totalItems = schedules.length;

        for (let index = 0; index < totalItems; index++) {
            const schedule = schedules[index];
            const originalSchedule = originalSchedules.filter(item => item.employeeId === schedule.employeeId)[0];
            unset(schedule, 'selectWeekDays');
            unset(originalSchedule, 'selectWeekDays');

            if (!isEqual(schedule, originalSchedule)) {
                isDirty = true;
                break;
            }
        }

        return isDirty;
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

    getWeeklyShedules = filter => {
        const fromDate = moment(filter.fromDate).format('DD MMM, YYYY');
        const toDate = moment(filter.toDate).format('DD MMM, YYYY');

        blockUI();

        axios(_global.root + 'ScheduleArrange/GetWeeklySchedule', {
            params: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: filter.groupIds,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value,
                isOrderMode: this.state.isOrderMode
            }
        }).then(response => {
            this.selectedWeek = { fromDate, toDate };

            this.originalSchedules = response.data.map((schedule, index) => {
                schedule.displayOrder = index + 1;
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
        }).then(() => unblockUI());
    }

    handleGetWeeklyShedules = (isCurrentWeek) => {
        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            const filter = isCurrentWeek
                ? Object.assign({}, this.state.filter, {
                    fromDate: this.currentWeek.fromDate,
                    toDate: this.currentWeek.toDate
                }) : this.state.filter;

            this.setState({ filter });

            this.getWeeklyShedules(filter);
        }
    }

    handleGetPreviousWeekSchedule = () => {
        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            const fromDate = moment(this.selectedWeek.fromDate).add(-7, 'days').format('DD MMM, YYYY');
            const toDate = moment(this.selectedWeek.toDate).add(-7, 'days').format('DD MMM, YYYY');
            const filter = Object.assign({}, this.state.filter, {
                fromDate: fromDate,
                toDate: toDate
            });

            this.getWeeklyShedules(filter);

            this.setState({ filter: filter });
        }
    }

    handleGetNextWeekSchedule = () => {
        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            const fromDate = moment(this.selectedWeek.fromDate).add(7, 'days').format('DD MMM, YYYY');
            const toDate = moment(this.selectedWeek.toDate).add(7, 'days').format('DD MMM, YYYY');
            const filter = Object.assign({}, this.state.filter, {
                fromDate: fromDate,
                toDate: toDate
            });

            this.getWeeklyShedules(filter);

            this.setState({ filter: filter });
        }
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
                    schedule[attrName].shift = shift;
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
                        schedule[objectKey].isOffDay = shift !== null && shift.id === -1 ? true : isOffday;
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
        let isValidData = true;
        let weeklySchedules = [];
        let defaultShifts = [];
        let schedules = cloneDeep(this.state.schedules);
        let originalSchedules = cloneDeep(this.originalSchedules);
        const totalItems = schedules.length;
        const weekDayAttrNames = originalSchedules.length > 0 ? Object.keys(originalSchedules[0].selectWeekDays) : [];

        for (let index = 0; index < totalItems; index++) {
            unset(originalSchedules[index], 'selectWeekDays');
            unset(schedules[index], 'selectWeekDays');

            if (!isEqual(originalSchedules[index], schedules[index])) {
                const schedule = schedules[index];

                let totalOffDays = 0;
                weekDayAttrNames.forEach(objectKey => {
                    if (schedule[objectKey].isOffDay || schedule[objectKey].isLeaveOffDay) {
                        totalOffDays++;
                    }
                });

                if (schedule.offDaysbyWeek < totalOffDays && !this.isSARole) {
                    isValidData = false;
                    break;
                }

                defaultShifts.push({
                    employeeId: schedule.employeeId,
                    fromDate: schedule.mondaySchedule.workDate,
                    shiftId: schedule.defaultShift !== null ? schedule.defaultShift.id : null
                });

                weekDayAttrNames.forEach(objectKey => {
                    weeklySchedules.push({
                        employeeId: schedule.employeeId,
                        workDate: schedule[objectKey].workDate,
                        shiftId: schedule[objectKey].shift !== null ? schedule[objectKey].shift.id : null,
                        isHoliday: schedule[objectKey].isHoliday,
                        isOffDay: schedule[objectKey].isOffDay,
                        isPublic: schedule.isPublic
                    });
                });
            }
        }

        if (!isValidData && !this.isSARole) {
            toast.error('Some employees have a larger number of days off during the week according to the setting.');

            return;
        }

        if (weeklySchedules.length > 0 || defaultShifts.length > 0) {
            axios({
                url: _global.root + 'ScheduleArrange/EditWeekly',
                method: 'POST',
                data: stringify({ weeklySchedules, defaultShifts }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success('Updated Successfully!');

                    this.getWeeklyShedules(this.state.filter);
                } else {
                    toast.error(data.message);
                }
            });
        }
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

    handleShowConfirmRefreshDataModal = () => {
        this.setState({ showConfirmRefreshDataModal: true });
    }

    handleCloseConfirmRefreshDataModal = () => {
        this.setState({ showConfirmRefreshDataModal: false });
    }

    handleSubmitRefreshData = () => {
        this.getWeeklyShedules(this.state.filter);
        this.handleCloseConfirmRefreshDataModal();
    }

    handleChangeShowRemarkPopover = show => {
        this.setState({ showRemarkPopover: show });
    }

    handleUpdateRemark = (employeeId, remark) => {
        axios({
            url: _global.root + 'ScheduleArrange/EditRemark',
            method: 'POST',
            data: stringify({ employeeId, remark }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated Successfully!');

                let schedules = cloneDeep(this.state.schedules);
                const index = schedules.findIndex(schedule => schedule.employeeId === employeeId);

                if (index >= 0) {
                    schedules[index].remark = remark;
                    this.originalSchedules[index].remark = remark;

                    this.setState({ schedules });
                }
            } else {
                toast.error(data.message);
            }
        });
    }

    handleChangeOrderMode = isChecked => {
        if (isChecked) {
            if (this.isDirtyData()) {
                this.setState({ showConfirmSwitchOrderModeModal: true });
            } else {
                this.handleConfirmSwitchOrderMode();
            }
        } else {
            this.setState({ isOrderMode: false }, () => this.getWeeklyShedules(this.state.filter));
        }
    }

    handleCloseConfirmSwitchOrderModeModal = () => {
        this.setState({ showConfirmSwitchOrderModeModal: false });
    }

    handleConfirmSwitchOrderMode = () => {
        const filter = Object.assign({}, this.state.filter, {
            groupIds: this.groupOptions.map(group => group.id).join(','),
            employee: { value: 0, label: '' }
        });
        this.setState({ isOrderMode: true }, () => this.getWeeklyShedules(filter));
    }

    handleEditDisplayOrder = () => {
        if (this.isDirtyData()) {
            let displayOrders = [];
            let schedules = cloneDeep(this.state.schedules);
            const totalItems = schedules.length;

            for (let index = 0; index < totalItems; index++) {
                const schedule = schedules[index];
                displayOrders.push({
                    employeeId: schedule.employeeId,
                    displayOrder: schedule.displayOrder
                });
            }

            if (displayOrders.length > 0) {
                axios({
                    url: _global.root + 'ScheduleArrange/EditDisplayOrder',
                    method: 'POST',
                    data: stringify({ displayOrders }),
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest',
                        'Content-Type': 'application/x-www-form-urlencoded'
                    }
                }).then(response => {
                    let data = response.data;

                    if (data.isSuccessful) {
                        this.originalSchedules = schedules;
                    } else {
                        toast.error(data.message);
                    }
                });
            }
        }
    }

    reorder = (schedules, startIndex, endIndex) => {
        const [removed] = schedules.splice(startIndex, 1);
        schedules.splice(endIndex, 0, removed);

        return schedules;
    }

    handleDragEnd = result => {
        if (!result.destination) {
            return;
        }

        let schedules = this.reorder(cloneDeep(this.state.schedules), result.source.index, result.destination.index);
        schedules = schedules.map((schedule, index) => {
            schedule.displayOrder = index + 1;

            return schedule;
        });

        this.setState({ schedules }, () => this.handleEditDisplayOrder());
    }

    render() {
        let isHost = this.isHost;
        const {
            showShiftMultipleModal,
            showConfirmRefreshDataModal,
            showRemarkPopover,
            isOrderMode,
            showConfirmSwitchOrderModeModal,
            schedules
        } = this.state;

        if (moment(this.selectedWeek.toDate).isBefore(moment(this.currentWeek.fromDate).add(-28, 'days')) && !this.isSARole) {
            isHost = false;
        }

        return (
            <Fragment>
                <ToastLayout />
                <Search
                    isOrderMode={isOrderMode}
                    groupOptions={this.groupOptions}
                    employeeOptions={this.state.employeeOptions}
                    filter={this.state.filter}
                    onSearchEmployees={this.getEmployees}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleGetWeeklyShedules}
                />
                <Table
                    isHost={isHost}
                    isOrderMode={isOrderMode}
                    fromDate={this.selectedWeek.fromDate}
                    toDate={this.selectedWeek.toDate}
                    data={schedules}
                    shiftOptions={this.shiftOptions}
                    showRemarkPopover={showRemarkPopover}
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
                    onChangeShowRemarkPopover={this.handleChangeShowRemarkPopover}
                    onUpdateRemark={this.handleUpdateRemark}
                    onChangeOrderMode={this.handleChangeOrderMode}
                    onDragEnd={this.handleDragEnd}
                    onSave={this.handleSave}
                />
                {
                    showShiftMultipleModal &&
                    <ShiftMultipleModal show={showShiftMultipleModal}
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
                {
                    showConfirmRefreshDataModal &&
                    <ConfirmRefreshDataModal show={showConfirmRefreshDataModal}
                        onHide={this.handleCloseConfirmRefreshDataModal}
                        onSubmit={this.handleSubmitRefreshData}
                    />
                }
                {
                    showConfirmSwitchOrderModeModal &&
                    <ConfirmRefreshDataModal show={showConfirmSwitchOrderModeModal}
                        onHide={this.handleCloseConfirmSwitchOrderModeModal}
                        onSubmit={this.handleConfirmSwitchOrderMode}
                    />
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));