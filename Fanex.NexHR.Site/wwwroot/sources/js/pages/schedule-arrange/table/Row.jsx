import React, { PureComponent } from 'react';
import moment from 'moment';

import ColIndex from './ColIndex';
import ColNickname from './ColNickname';
import ColDefaultShift from './ColDefaultShift';
import ColShift from './ColShift';

export default class Row extends PureComponent {
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
        const { isHost, shiftOptions, data, showRemarkPopover } = this.props;

        return (
            <div className="table-row">
                <ColIndex data={data} />
                <div className="table-cell cell-section" title={data.team.name}>{data.team.code}</div>
                <ColNickname isHost={isHost}
                    employeeId={data.employeeId}
                    nickname={data.nickname}
                    remark={data.remark}
                    showRemarkPopover={showRemarkPopover}
                    onChangeShowRemarkPopover={this.props.onChangeShowRemarkPopover}
                    onUpdateRemark={this.props.onUpdateRemark}
                />
                <ColDefaultShift isHost={isHost} employeeId={data.employeeId}
                    shiftOptions={shiftOptions.filter(shift => shift.id !== -1)}
                    data={data.defaultShift}
                    onChangeDefaultShift={this.props.onChangeDefaultShift}
                />
                <ColShift
                    isHost={isHost} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.mondaySchedule}
                    scheduleAttrName={'mondaySchedule'}
                    isChecked={data.selectWeekDays.mondaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.tuesdaySchedule}
                    scheduleAttrName={'tuesdaySchedule'}
                    isChecked={data.selectWeekDays.tuesdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.wednesdaySchedule}
                    scheduleAttrName={'wednesdaySchedule'}
                    isChecked={data.selectWeekDays.wednesdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.thursdaySchedule}
                    scheduleAttrName={'thursdaySchedule'}
                    isChecked={data.selectWeekDays.thursdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.fridaySchedule}
                    scheduleAttrName={'fridaySchedule'}
                    isChecked={data.selectWeekDays.fridaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.saturdaySchedule}
                    scheduleAttrName={'saturdaySchedule'}
                    isChecked={data.selectWeekDays.saturdaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <ColShift
                    isHost={isHost} shiftOptions={shiftOptions}
                    employeeId={data.employeeId} data={data.sundaySchedule}
                    scheduleAttrName={'sundaySchedule'}
                    isChecked={data.selectWeekDays.sundaySchedule}
                    onChangeShift={this.handleChangeShift}
                    onCheckWeekDay={this.handleCheckWeekDay}
                />
                <div className="table-cell cell-public">
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom`} id={`ckb-public${data.employeeId}`}
                            checked={data.isPublic} disabled={!isHost} onChange={this.handleChangePublic}
                        />
                        <label htmlFor={`ckb-public${data.employeeId}`} />
                    </div>
                </div>
                <div className="table-cell cell-modifier" title={moment(data.mondaySchedule.modifiedDate).format('DD MMM, YYYY HH:mm')}>
                    {data.mondaySchedule.modifierName}
                </div>
            </div>
        );
    }
}