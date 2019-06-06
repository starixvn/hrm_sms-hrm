import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';
import moment from 'moment';

import ShiftAutocomplete from '../../ShiftAutocomplete';

export default class ColShift extends PureComponent {
    render() {
        const { employeeId, data, scheduleAttrName, defaultShift, isUsedWeeklyDefaultShift } = this.props;
        let classOfInput = '';
        let valueOfInput = '';
        let isLeaveHalfday = false;
        let leaveCodeHalfday = '';
        let classOfleavetype = '';
        let fromTime = '';
        let toTime = '';

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
                if (data.isLeaveHalfDay) {
                    leaveCodeHalfday = data.leaveType.code + ' ½';
                    fromTime = moment(data.leaveType.fromTime).format('DD MMM, YYYY HH:mm');
                    toTime = moment(data.leaveType.toTime).format('DD MMM, YYYY HH:mm');
                    isLeaveHalfday = true;

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

        if (!data.isModified) {
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
                    <ShiftAutocomplete shift={shift} className={classOfInput} isDisabled />
                </div>
            </div>
        );
    }
}