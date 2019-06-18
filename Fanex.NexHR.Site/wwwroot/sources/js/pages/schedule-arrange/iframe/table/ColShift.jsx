import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';
import moment from 'moment';

import ShiftAutocomplete from '../../ShiftAutocomplete';

export default class ColShift extends PureComponent {
    render() {
        const { employeeId, data, scheduleAttrName, defaultShift, isUsedWeeklyDefaultShift } = this.props;
        let classOfInput = '';
        let valueOfInput = '';
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
        } else if (data.isLeaveDay) {
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
            if (data.isChangeShift || data.isOvertime) {
                moreInfo = {
                    show: true,
                    content: data.isChangeShift ? 'Change Shift' : 'Overtime'
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
            classOfInput += ' same-default-shift';
        }

        if (data.isOffDayReplacement) {
            classOfInput += ' offdayreplacement';
        }

        if (!data.isModified && data.shift !== null) {
            classOfInput += ' is-default-shift';
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
            <div className="table-cell cell-shift">
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
                    <ShiftAutocomplete shift={shift} className={classOfInput} isDisabled />
                </div>
            </div>
        );
    }
}