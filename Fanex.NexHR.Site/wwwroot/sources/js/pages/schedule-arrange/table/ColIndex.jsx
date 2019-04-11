import React, { PureComponent } from 'react';

export default class ColIndex extends PureComponent {
    render() {
        const { data } = this.props;
        const numberOffDefault = data.mondaySchedule.offDaysbyWeek;
        let numberOffCurrent = 0;
        Object.keys(data.selectWeekDays).forEach(objectKey => {
            if (data[objectKey].isOffDay || data[objectKey].isLeaveOffDay) {
                numberOffCurrent++;
            }
        });

        return (
            <div className="table-cell cell-index">
                {numberOffDefault < numberOffCurrent ? <span className="glyphicon glyphicon-alert" /> : ''}
            </div>
        );
    }
}