import React from 'react';
import * as dayjs from 'dayjs';

const CellShift = ({ data }) => {
    let value,
        className = '';
    if (data.isLeaveDay) {
        value = data.leaveTypeCode;
        className = 'leaveday';
    } else if (data.isHoliday) {
        value = 'PH';
        className = 'offday';
    } else if (data.isOffDay) {
        value = 'Off';
        className = 'offday';
    } else if (data.isWorkDay) {
        value = 'W';
    } else {
        value = '';
    }

    const today = dayjs().format('YYYY-MMM-DD');
    const workDate = dayjs(data.workDate).format('YYYY-MMM-DD');

    return (
        <div className={`table-cell cell-shift ${className} ${dayjs(today).isAfter(workDate) ? 'past' : ''}`}>
            {value}
        </div>
    );
};

export default CellShift;