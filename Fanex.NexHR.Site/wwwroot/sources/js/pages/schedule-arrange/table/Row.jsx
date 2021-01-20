import React, { PureComponent } from 'react';
import moment from 'moment';

import { DateTimeFormat, OffDayShiftId } from '../../../shared/constants';
import ColIndex from './ColIndex';
import ColNickname from './ColNickname';
import ColDefaultShift from './ColDefaultShift';
import ColShift from './ColShift';

export default class Row extends PureComponent {
    handleChangeSelectWeekDay = (attrName, isChecked) => {
        const { teamId, data } = this.props;
        const employeeId = data.employeeId;

        this.props.onChangeSelectWeekDay(teamId, employeeId, attrName, isChecked);
    };

    handleChangeShift = (name, shift) => {
        const { teamId, data } = this.props;
        const employeeId = data.employeeId;

        this.props.onChangeShift(teamId, employeeId, name, shift);
    };

    render() {
        const { isHost, shiftOptions, teamId, data, showRemarkPopover, draggableProvided, isDragging } = this.props;

        return (
            <div className={`table-row ${isDragging ? 'dragging' : ''}`}
                ref={draggableProvided.innerRef} {...draggableProvided.draggableProps} {...draggableProvided.dragHandleProps}
            >
                <ColIndex data={data} />
                <ColNickname isHost={isHost}
                    teamId={teamId} employeeId={data.employeeId} nickname={data.nickname} remark={data.remark}
                    showRemarkPopover={showRemarkPopover}
                    onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                    onShowWeeklyDefaultShift={this.props.onShowWeeklyDefaultShift}
                    onUpdateRemark={this.props.onUpdateRemark}
                    onChangeOrder={this.props.onChangeOrder}
                />
                <ColDefaultShift isHost={isHost} teamId={teamId} employeeId={data.employeeId}
                    isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift} data={data.defaultShift}
                    shiftOptions={shiftOptions.filter(shift => shift.id !== OffDayShiftId)}
                    onChangeDefaultShift={this.props.onChangeDefaultShift}
                />
                <ColShift isHost={isHost} shiftOptions={shiftOptions} isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.mondaySchedule}
                    scheduleAttrName={'mondaySchedule'}
                    isChecked={data.selectWeekDays.mondaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                />
                <ColShift isHost={isHost} shiftOptions={shiftOptions} isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.tuesdaySchedule}
                    scheduleAttrName={'tuesdaySchedule'}
                    isChecked={data.selectWeekDays.tuesdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                />
                <ColShift isHost={isHost} shiftOptions={shiftOptions} isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.wednesdaySchedule}
                    scheduleAttrName={'wednesdaySchedule'}
                    isChecked={data.selectWeekDays.wednesdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                />
                <ColShift isHost={isHost} shiftOptions={shiftOptions} isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.thursdaySchedule}
                    scheduleAttrName={'thursdaySchedule'}
                    isChecked={data.selectWeekDays.thursdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                />
                <ColShift isHost={isHost} shiftOptions={shiftOptions} isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.fridaySchedule}
                    scheduleAttrName={'fridaySchedule'}
                    isChecked={data.selectWeekDays.fridaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                />
                <ColShift isHost={isHost} shiftOptions={shiftOptions} isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.saturdaySchedule}
                    scheduleAttrName={'saturdaySchedule'}
                    isChecked={data.selectWeekDays.saturdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                />
                <ColShift isHost={isHost} shiftOptions={shiftOptions} isUsedWeeklyDefaultShift={data.isUsedWeeklyDefaultShift}
                    employeeId={data.employeeId} defaultShift={data.defaultShift} data={data.sundaySchedule}
                    scheduleAttrName={'sundaySchedule'}
                    isChecked={data.selectWeekDays.sundaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onChangeSelectWeekDay={this.handleChangeSelectWeekDay}
                />
                <div className="table-cell cell-modifier">
                    {
                        data.modifier &&
                        <span title={moment(data.modifiedDate).format(DateTimeFormat)}>
                            {data.modifier}
                        </span>
                    }
                </div>
            </div>
        );
    }
}