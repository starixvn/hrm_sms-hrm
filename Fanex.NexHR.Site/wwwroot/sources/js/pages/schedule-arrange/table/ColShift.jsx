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
        let classOfleavetype = '';
        let moreInfo = {
            show: false,
            isLeaveHalfDay: false,
            leaveCode: '',
            content: ''
        };

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

                if (data.isLeaveHalfDay) {
                    const fromTime = moment(data.leaveType.fromTime).format('DD MMM, YYYY HH:mm');
                    const toTime = moment(data.leaveType.toTime).format('DD MMM, YYYY HH:mm');
                    isDisabled = false;
                    moreInfo = {
                        show: true,
                        isLeaveHalfDay: true,
                        leaveCode: data.leaveType.code + ' ½',
                        content: `From: ${fromTime}\r\nTo: ${toTime}`
                    };

                    if (data.shift !== null) {
                        valueOfInput = data.shift.code;
                    }
                } else {
                    valueOfInput = data.leaveType.code;
                    classOfInput = classOfleavetype;

                    if (data.leaveType2 !== null && data.leaveType.code !== data.leaveType2.code) {
                        classOfInput = 'leaveday leavehalfday2';
                        valueOfInput = data.leaveType.code + ' / ' + data.leaveType2.code;
                    }
                }
            }
        } else if (data.isHoliday) {
            classOfInput = 'holiday';
            if (data.isOffDay) {
                valueOfInput = 'Off';
            } else {
                valueOfInput = data.shift !== null ? data.shift.code : 'PH';
            }

            if (data.isChangeShift || data.isOvertime) {
                moreInfo = {
                    show: true,
                    content: data.isChangeShift ? 'Change Shift' : 'Overtime'
                };
            }
        } else if (data.isOffDay) {
            classOfInput = 'offday';
            valueOfInput = data.shift !== null ? data.shift.code : 'Off';
            if (data.isFixedOffDay || data.isChangeShift || data.isOvertime) {
                moreInfo = {
                    show: true,
                    content: data.isFixedOffDay ? 'Off Day OT Fixed' : data.isChangeShift ? 'Change Shift' : 'Overtime'
                };
            }
        } else if (data.isKeepInView) {
            classOfInput = 'keepinviewday';
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

        if (!data.isModified && data.shift !== null) {
            classOfInput += ' is-default-shift';
        }

        if (!isHost || data.isOvertime || data.isChangeShift) {
            isDisabled = true;
            isShowCheckBox = false;
        }

        let shift;
        if (data.shift === null) {
            shift = { code: valueOfInput, name: valueOfInput };
        } else if (data.shift.code !== valueOfInput) {
            shift = { ...data.shift, code: valueOfInput, name: valueOfInput };
        } else {
            shift = data.shift;
        }

        return (
            <div className="table-cell cell-shift" onMouseOver={this.handleMouseOver} onMouseLeave={this.handleMouseLeave}>
                <div className={`cell-shift-wrapper ${moreInfo.isLeaveHalfDay ? 'leavehalfday' : ''}`}>
                    {
                        moreInfo.show &&
                        <OverlayTrigger trigger={['hover']} placement={'top'}
                            overlay={
                                <Tooltip id={`moreInfo-${scheduleAttrName}${employeeId}`}>
                                    {moreInfo.content}
                                </Tooltip>
                            }
                        >
                            {
                                moreInfo.isLeaveHalfDay
                                    ? <span className={`halfdaycode ${classOfleavetype}`}>{moreInfo.leaveCode}</span>
                                    : <i className="fa fa-info-circle" />
                            }
                        </OverlayTrigger>
                    }
                    <ShiftAutocomplete
                        shift={shift}
                        name={scheduleAttrName}
                        className={classOfInput}
                        placeholder={placeholder}
                        isDisabled={isDisabled}
                        shiftOptions={moreInfo.isLeaveHalfDay ? shiftOptions.filter(shift => shift.id !== OffDayShiftId) : shiftOptions}
                        onChange={this.handleChangeShift}
                    />
                    <div className={`checkbox ${isShowCheckBox && this.state.isShowCheckbox ? '' : 'hidden'}`}>
                        <input type="checkbox" className={`checkbox-custom ${scheduleAttrName}`} checked={this.props.isChecked}
                            id={`ckb-${scheduleAttrName}${employeeId}`} onChange={this.handleChangeSelectWeekDay}
                        />
                        <label htmlFor={`ckb-${scheduleAttrName}${employeeId}`} />
                    </div>
                </div>
            </div>
        );
    }
}