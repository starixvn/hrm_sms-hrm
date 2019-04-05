import React, { Component } from 'react';
import moment from 'moment';

import ShiftAutocomplete from '../ShiftAutocomplete';
import ColShift from './ColShift';

export default class Row extends Component {
    shouldComponentUpdate(nextProps) {
        return this.props.isHost !== nextProps.isHost
            || this.props.isSARole !== nextProps.isSARole
            || this.props.shiftOptions !== nextProps.shiftOptions
            || this.props.data !== nextProps.data;
    }

    handleChangePublic = event => {
        const employeeId = this.props.data.employeeId;
        this.props.onChangePublics(employeeId, event.target.checked);
    }

    handleCheckWeekDay = (attrName, isChecked) => {
        const employeeId = this.props.data.employeeId;
        this.props.onChangeSelectWeekDay(employeeId, attrName, isChecked);
    }

    handleChangeShift = (name, shift) => {
        const employeeId = this.props.data.employeeId;
        this.props.onChangeShift(employeeId, name, shift);
    }

    handleChangeDefaultShift = (name, shift) => {
        const employeeId = this.props.data.employeeId;
        this.props.onChangeDefaultShift(employeeId, shift);
    }

    render() {
        const { isHost, isSARole, shiftOptions, data } = this.props;
        console.log('a');

        return (
            <tr>
                <td />
                <td className="col-section" title={data.teamName}>{data.teamCode}</td>
                <td className="col-nickname">{data.nickName}</td>
                <td className="col-shift">
                    <div className="shift-autocomplete-wrapper">
                        <ShiftAutocomplete
                            value={data.defaultShift !== null ? data.defaultShift.code : ''}
                            isDisabled={!isHost}
                            shiftOptions={shiftOptions.filter(shift => shift.id !== -1)}
                            onChange={this.handleChangeDefaultShift}
                        />
                    </div>
                </td>
                <ColShift
                    isHost={isHost} isSARole={isSARole} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.mondaySchedule}
                    scheduleAttrName={'mondaySchedule'}
                    isChecked={data.selectWeekDays.mondaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} isSARole={isSARole} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.tuesdaySchedule}
                    scheduleAttrName={'tuesdaySchedule'}
                    isChecked={data.selectWeekDays.tuesdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} isSARole={isSARole} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.wednesdaySchedule}
                    scheduleAttrName={'wednesdaySchedule'}
                    isChecked={data.selectWeekDays.wednesdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} isSARole={isSARole} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.thursdaySchedule}
                    scheduleAttrName={'thursdaySchedule'}
                    isChecked={data.selectWeekDays.thursdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} isSARole={isSARole} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.fridaySchedule}
                    scheduleAttrName={'fridaySchedule'}
                    isChecked={data.selectWeekDays.fridaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} isSARole={isSARole} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.saturdaySchedule}
                    scheduleAttrName={'saturdaySchedule'}
                    isChecked={data.selectWeekDays.saturdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} isSARole={isSARole} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.sundaySchedule}
                    scheduleAttrName={'sundaySchedule'}
                    isChecked={data.selectWeekDays.sundaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <td className="col-public">
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-public${data.employeeId}`}
                            checked={data.isPublic} disabled={!isHost} onChange={this.handleChangePublic}
                        />
                        <label htmlFor={`ckb-public${data.employeeId}`} />
                    </div>
                </td>
                <td title={moment(data.mondaySchedule.modifiedDate).format('DD MMM, YYYY HH:mm')}>
                    {data.mondaySchedule.modifierName}
                </td>
            </tr>
        );
    }
}