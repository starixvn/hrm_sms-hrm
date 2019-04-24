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
            isHost: false,
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
            isOrderMode: false
        };

        this.originalSchedules = [];
        this.groupOptions = [];
        this.shiftOptions = [];
        this.currentWeek = { fromDate, toDate };
        this.selectedWeek = { fromDate, toDate };
        this.isSARole = document.getElementById('IsSARole').checked;
        this.scheduleAttrNames = ['mondaySchedule', 'tuesdaySchedule', 'wednesdaySchedule',
            'thursdaySchedule', 'fridaySchedule', 'saturdaySchedule', 'sundaySchedule'];
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

        outerloop:
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const originalGroup = originalSchedules.find(item => item.groupId === schedules[gindex].groupId);
            const totalSubItems = schedules[gindex].members.length;

            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                let schedule = schedules[gindex].members[eindex];
                let originalSchedule = originalGroup.members.find(item => item.employeeId === schedule.employeeId);
                unset(schedule, 'selectWeekDays');
                unset(originalSchedule, 'selectWeekDays');

                if (!isEqual(schedule, originalSchedule)) {
                    isDirty = true;
                    break outerloop;
                }
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
                employeeId: filter.employee.value === 0 ? null : filter.employee.value
            }
        }).then(response => {
            this.selectedWeek = { fromDate, toDate };
            let { isHost, schedules } = response.data;

            this.originalSchedules = schedules.map((group, gindex) => {
                group.displayOrder = gindex + 1;

                group.members.map((schedule, eindex) => {
                    schedule.displayOrder = eindex + 1;
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

                return group;
            });

            this.setState({ isHost, schedules: cloneDeep(this.originalSchedules) });
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

    handleChangeSelectWeekDay = (groupId, employeeId, scheduleAttrName, isChecked) => {
        let schedules = this.state.schedules.slice();
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = schedules[gindex].members.slice();
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        members[eindex] = Object.assign({}, members[eindex], {
            selectWeekDays: { ...members[eindex].selectWeekDays, [scheduleAttrName]: isChecked }
        });
        schedules[gindex] = { ...schedules[gindex], members };

        this.setState({ schedules });
    }

    handleChangeSelectWeekDays = (scheduleAttrName, isChecked) => {
        let schedules = this.state.schedules.slice();
        const totalItems = schedules.length;

        if (scheduleAttrName !== null) {
            for (let gindex = 0; gindex < totalItems; gindex++) {
                let members = schedules[gindex].members.slice();
                const totalSubItems = members.length;
                for (let subindex = 0; subindex < totalSubItems; subindex++) {
                    members[subindex] = Object.assign({}, members[subindex], {
                        selectWeekDays: { ...members[subindex].selectWeekDays, [scheduleAttrName]: isChecked }
                    });
                }
                schedules[gindex] = { ...schedules[gindex], members };
            }
        } else {
            for (let gindex = 0; gindex < totalItems; gindex++) {
                let members = schedules[gindex].members.slice();
                const totalSubItems = members.length;
                for (let subindex = 0; subindex < totalSubItems; subindex++) {
                    members[subindex] = Object.assign({}, members[subindex], {
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
                schedules[gindex] = { ...schedules[gindex], members };
            }
        }

        this.setState({ schedules });
    }

    handleChangeShift = (groupId, employeeId, attrName, shift) => {
        let schedules = this.state.schedules.slice();
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = schedules[gindex].members.slice();
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        let member = { ...members[eindex] };

        if (shift !== null) {
            const isOffDay = shift.id === -1;

            if (isOffDay) {
                if (!member[attrName].isHoliday) {
                    member[attrName] = { ...member[attrName], isOffDay: true, shift: null };
                } else {
                    member[attrName] = { ...member[attrName], shift: null };
                }
            } else {
                member[attrName] = { ...member[attrName], isOffDay: false, shift: shift };
            }
        } else {
            member[attrName] = { ...member[attrName], isOffDay: false, shift: null };
        }

        members[eindex] = member;
        schedules[gindex] = { ...schedules[gindex], members };

        this.setState({ schedules });
    }

    handleChangeDefaultShift = (groupId, employeeId, defaultShift) => {
        let schedules = this.state.schedules.slice();
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = schedules[gindex].members.slice();
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        members[eindex] = { ...members[eindex], defaultShift };
        schedules[gindex] = { ...schedules[gindex], members };

        this.setState({ schedules });
    }

    handleChangePublics = (groupId, employeeId, isChecked) => {
        let schedules = this.state.schedules.slice();

        if (groupId !== null && employeeId !== null) {
            const gindex = schedules.findIndex(item => item.groupId === groupId);
            let members = schedules[gindex].members.slice();
            const eindex = members.findIndex(item => item.employeeId === employeeId);
            members[eindex] = Object.assign({}, members[eindex], { isPublic: isChecked });
            schedules[gindex] = { ...schedules[gindex], members };

            this.setState({ schedules });
        } else {
            const totalItems = schedules.length;
            for (let gindex = 0; gindex < totalItems; gindex++) {
                let members = schedules[gindex].members.slice();
                const totalSubItems = members.length;
                for (let subindex = 0; subindex < totalSubItems; subindex++) {
                    members[subindex] = Object.assign({}, members[subindex], { isPublic: isChecked });
                }
                schedules[gindex] = { ...schedules[gindex], members };
            }

            this.setState({ schedules });
        }
    }

    getSelectedMembers = schedules => {
        let selectedMembers = [];
        let totalItems = schedules.length;
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                if (this.scheduleAttrNames.filter(attrName => schedules[gindex].members[eindex].selectWeekDays[attrName]).length > 0) {
                    selectedMembers.push({ gindex, eindex });
                }
            }
        }

        return selectedMembers;
    }

    handleChangeOffNoShift = () => {
        let schedules = this.state.schedules.slice();
        const selectedMembers = this.getSelectedMembers(schedules);
        const totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const selectedMember = selectedMembers[index];
                let members = schedules[selectedMember.gindex].members.slice();
                let member = { ...members[selectedMember.eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName] && !member[attrName].isHoliday) {
                        member[attrName] = { ...member[attrName], isOffDay: true, shift: null };
                    }
                });

                members[selectedMember.eindex] = member;
                schedules[selectedMember.gindex] = { ...schedules[selectedMember.gindex], members };
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
        const selectedMembers = this.getSelectedMembers(schedules);
        const totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const selectedMember = selectedMembers[index];
                let members = schedules[selectedMember.gindex].members.slice();
                let member = { ...members[selectedMember.eindex] };
                const selectWeekDays = member.selectWeekDays;
                let isChanged = false;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName] && !member[attrName].isHoliday && member[attrName].shift !== null && !member[attrName].isOffDay) {
                        member[attrName] = { ...member[attrName], isOffDay: true };
                        isChanged = true;
                    }
                });

                if (isChanged) {
                    members[selectedMember.eindex] = member;
                    schedules[selectedMember.gindex] = { ...schedules[selectedMember.gindex], members };
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
        const selectedMembers = this.getSelectedMembers(schedules);
        const totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const selectedMember = selectedMembers[index];
                let members = schedules[selectedMember.gindex].members.slice();
                let member = { ...members[selectedMember.eindex] };
                const selectWeekDays = member.selectWeekDays;
                let isChanged = false;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName] && !member[attrName].isHoliday && member[attrName].isOffDay) {
                        member[attrName] = { ...member[attrName], isOffDay: false };
                        isChanged = true;
                    }
                });

                if (isChanged) {
                    members[selectedMember.eindex] = member;
                    schedules[selectedMember.gindex] = { ...schedules[selectedMember.gindex], members };
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
        const selectedMembers = this.getSelectedMembers(this.state.schedules);
        const totalItems = selectedMembers.length;

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
        const selectedMembers = this.getSelectedMembers(schedules);
        const totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const selectedMember = selectedMembers[index];
                let members = schedules[selectedMember.gindex].members.slice();
                let member = { ...members[selectedMember.eindex] };
                const selectWeekDays = member.selectWeekDays;
                let isChanged = false;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName] && !member[attrName].isHoliday) {
                        member[attrName] = {
                            ...member[attrName],
                            isOffDay: shift !== null && shift.id === -1 ? true : isOffday,
                            shift
                        };
                        isChanged = true;
                    }
                });

                if (isChanged) {
                    members[selectedMember.eindex] = member;
                    schedules[selectedMember.gindex] = { ...schedules[selectedMember.gindex], members };
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

        outerloop:
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const originalGroup = originalSchedules.find(item => item.groupId === schedules[gindex].groupId);
            const totalSubItems = schedules[gindex].members.length;

            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                let schedule = schedules[gindex].members[eindex];
                let originalSchedule = originalGroup.members.find(item => item.employeeId === schedule.employeeId);
                unset(schedule, 'selectWeekDays');
                unset(originalSchedule, 'selectWeekDays');

                if (!isEqual(schedule, originalSchedule)) {
                    let totalOffDays = 0;
                    this.scheduleAttrNames.forEach(attrName => {
                        if (schedule[attrName].isOffDay || schedule[attrName].isLeaveOffDay) {
                            totalOffDays++;
                        }
                    });

                    if (schedule.offDaysbyWeek < totalOffDays && !this.isSARole) {
                        isValidData = false;
                        break outerloop;
                    }

                    defaultShifts.push({
                        employeeId: schedule.employeeId,
                        fromDate: schedule.mondaySchedule.workDate,
                        shiftId: schedule.defaultShift !== null ? schedule.defaultShift.id : null
                    });

                    this.scheduleAttrNames.forEach(attrName => {
                        weeklySchedules.push({
                            employeeId: schedule.employeeId,
                            workDate: schedule[attrName].workDate,
                            shiftId: schedule[attrName].shift !== null ? schedule[attrName].shift.id : null,
                            isHoliday: schedule[attrName].isHoliday,
                            isOffDay: schedule[attrName].isOffDay,
                            isPublic: schedule.isPublic
                        });
                    });
                }
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

    handleUpdateRemark = (groupId, employeeId, remark) => {
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

                let schedules = this.state.schedules.slice();
                const gindex = schedules.findIndex(item => item.groupId === groupId);
                let members = schedules[gindex].members.slice();
                const eindex = members.findIndex(item => item.employeeId === employeeId);
                members[eindex] = { ...members[eindex], remark };
                schedules[gindex] = { ...schedules[gindex], members };

                this.originalSchedules[gindex].members[eindex].remark = remark;
                this.setState({ schedules });
            } else {
                toast.error(data.message);
            }
        });
    }

    handleChangeOrderMode = isChecked => {
        this.setState({ isOrderMode: isChecked });
    }

    handleEditGroupOrder = () => {
        let displayOrders = [];
        let schedules = this.state.schedules;
        const totalItems = schedules.length;

        for (let index = 0; index < totalItems; index++) {
            displayOrders.push({
                groupId: schedules[index].groupId,
                displayOrder: schedules[index].displayOrder
            });
        }

        if (displayOrders.length > 0) {
            axios({
                url: _global.root + 'ScheduleArrange/EditGroupOrder',
                method: 'POST',
                data: stringify({ displayOrders }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    this.originalSchedules = cloneDeep(schedules);
                } else {
                    toast.error(data.message);
                }
            });
        }
    }

    handleEditDisplayOrder = () => {
        let displayOrders = [];
        let schedules = this.state.schedules;
        const totalItems = schedules.length;

        for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                displayOrders.push({
                    employeeId: schedules[gindex].members[eindex].employeeId,
                    displayOrder: schedules[gindex].members[eindex].displayOrder
                });
            }
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
                    this.originalSchedules = cloneDeep(schedules);
                } else {
                    toast.error(data.message);
                }
            });
        }
    }

    reorder = (schedules, startIndex, endIndex) => {
        const [removed] = schedules.splice(startIndex, 1);
        schedules.splice(endIndex, 0, removed);

        return schedules;
    }

    handleDragEnd = result => {
        if (!result.destination || result.source.index === result.destination.index) {
            return;
        }

        const resultType = result.type;
        let weeklySchedules = this.state.schedules.slice();

        if (resultType === 'droppableSection') {
            let schedules = this.reorder(weeklySchedules, result.source.index, result.destination.index);
            this.setState({ schedules }, () => this.handleEditGroupOrder());
        } else if (resultType.includes('droppableMember')) {
            const groupId = parseInt(resultType.split('-')[1]);
            const index = weeklySchedules.findIndex(x => x.groupId === groupId);
            weeklySchedules[index].members = this.reorder(weeklySchedules[index].members.slice(), result.source.index, result.destination.index);
            this.setState({ schedules: weeklySchedules }, () => this.handleEditDisplayOrder());
        }
    }

    handleChangeSectionOrder = (key, groupId) => {
        let weeklySchedules = this.state.schedules.slice();
        const sourceIndex = weeklySchedules.findIndex(x => x.groupId === groupId);
        const totalItems = weeklySchedules.length;
        let destinationIndex = 0;

        switch (key) {
            case 'up':
                destinationIndex = sourceIndex - 1;
                break;
            case 'down':
                destinationIndex = sourceIndex + 1;
                break;
            case 'top':
                destinationIndex = 0;
                break;
            case 'bottom':
                destinationIndex = totalItems;
                break;
        }

        if (destinationIndex < 0 || destinationIndex >= totalItems || sourceIndex === destinationIndex) {
            return;
        }

        let schedules = this.reorder(weeklySchedules, sourceIndex, destinationIndex);
        schedules = schedules.map((schedule, index) => {
            schedule.displayOrder = index + 1;
            return schedule;
        });

        this.setState({ schedules }, () => this.handleEditGroupOrder());
    }

    handleChangeOrder = (key, groupId, employeeId) => {
        let weeklySchedules = this.state.schedules.slice();
        const index = weeklySchedules.findIndex(x => x.groupId === groupId);
        const totalItems = weeklySchedules[index].members.length;
        let sourceIndex = weeklySchedules[index].members.findIndex(schedule => schedule.employeeId === employeeId);
        let destinationIndex = 0;

        switch (key) {
            case 'up':
                destinationIndex = sourceIndex - 1;
                break;
            case 'down':
                destinationIndex = sourceIndex + 1;
                break;
            case 'top':
                destinationIndex = 0;
                break;
            case 'bottom':
                destinationIndex = totalItems;
                break;
        }

        if (destinationIndex < 0 || destinationIndex >= totalItems || sourceIndex === destinationIndex) {
            return;
        }

        let schedules = this.reorder(weeklySchedules[index].members.slice(), sourceIndex, destinationIndex);
        weeklySchedules[index].members = schedules.map((schedule, index) => {
            schedule.displayOrder = index + 1;
            return schedule;
        });

        this.setState({ schedules: weeklySchedules }, () => this.handleEditDisplayOrder());
    }

    render() {
        const {
            isHost,
            schedules,
            showShiftMultipleModal,
            showConfirmRefreshDataModal,
            showRemarkPopover,
            isOrderMode
        } = this.state;

        return (
            <Fragment>
                <ToastLayout />
                <Search
                    groupOptions={this.groupOptions}
                    employeeOptions={this.state.employeeOptions}
                    filter={this.state.filter}
                    onSearchEmployees={this.getEmployees}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleGetWeeklyShedules}
                />
                <Table
                    isHost={isHost} isOrderMode={isOrderMode}
                    fromDate={this.selectedWeek.fromDate} toDate={this.selectedWeek.toDate}
                    data={schedules}
                    shiftOptions={this.shiftOptions} showRemarkPopover={showRemarkPopover}
                    onPrevious={this.handleGetPreviousWeekSchedule}
                    onNext={this.handleGetNextWeekSchedule}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                    onChangeSelectWeekDays={this.handleChangeSelectWeekDays}
                    onChangeShift={this.handleChangeShift}
                    onChangeDefaultShift={this.handleChangeDefaultShift}
                    onChangePublics={this.handleChangePublics}
                    onChangeOffNoShift={this.handleChangeOffNoShift}
                    onChangeOffdayOT={this.handleChangeOffdayOT}
                    onChangeWorkday={this.handleChangeWorkday}
                    onShiftMultiple={this.handleShiftMultiple}
                    onChangeShowRemarkPopover={this.handleChangeShowRemarkPopover}
                    onUpdateRemark={this.handleUpdateRemark}
                    onChangeOrderMode={this.handleChangeOrderMode}
                    onDragEnd={this.handleDragEnd}
                    onChangeSectionOrder={this.handleChangeSectionOrder}
                    onChangeOrder={this.handleChangeOrder}
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
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));