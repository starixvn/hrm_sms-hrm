import React, { PureComponent } from 'react';
import moment from 'moment';

import ShiftAutocomplete from '../ShiftAutocomplete';

export default class ColShift extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            isShowCheckbox: props.isChecked
        };
    }

    componentDidUpdate(prevProps) {
        if (this.props.isChecked !== prevProps.isChecked) {
            this.setState({ isShowCheckbox: this.props.isChecked });
        }
    }

    handleCheckWeekDay = (event) => {
        this.props.onCheckWeekDay(this.props.scheduleAttrName, event.target.checked);
    }

    handleChangeShift = (scheduleAttrName, shift) => {
        this.props.onChangeShift(scheduleAttrName, shift);
    }

    renderShiftAutocomplete = () => {
        const { employeeId, data, isHost, isSARole, scheduleAttrName, shiftOptions } = this.props;
        let classOfInput = '';
        let valueOfInput = '';
        let isDisabled = false;
        let isShowCheckBox = isHost;
        let isShowAddShiftInLeaveHalfday = false;
        let shiftCodeForHalfday = '';
        let classOfleavetype = '';
        let isHoliday = false;

        if (data.isLeaveOffDay) {
            valueOfInput = 'Off';
            classOfInput = 'offday';
            isDisabled = true;
            isShowCheckBox = false;
        } else if (data.isLeaveDay) {
            isDisabled = true;
            isShowCheckBox = false;
            classOfInput = 'leaveday';

            if (data.isHoliday) {
                valueOfInput = 'PH';
                classOfInput = 'holiday';
                isHoliday = true;
            } else if (data.isOffDay) {
                valueOfInput = 'Off';
                classOfInput = 'offday';
            } else if (data.leaveDay === 0.5) {
                classOfInput += ' shift';
                valueOfInput = data.leaveType.code + '½';
                isShowAddShiftInLeaveHalfday = true;

                if (data.shift !== null) {
                    shiftCodeForHalfday = data.shift.code;
                }
            } else {
                if (data.leaveType2 === null) {
                    valueOfInput = data.leaveType.code;
                } else {
                    classOfInput += ' leavehalfday2';
                    valueOfInput = data.leaveType.code + ' / ' + data.leaveType2.code;
                }
            }
        } else if (data.isHoliday) {
            isHoliday = true;
            classOfInput = 'shift holiday';
            if (data.shift !== null && data.shift.id !== -1) {
                valueOfInput = data.shift.code;
            } else {
                valueOfInput = 'PH';
            }

            if (data.isKeepInViewDay) {
                classOfInput = 'holiday keepinviewday';
                isDisabled = true;
            }
        } else if (data.isOffDay) {
            if (data.shift !== null) {
                valueOfInput = data.shift.code;
            } else {
                valueOfInput = 'Off';
            }

            if (data.isKeepInViewDay) {
                isShowCheckBox = false;
                classOfInput = 'offday keepinviewday';
                isDisabled = true;
            } else {
                classOfInput = 'shift offday';
            }
        } else if (data.isKeepInViewDay) {
            classOfInput = 'shift keepinviewday';
            valueOfInput = 'KIV';
        } else if (data.isChangeShift) {
            classOfInput = 'changeshift';
            valueOfInput = data.shift.code;
            isDisabled = true;
            isShowCheckBox = false;
        } else if (data.shift !== null) {
            classOfInput = 'shift';
            valueOfInput = data.shift.code;
        } else {
            classOfInput = 'shift';
        }

        if (data.isOvertime || data.isChangeShift) {
            isDisabled = true;
            isShowCheckBox = false;
        }

        if (data.leaveType !== null && data.leaveType2 === null) {
            switch (data.leaveType.id) {
                case 2:
                    classOfleavetype = 'annualleave';
                    break;

                case 25:
                    classOfleavetype = 'unpaidleave';
                    break;

                case 9:
                    classOfleavetype = 'businesstrip';
                    break;

                case 6:
                    classOfleavetype = 'weddingleave';
                    break;
            }
        }

        //classOfInput += ' tooltip';
        if (classOfleavetype !== '') {
            classOfInput += ' ' + classOfleavetype;
        }

        if (!isSARole && moment(data.workDate) <= moment().add(-28, 'days')) {
            isShowCheckBox = false;
            isDisabled = true;
        }

        if (!isHost) {
            isDisabled = true;
        }

        return (
            <div className="shift-autocomplete-wrapper">
                <ShiftAutocomplete
                    value={valueOfInput}
                    name={scheduleAttrName}
                    className={classOfInput}
                    isDisabled={isDisabled}
                    shiftOptions={isHoliday ? shiftOptions.filter(shift => shift.id !== -1) : shiftOptions}
                    onChange={this.handleChangeShift}
                />
                {
                    isShowCheckBox && this.state.isShowCheckbox &&
                    <div className="checkbox">
                        <input type="checkbox" className={`checkbox-custom ${scheduleAttrName}`} checked={this.props.isChecked}
                            id={`ckb-${scheduleAttrName}${employeeId}`} onChange={this.handleCheckWeekDay}
                        />
                        <label htmlFor={`ckb-${scheduleAttrName}${employeeId}`} />
                    </div>
                }
                {
                    !isShowCheckBox && isShowAddShiftInLeaveHalfday &&
                    <a className="addHalfdayShift" href="javascript:;" title={shiftCodeForHalfday}>+</a>
                }
            </div>
        );
    }

    handleMouseOver = () => {
        if (!this.state.isShowCheckbox) {
            this.setState({ isShowCheckbox: true });
        }
    }

    handleMouseLeave = () => {
        if (this.state.isShowCheckbox && !this.props.isChecked) {
            this.setState({ isShowCheckbox: false });
        }
    }

    render() {
        return (
            <td className="col-shift" onMouseOver={this.handleMouseOver} onMouseLeave={this.handleMouseLeave}>
                {this.renderShiftAutocomplete()}
            </td>
        );
    }
}