import React, { PureComponent } from 'react';

export default class ColIndex extends PureComponent {
    render() {
        const { data } = this.props;
        const totalOffDaysDefault = data.offDaysbyWeek + data.totalOffDaysReplacement;
        let totalOffDays = 0;
        Object.keys(data.selectWeekDays).forEach(objectKey => {
            if (!data[objectKey].isHoliday && (data[objectKey].isOffDay || data[objectKey].isLeaveOffDay)) {
                totalOffDays++;
            }
        });

        return (
            <div className="table-cell cell-index">
                {totalOffDaysDefault < totalOffDays ? <span className="glyphicon glyphicon-alert" /> : ''}
            </div>
        );
    }
}