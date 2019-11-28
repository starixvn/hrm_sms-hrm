import 'jquery-ui-bundle/jquery-ui';
import '../../jquery-weekpicker';

import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { toast } from 'react-toastify';
import axios from 'axios';
import moment from 'moment';
import { stringify } from 'qs';
import { cloneDeep, isEqual, unset } from 'lodash';

import { axiosError, blockUI, unblockUI, reorder, resolveClientUrl } from '../../shared/common.functions';
import { getTotalOffDays } from './common.functions';
import { DateFormat, OffDayShiftId } from '../../shared/constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import buildData from '../../shared/components/react-tree-multiselect/buildData';
import ConfirmRefreshDataModal from './ConfirmRefreshDataModal';
import ShiftMultipleModal from './ShiftMultipleModal';
import WeeklyDefaultShiftModal from './WeeklyDefaultShiftModal';
import Search from './Search';
import Table from './table/table';

export default class App extends Component {
    constructor(props) {
        super(props);

        const fromDate = moment().day('Monday').format(DateFormat),
            toDate = moment(fromDate).add(6, 'days').format(DateFormat);

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
            showRemarkPopover: false,
            editWeeklyDefaultShift: {
                show: false,
                groupId: 0,
                employeeId: 0,
                nickname: '',
                data: null,
                isUsedWeeklyDefaultShift: false
            }
        };

        this.originalSchedules = [];
        this.groupOptions = [];
        this.shiftOptions = [];
        this.currentWeek = { fromDate, toDate };
        this.selectedWeek = { fromDate, toDate };
        this.scheduleAttrNames = ['mondaySchedule', 'tuesdaySchedule', 'wednesdaySchedule',
            'thursdaySchedule', 'fridaySchedule', 'saturdaySchedule', 'sundaySchedule'];
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('department/GetHierarchy')),
            axios(resolveClientUrl('Calendar/GetShift'), {
                params: {
                    query: '',
                    isShowOff: true
                }
            })
        ];

        Promise.all(promises)
            .then(([groupResponse, shiftResponse]) => {
                this.groupOptions = groupResponse.data;
                const { floatData } = buildData(groupResponse.data);
                this.shiftOptions = shiftResponse.data.map(shift => { return { id: shift.Id, code: shift.Code, name: shift.Name }; });
                const filter = { ...this.state.filter, groupIds: floatData.filter(item => item.isLeaf).map(item => item.id).join(',') };

                this.getWeeklyShedules(filter);

                this.setState({ filter: filter });
            }).catch(axiosError);

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
        const schedules = cloneDeep(this.state.schedules),
            originalSchedules = cloneDeep(this.originalSchedules),
            totalItems = schedules.length;

        outerloop:
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;
            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                let schedule = schedules[gindex].members[eindex],
                    originalSchedule = originalSchedules[gindex].members[eindex];
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
        this.setState({ filter: { ...this.state.filter, [name]: value } });
    }

    getEmployees = query => {
        axios(resolveClientUrl('ScheduleArrange/GetEmployees'), {
            params: {
                groupIds: this.state.filter.groupIds,
                query: query
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
        const fromDate = moment(filter.fromDate).format(DateFormat),
            toDate = moment(filter.toDate).format(DateFormat);
        blockUI();

        axios(resolveClientUrl('ScheduleArrange/GetWeeklySchedule'), {
            params: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: filter.groupIds,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value
            }
        }).then(response => {
            this.selectedWeek = { fromDate, toDate };
            let { isHost, schedules } = response.data;

            this.originalSchedules = schedules.map(group => {
                group.members.map(schedule => {
                    schedule.selectWeekDays = {
                        mondaySchedule: false,
                        tuesdaySchedule: false,
                        wednesdaySchedule: false,
                        thursdaySchedule: false,
                        fridaySchedule: false,
                        saturdaySchedule: false,
                        sundaySchedule: false
                    };
                    schedule.totalOffDayReplacement = 0;

                    Object.keys(schedule.selectWeekDays).forEach(objectKey => {
                        if (schedule[objectKey].isOffDayReplacement) {
                            schedule.totalOffDayReplacement = schedule.totalOffDayReplacement + 1;
                        }
                    });

                    return schedule;
                });

                return group;
            });

            this.setState({
                filter: { ...this.state.filter, fromDate, toDate },
                isHost,
                schedules: cloneDeep(this.originalSchedules)
            });
        }).catch(axiosError).then(() => unblockUI());
    }

    handleExportExcel = () => {
        const filter = this.state.filter,
            fromDate = moment(filter.fromDate).format('DD-MMM-YYYY'),
            toDate = moment(filter.toDate).format('DD-MMM-YYYY');

        axios(resolveClientUrl('ScheduleArrange/ExportExcel'), {
            params: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: filter.groupIds,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value
            },
            responseType: 'blob'
        }).then(response => {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(new Blob([response.data]));
            link.setAttribute('download', `Working shift ${fromDate} to ${toDate}.xlsx`);
            document.body.appendChild(link);
            link.click();
        }).catch(axiosError);
    }

    handleExportExcelDriverPickup = () => {
        const filter = this.state.filter,
            fromDate = moment(filter.fromDate).format('DD-MMM-YYYY'),
            toDate = moment(filter.toDate).format('DD-MMM-YYYY');

        axios(resolveClientUrl('ScheduleArrange/ExportExcelDriverPickup'), {
            params: {
                fromDate: fromDate,
                toDate: toDate,
                groupIds: filter.groupIds,
                employeeId: filter.employee.value === 0 ? null : filter.employee.value
            },
            responseType: 'blob'
        }).then(response => {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(new Blob([response.data]));
            link.setAttribute('download', `Schedule for Driver ${fromDate} to ${toDate}.xlsx`);
            document.body.appendChild(link);
            link.click();
        }).catch(axiosError);
    }

    handleGetWeeklyShedules = isCurrentWeek => {
        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            const filter = isCurrentWeek
                ? { ...this.state.filter, fromDate: this.currentWeek.fromDate, toDate: this.currentWeek.toDate }
                : this.state.filter;

            this.setState({ filter });

            this.getWeeklyShedules(filter);
        }
    }

    handleGetPreviousWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate).add(-7, 'days').format(DateFormat),
            toDate = moment(this.selectedWeek.toDate).add(-7, 'days').format(DateFormat),
            filter = { ...this.state.filter, fromDate, toDate };
        this.setState({ filter });

        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            this.getWeeklyShedules(filter);
        }
    }

    handleGetNextWeekSchedule = () => {
        const fromDate = moment(this.selectedWeek.fromDate).add(7, 'days').format(DateFormat),
            toDate = moment(this.selectedWeek.toDate).add(7, 'days').format(DateFormat),
            filter = { ...this.state.filter, fromDate, toDate };
        this.setState({ filter });

        if (this.isDirtyData()) {
            this.handleShowConfirmRefreshDataModal();
        } else {
            this.getWeeklyShedules(filter);
        }
    }

    handleChangeSelectWeekDay = (groupId, employeeId, scheduleAttrName, isChecked) => {
        let schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        members[eindex] = Object.assign({}, members[eindex], {
            selectWeekDays: { ...members[eindex].selectWeekDays, [scheduleAttrName]: isChecked }
        });
        schedules[gindex] = { ...schedules[gindex], members };

        this.setState({ schedules });
    }

    handleChangeSelectWeekDays = (scheduleAttrName, isChecked) => {
        let schedules = [...this.state.schedules];
        const totalItems = schedules.length;

        if (scheduleAttrName !== null) {
            for (let gindex = 0; gindex < totalItems; gindex++) {
                let members = [...schedules[gindex].members];
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
                let members = [...schedules[gindex].members];
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
        let schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        let member = { ...members[eindex] };

        if (shift !== null && isEqual(member[attrName].shift, shift)) {
            return;
        }

        let defaultShift = member.defaultShift !== null ? { ...member.defaultShift } : null;
        if (member.isUsedWeeklyDefaultShift) {
            defaultShift = member.weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`];
        }

        shift = shift === null && !member[attrName].isHoliday ? defaultShift : shift;
        const isOffDay = shift !== null && shift.id === OffDayShiftId;
        shift = shift !== null && shift.id === OffDayShiftId ? null : shift;
        const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName];

        member[attrName] = {
            ...member[attrName],
            isOffDay,
            isOffDayReplacement: false,
            shift,
            isModified: !isEqual(shift, originalSchedule.shift) ? true : originalSchedule.isModified
        };

        members[eindex] = member;
        schedules[gindex] = { ...schedules[gindex], members };
        this.setState({ schedules });
    }

    handleChangeDefaultShift = (groupId, employeeId, defaultShift) => {
        let schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId),
            originalDefaultShift = members[eindex].defaultShift;

        if (defaultShift !== null) {
            this.scheduleAttrNames.forEach(attrName => {
                if (!members[eindex][attrName].isDisabled) {
                    const shift = members[eindex][attrName].shift;
                    if (members[eindex][attrName].isHoliday || members[eindex][attrName].isOffDay) {
                        if (shift !== null && isEqual(originalDefaultShift, shift)) {
                            const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName],
                                isModified = !isEqual(defaultShift, originalSchedule.shift) ? true : originalSchedule.isModified;
                            members[eindex][attrName] = { ...members[eindex][attrName], shift: defaultShift, isModified };
                        }
                    } else {
                        if (shift === null || isEqual(originalDefaultShift, shift)) {
                            const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName],
                                isModified = !isEqual(defaultShift, originalSchedule.shift) ? true : originalSchedule.isModified;
                            members[eindex][attrName] = { ...members[eindex][attrName], shift: defaultShift, isModified };
                        }
                    }
                }
            });
        } else {
            this.scheduleAttrNames.forEach(attrName => {
                const shift = members[eindex][attrName].shift;
                if (!members[eindex][attrName].isModified && shift !== null && isEqual(originalDefaultShift, shift)) {
                    const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName],
                        isModified = !isEqual(defaultShift, originalSchedule.shift) ? true : originalSchedule.isModified;
                    members[eindex][attrName] = { ...members[eindex][attrName], shift: defaultShift, isModified };
                }
            });
        }

        members[eindex] = { ...members[eindex], defaultShift };
        schedules[gindex] = { ...schedules[gindex], members };

        this.setState({ schedules });
    }

    handleChangePublics = (groupId, isChecked) => {
        let schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = [...schedules[gindex].members];
        const totalItems = members.length;
        for (let index = 0; index < totalItems; index++) {
            members[index] = { ...members[index], isPublic: isChecked };
        }
        schedules[gindex] = { ...schedules[gindex], isPublic: isChecked, members };

        this.setState({ schedules });
    }

    getSelectedMembers = schedules => {
        let selectedMembers = [];
        const totalItems = schedules.length;
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
        let schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                let members = [...schedules[gindex].members];
                let member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = { ...member.selectWeekDays, [attrName]: false };
                        if (!member[attrName].isHoliday && !member[attrName].isChangeShift) {
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: true,
                                isFixedOffDay: false,
                                isOffDayReplacement: false,
                                shift: null
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to off!');
        }
    }

    handleChangeOffDayOT = () => {
        let schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                let members = [...schedules[gindex].members];
                let member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = { ...member.selectWeekDays, [attrName]: false };
                        if (!member[attrName].isHoliday && !member[attrName].isChangeShift && member[attrName].shift !== null) {
                            const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName];
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: true,
                                isFixedOffDay: false,
                                isOffDayReplacement: false,
                                isModified: !originalSchedule.isOffDay ? true : originalSchedule.isModified
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to off!');
        }
    }

    handleChangeWorkday = () => {
        let schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                let members = [...schedules[gindex].members];
                let member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = { ...member.selectWeekDays, [attrName]: false };
                        if (!member[attrName].isHoliday && member[attrName].shift !== null) {
                            const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName];
                            selectWeekDays[attrName] = false;
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: false,
                                isFixedOffDay: false,
                                isOffDayReplacement: false,
                                isModified: originalSchedule.isOffDay ? true : originalSchedule.isModified
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to work!');
        }
    }

    handleShiftMultiple = () => {
        const selectedMembers = this.getSelectedMembers(this.state.schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            this.setState({ showShiftMultipleModal: true });
        } else {
            toast.warn('Please choose at least one day to continue!');
        }
    }

    handleChangeMultipleShift = (shift, isOffDay) => {
        let schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                let members = [...schedules[gindex].members];
                let member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = { ...member.selectWeekDays, [attrName]: false };
                        if (!member[attrName].isHoliday) {
                            const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName];
                            const isModified = !isEqual(shift, originalSchedule.shift) || originalSchedule.isOffDay !== isOffDay
                                ? true : originalSchedule.isModified;
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: isOffDay && !member[attrName].isChangeShift,
                                isFixedOffDay: isOffDay ? member[attrName].isFixedOffDay : false,
                                shift,
                                isModified
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            this.setState({ schedules });
        }

        this.handleCloseShiftMultipleModal();
    }

    handleFixedOffDay = () => {
        let schedules = [...this.state.schedules];
        const selectedMembers = this.getSelectedMembers(schedules),
            totalItems = selectedMembers.length;

        if (totalItems > 0) {
            for (let index = 0; index < totalItems; index++) {
                const { gindex, eindex } = selectedMembers[index];
                let members = [...schedules[gindex].members];
                let member = { ...members[eindex] };
                const selectWeekDays = member.selectWeekDays;

                this.scheduleAttrNames.forEach(attrName => {
                    if (selectWeekDays[attrName]) {
                        member.selectWeekDays = { ...member.selectWeekDays, [attrName]: false };
                        if (!member[attrName].isHoliday && !member[attrName].isChangeShift && member[attrName].shift !== null) {
                            const originalSchedule = this.originalSchedules[gindex].members[eindex][attrName];
                            member[attrName] = {
                                ...member[attrName],
                                isOffDay: true,
                                isFixedOffDay: true,
                                isOffDayReplacement: false,
                                isModified: !originalSchedule.isOffDay || !originalSchedule.isFixedOffDay ? true : originalSchedule.isModified
                            };
                        }
                    }
                });

                members[eindex] = member;
                schedules[gindex] = { ...schedules[gindex], members };
            }

            this.setState({ schedules });
        } else {
            toast.warn('Please choose at least one day to continue!');
        }
    }

    handleSave = () => {
        let isValidData = true,
            weeklySchedules = [],
            defaultShifts = [],
            schedules = cloneDeep(this.state.schedules),
            originalSchedules = cloneDeep(this.originalSchedules);
        const totalItems = schedules.length,
            { fromDate, toDate } = this.selectedWeek;

        outerloop:
        for (let gindex = 0; gindex < totalItems; gindex++) {
            const totalSubItems = schedules[gindex].members.length;

            for (let eindex = 0; eindex < totalSubItems; eindex++) {
                let schedule = schedules[gindex].members[eindex],
                    originalSchedule = originalSchedules[gindex].members[eindex];
                unset(schedule, 'selectWeekDays');
                unset(originalSchedule, 'selectWeekDays');

                if (!isEqual(schedule, originalSchedule)) {
                    const totalOffDaysDefault = schedule.offDaysbyWeek + schedule.totalOffDayReplacement,
                        totalOffDays = getTotalOffDays(schedule);

                    if (totalOffDaysDefault < totalOffDays) {
                        isValidData = false;
                        break outerloop;
                    }

                    if (!isEqual(originalSchedule.defaultShift, schedule.defaultShift)) {
                        defaultShifts.push({
                            employeeId: schedule.employeeId,
                            fromDate: schedule.mondaySchedule.workDate,
                            shiftId: schedule.defaultShift !== null ? schedule.defaultShift.id : null
                        });
                    }

                    this.scheduleAttrNames.forEach(attrName => {
                        if (!isEqual(originalSchedule[attrName], schedule[attrName]) || originalSchedule.isPublic !== schedule.isPublic) {
                            weeklySchedules.push({
                                employeeId: schedule.employeeId,
                                workDate: schedule[attrName].workDate,
                                shiftId: schedule[attrName].shift !== null ? schedule[attrName].shift.id : null,
                                isHoliday: schedule[attrName].isHoliday,
                                isOffDay: schedule[attrName].isOffDay,
                                isFixedOffDay: schedule[attrName].isFixedOffDay,
                                isPublic: schedule.isPublic
                            });
                        }
                    });
                }
            }
        }

        if (!isValidData) {
            toast.error('Some employees have a larger number of days off during the week according to the setting.');

            return;
        }

        if (weeklySchedules.length > 0 || defaultShifts.length > 0) {
            blockUI();

            axios({
                url: resolveClientUrl('ScheduleArrange/EditWeekly'),
                method: 'POST',
                data: stringify({ fromDate, toDate, weeklySchedules, defaultShifts }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                const data = response.data;
                if (data.isSuccessful) {
                    toast.success('Updated Successfully!');

                    this.getWeeklyShedules(this.state.filter);
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError).then(() => unblockUI());
        }
    }

    handleCloseShiftMultipleModal = () => {
        this.setState({ showShiftMultipleModal: false });
    }

    handleShowConfirmRefreshDataModal = () => {
        this.setState({ showConfirmRefreshDataModal: true });
    }

    handleCloseConfirmRefreshDataModal = () => {
        const fromDate = moment(this.selectedWeek.fromDate).format(DateFormat),
            toDate = moment(this.selectedWeek.toDate).format(DateFormat),
            filter = { ...this.state.filter, fromDate, toDate };

        this.setState({ filter, showConfirmRefreshDataModal: false });
    }

    handleSubmitRefreshData = () => {
        this.getWeeklyShedules(this.state.filter);
        this.handleCloseConfirmRefreshDataModal();
    }

    handleChangeShowRemarkPopover = show => {
        this.setState({ showRemarkPopover: show });
    }

    handleShowWeeklyDefaultShiftModal = (groupId, employeeId) => {
        const schedules = this.state.schedules,
            gindex = schedules.findIndex(item => item.groupId === groupId),
            eindex = schedules[gindex].members.findIndex(item => item.employeeId === employeeId);

        this.setState({
            editWeeklyDefaultShift: {
                show: true,
                groupId,
                employeeId,
                nickname: schedules[gindex].members[eindex].nickname,
                data: schedules[gindex].members[eindex].weeklyDefaultShift,
                isUsedWeeklyDefaultShift: schedules[gindex].members[eindex].isUsedWeeklyDefaultShift
            }
        });
    }

    handleCloseWeeklyDefaultShiftModal = () => {
        this.setState({
            editWeeklyDefaultShift: {
                show: false,
                groupId: 0,
                employeeId: 0,
                nickname: '',
                data: null,
                isUsedWeeklyDefaultShift: false
            }
        });
    }

    updateWeeklyDefaultShift = (groupId, employeeId, weeklyDefaultShift, isUsedWeeklyDefaultShift) => {
        let schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);
        members[eindex] = { ...members[eindex], weeklyDefaultShift, isUsedWeeklyDefaultShift };
        this.originalSchedules[gindex].members[eindex] = {
            ...this.originalSchedules[gindex].members[eindex],
            weeklyDefaultShift,
            isUsedWeeklyDefaultShift
        };
        const fromDate = moment(this.currentWeek.toDate).add(1, 'days').format(DateFormat);

        if (isUsedWeeklyDefaultShift) {
            this.scheduleAttrNames.forEach(attrName => {
                const shiftId = weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`].id,
                    isOffDay = shiftId === OffDayShiftId,
                    shift = isOffDay ? null : weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`],
                    workDate = moment(members[eindex][attrName].workDate);

                if (workDate.isSameOrAfter(fromDate)
                    && !members[eindex][attrName].isDisabled) {
                    members[eindex][attrName] = { ...members[eindex][attrName], shift, isOffDay };
                    this.originalSchedules[gindex].members[eindex][attrName] =
                        { ...this.originalSchedules[gindex].members[eindex][attrName], shift, isOffDay };
                }
            });
        } else {
            const defaultShift = members[eindex].defaultShift;

            if (defaultShift !== null) {
                this.scheduleAttrNames.forEach(attrName => {
                    const workDate = moment(members[eindex][attrName].workDate);
                    if (workDate.isSameOrAfter(fromDate)
                        && !members[eindex][attrName].isDisabled
                        && !members[eindex][attrName].isHoliday
                        && !members[eindex][attrName].isOffDay
                        && members[eindex][attrName].shift === null) {
                        members[eindex][attrName] = { ...members[eindex][attrName], shift: defaultShift };
                    }
                });
            }
        }

        schedules[gindex] = { ...schedules[gindex], members };
        this.setState({ schedules });
    }

    handleChangeWeeklyDefaultShift = (weeklyDefaultShift, isUsedWeeklyDefaultShift) => {
        const { groupId, employeeId } = this.state.editWeeklyDefaultShift;
        let schedules = this.state.schedules;
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = schedules[gindex].members;
        const eindex = members.findIndex(item => item.employeeId === employeeId);

        let totalOffDays = 0;
        this.scheduleAttrNames.forEach(attrName => {
            const shiftId = weeklyDefaultShift[`${attrName.substring(0, 3)}Shift`].id,
                isOffDay = shiftId === OffDayShiftId;

            if (isOffDay) {
                totalOffDays++;
            }
        });

        if (totalOffDays > members[eindex].offDaysbyWeek) {
            toast.error('Employee has a larger number of days off during the week according to the setting.');

            return;
        }

        axios({
            url: resolveClientUrl('ScheduleArrange/EditWeeklyDefaultShift'),
            method: 'POST',
            data: stringify({ weeklyDefaultShift, isUsedWeeklyDefaultShift }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            const data = response.data;
            if (data.isSuccessful) {
                toast.success('Updated Successfully!');
                this.updateWeeklyDefaultShift(groupId, employeeId, weeklyDefaultShift, isUsedWeeklyDefaultShift);

                this.handleCloseWeeklyDefaultShiftModal();
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    handleUpdateRemark = (groupId, employeeId, remark) => {
        let schedules = [...this.state.schedules];
        const gindex = schedules.findIndex(item => item.groupId === groupId);
        let members = [...schedules[gindex].members];
        const eindex = members.findIndex(item => item.employeeId === employeeId);

        if (!isEqual(remark, this.originalSchedules[gindex].members[eindex].remark)) {
            axios({
                url: resolveClientUrl('ScheduleArrange/EditRemark'),
                method: 'POST',
                data: stringify({ employeeId, remark }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                const data = response.data;
                if (data.isSuccessful) {
                    toast.success('Updated Successfully!');

                    members[eindex] = { ...members[eindex], remark };
                    schedules[gindex] = { ...schedules[gindex], members };
                    this.originalSchedules[gindex].members[eindex].remark = remark;

                    this.setState({ schedules });
                } else {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        }
    }

    handleEditGroupOrder = () => {
        let displayOrders = [];
        const schedules = this.state.schedules,
            totalItems = schedules.length;

        for (let index = 0; index < totalItems; index++) {
            displayOrders.push({
                groupId: schedules[index].groupId,
                displayOrder: index + 1
            });
        }

        if (displayOrders.length > 0) {
            axios({
                url: resolveClientUrl('ScheduleArrange/EditGroupOrder'),
                method: 'POST',
                data: stringify({ displayOrders }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                const data = response.data;
                if (!data.isSuccessful) {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        }
    }

    handleEditDisplayOrder = gindex => {
        let displayOrders = [];
        const members = this.state.schedules[gindex].members,
            totalItems = members.length;

        for (let index = 0; index < totalItems; index++) {
            displayOrders.push({
                employeeId: members[index].employeeId,
                displayOrder: index + 1
            });
        }

        if (displayOrders.length > 0) {
            axios({
                url: resolveClientUrl('ScheduleArrange/EditDisplayOrder'),
                method: 'POST',
                data: stringify({ displayOrders }),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                const data = response.data;
                if (!data.isSuccessful) {
                    toast.error(data.message);
                }
            }).catch(axiosError);
        }
    }

    handleDragEnd = result => {
        if (!result.destination || result.source.index === result.destination.index) {
            return;
        }

        const resultType = result.type,
            sourceIndex = result.source.index,
            destinationIndex = result.destination.index;
        let schedules = [...this.state.schedules];
        if (resultType === 'droppableSection') {
            schedules = reorder(schedules, sourceIndex, destinationIndex);
            this.originalSchedules = reorder(schedules, sourceIndex, destinationIndex);
            this.setState({ schedules }, () => this.handleEditGroupOrder());
        } else if (resultType.includes('droppableMember')) {
            const groupId = parseInt(resultType.split('-')[1], 10),
                index = schedules.findIndex(x => x.groupId === groupId);
            this.originalSchedules[index].members = reorder(this.originalSchedules[index].members, sourceIndex, destinationIndex);
            schedules[index].members = reorder(schedules[index].members, sourceIndex, destinationIndex);
            this.setState({ schedules }, () => this.handleEditDisplayOrder(index));
        }
    }

    handleChangeSectionOrder = (key, groupId) => {
        let schedules = [...this.state.schedules];
        const sourceIndex = schedules.findIndex(x => x.groupId === groupId),
            totalItems = schedules.length;
        let destinationIndex = 0;

        switch (key) {
            case 'up':
                destinationIndex = sourceIndex - 1;
                break;
            case 'down':
                destinationIndex = sourceIndex + 1;
                break;
            case 'bottom':
                destinationIndex = totalItems;
                break;
        }

        if (destinationIndex < 0 || destinationIndex > totalItems || sourceIndex === destinationIndex) {
            return;
        }

        schedules = reorder(schedules, sourceIndex, destinationIndex);
        this.originalSchedules = reorder(this.originalSchedules, sourceIndex, destinationIndex);
        this.setState({ schedules }, () => this.handleEditGroupOrder());
    }

    handleChangeOrder = (key, groupId, employeeId) => {
        let schedules = [...this.state.schedules];
        const index = schedules.findIndex(x => x.groupId === groupId),
            totalItems = schedules[index].members.length,
            sourceIndex = schedules[index].members.findIndex(schedule => schedule.employeeId === employeeId);
        let destinationIndex = 0;

        switch (key) {
            case 'up':
                destinationIndex = sourceIndex - 1;
                break;
            case 'down':
                destinationIndex = sourceIndex + 1;
                break;
            case 'bottom':
                destinationIndex = totalItems;
                break;
        }

        if (destinationIndex < 0 || destinationIndex > totalItems || sourceIndex === destinationIndex) {
            return;
        }

        schedules[index].members = reorder(schedules[index].members, sourceIndex, destinationIndex);
        this.originalSchedules[index].members = reorder(this.originalSchedules[index].members, sourceIndex, destinationIndex);
        this.setState({ schedules }, () => this.handleEditDisplayOrder(index));
    }

    render() {
        const {
            isHost,
            schedules,
            showShiftMultipleModal,
            showConfirmRefreshDataModal,
            showRemarkPopover,
            editWeeklyDefaultShift
        } = this.state;

        return (
            <Fragment>
                <ToastLayout />
                <Search
                    showExportExcel
                    groupOptions={this.groupOptions}
                    employeeOptions={this.state.employeeOptions}
                    filter={this.state.filter}
                    onSearchEmployees={this.getEmployees}
                    onChange={this.handleFilterChange}
                    onSubmit={this.handleGetWeeklyShedules}
                    onExportExcel={this.handleExportExcel}
                    onExportExcelDriverPickup={this.handleExportExcelDriverPickup}
                />
                <Table
                    isHost={isHost}
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
                    onChangeOffdayOT={this.handleChangeOffDayOT}
                    onChangeWorkday={this.handleChangeWorkday}
                    onShiftMultiple={this.handleShiftMultiple}
                    onFixedOffDay={this.handleFixedOffDay}
                    onChangeShowRemarkPopover={this.handleChangeShowRemarkPopover}
                    onShowWeeklyDefaultShift={this.handleShowWeeklyDefaultShiftModal}
                    onUpdateRemark={this.handleUpdateRemark}
                    onDragEnd={this.handleDragEnd}
                    onChangeSectionOrder={this.handleChangeSectionOrder}
                    onChangeOrder={this.handleChangeOrder}
                    onSave={this.handleSave}
                />
                {
                    showShiftMultipleModal &&
                    <ShiftMultipleModal show={showShiftMultipleModal}
                        onHide={this.handleCloseShiftMultipleModal}
                        shiftOptions={this.shiftOptions.filter(shift => shift.id !== OffDayShiftId)}
                        onSubmit={this.handleChangeMultipleShift}
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
                    editWeeklyDefaultShift.show &&
                    <WeeklyDefaultShiftModal show={editWeeklyDefaultShift.show}
                        isHost={isHost} nickname={editWeeklyDefaultShift.nickname}
                        data={editWeeklyDefaultShift.data} isUsedWeeklyDefaultShift={editWeeklyDefaultShift.isUsedWeeklyDefaultShift}
                        fromDate={moment(this.currentWeek.toDate).add(1, 'days').format(DateFormat)}
                        shiftOptions={this.shiftOptions}
                        onHide={this.handleCloseWeeklyDefaultShiftModal}
                        onSubmit={this.handleChangeWeeklyDefaultShift}
                    />
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('schedule-wrapper'));