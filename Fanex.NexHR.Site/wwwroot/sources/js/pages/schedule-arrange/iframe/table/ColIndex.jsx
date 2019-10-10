import React, { PureComponent } from 'react';

export default class ColIndex extends PureComponent {
    render() {
        this.scheduleAttrNames = ['mondaySchedule', 'tuesdaySchedule', 'wednesdaySchedule',
            'thursdaySchedule', 'fridaySchedule', 'saturdaySchedule', 'sundaySchedule'];
        const { data } = this.props;
        const totalOffDaysDefault = data.offDaysbyWeek;
        let totalOffDays = 0;
        this.scheduleAttrNames.forEach(attrName => {
            if (!data[attrName].isHoliday
                && !data[objectKey].isMoveOffDay
                && (data[attrName].isOffDay || data[attrName].isOffDayReplacement || data[attrName].isLeaveOffDay)) {
                totalOffDays++;
            }
        });

        return (
            <div className="table-cell cell-index">
                {totalOffDaysDefault < totalOffDays ? <i className="fa fa-exclamation-triangle" /> : ''}
            </div>
        );
    }
}