import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';
import moment from 'moment';

import { OffDayShiftId } from '../../../shared/constants';
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

    handleChangeSelectWeekDay = (event) => {
        this.props.onChangeSelectWeekDay(this.props.scheduleAttrName, event.target.checked);
    }

    handleChangeShift = (scheduleAttrName, shift) => {
        this.props.onChangeShift(scheduleAttrName, shift);
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
        const { employeeId, data, isHost, scheduleAttrName, defaultShift, shiftOptions, isUsedWeeklyDefaultShift } = this.props;
        let classOfInput = '';
        let valueOfInput = '';
        let placeholder = '';
        let isDisabled = false;
        let isShowCheckBox = isHost;
        let isLeaveHalfday = false;
        let leaveCodeHalfday = '';
        let classOfleavetype = '';
        let fromTime = '';
        let toTime = '';

        if (data.isLeaveOffDay) {
            valueOfInput = 'Off';
            classOfInput = 'leave offday';
            isDisabled = true;
            isShowCheckBox = false;
        } else if (data.isLeaveDay) {
            isDisabled = true;
            isShowCheckBox = false;

            if (data.isHoliday) {
                valueOfInput = 'PH';
                classOfInput = 'holiday';
            } else if (data.isOffDay) {
                valueOfInput = 'Off';
                classOfInput = 'offday';
            } else {
                if (data.isLeaveHalfDay) {
                    leaveCodeHalfday = data.leaveType.code + ' ½';
                    fromTime = moment(data.leaveType.fromTime).format('DD MMM, YYYY HH:mm');
                    toTime = moment(data.leaveType.toTime).format('DD MMM, YYYY HH:mm');
                    isLeaveHalfday = true;
                    isDisabled = false;

                    if (data.shift !== null) {
                        valueOfInput = data.shift.code;
                    }
                } else {
                    valueOfInput = data.leaveType.code;
                    if (data.leaveType2 !== null && data.leaveType.code !== data.leaveType2.code) {
                        classOfInput = 'leaveday leavehalfday2';
                        valueOfInput = data.leaveType.code + ' / ' + data.leaveType2.code;
                    }
                }

                if (data.leaveType !== null && (data.leaveType2 === null || data.leaveType.code === data.leaveType2.code)) {
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
                        default:
                            classOfleavetype = 'leaveday';
                            break;
                    }
                }

                if (classOfleavetype !== '' && !isLeaveHalfday) {
                    classOfInput = classOfleavetype;
                }
            }
        } else if (data.isHoliday) {
            classOfInput = `holiday ${data.isKeepInView ? 'iskeepinview' : data.isOvertime ? 'isovertime' : ''}`;
            if (data.isOffDay) {
                valueOfInput = 'Off';
            } else {
                valueOfInput = data.shift !== null ? data.shift.code : 'PH';
            }
        } else if (data.isOffDay) {
            classOfInput = `offday ${data.isKeepInView ? 'iskeepinview' : data.isOvertime ? 'isovertime' : ''}`;
            valueOfInput = data.shift !== null ? data.shift.code : 'Off';
        } else if (data.isKeepInView) {
            classOfInput = 'keepinviewday';
            valueOfInput = 'KIV';
        } else if (data.isChangeShift) {
            classOfInput = 'changeshift';
            valueOfInput = data.shift.code;
        } else if (data.isOvertime) {
            classOfInput = 'overtime';
            valueOfInput = data.shift.code;
        } else {
            valueOfInput = data.shift !== null ? data.shift.code : '';
        }

        if (data.shift !== null
            && defaultShift !== null
            && valueOfInput === data.shift.code
            && valueOfInput === defaultShift.code
            && !isUsedWeeklyDefaultShift) {
            placeholder = valueOfInput;
            classOfInput += ' same-default-shift';
        }

        if (data.isOffDayReplacement) {
            classOfInput += ' offdayreplacement';
        }

        if (!data.isModified) {
            classOfInput += ' is-default-shift';
        }

        if (!isHost || data.isOvertime || data.isChangeShift) {
            isDisabled = true;
            isShowCheckBox = false;
        }

        return (
            <div className="table-cell cell-shift" onMouseOver={this.handleMouseOver} onMouseLeave={this.handleMouseLeave}>
                <div className={`cell-shift-wrapper ${isLeaveHalfday ? 'leavehalfday' : ''}`}>
                    {
                        isLeaveHalfday &&
                        <OverlayTrigger trigger={['hover']} placement={'top'}
                            overlay={
                                <Tooltip id={`leavehalfday${scheduleAttrName}${employeeId}`}>
                                    {`From: ${fromTime}`}<br />{`To: ${toTime}`}
                                </Tooltip>
                            }
                        >
                            <span className={`halfdaycode ${classOfleavetype}`}>{leaveCodeHalfday}</span>
                        </OverlayTrigger>
                    }
                    <ShiftAutocomplete
                        shift={{ code: valueOfInput }}
                        name={scheduleAttrName}
                        className={classOfInput}
                        placeholder={placeholder}
                        isDisabled={isDisabled}
                        shiftOptions={isLeaveHalfday ? shiftOptions.filter(shift => shift.id !== OffDayShiftId) : shiftOptions}
                        onChange={this.handleChangeShift}
                    />
                    {
                        isShowCheckBox && this.state.isShowCheckbox &&
                        <div className="checkbox">
                            <input type="checkbox" className={`checkbox-custom ${scheduleAttrName}`} checked={this.props.isChecked}
                                id={`ckb-${scheduleAttrName}${employeeId}`} onChange={this.handleChangeSelectWeekDay}
                            />
                            <label htmlFor={`ckb-${scheduleAttrName}${employeeId}`} />
                        </div>
                    }
                </div>
            </div>
        );
    }
}