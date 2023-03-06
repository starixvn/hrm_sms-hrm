import React from 'react';
import * as dayjs from 'dayjs';

const THead = ({ isMobile, fromDate }) => {
    const DateFormat = 'YYYY-MM-DD';

    const dayOfWeeks = [
        { name: 'mondaySchedule', value: 0 },
        { name: 'tuesdaySchedule', value: 1 },
        { name: 'wednesdaySchedule', value: 2 },
        { name: 'thursdaySchedule', value: 3 },
        { name: 'fridaySchedule', value: 4 },
        { name: 'saturdaySchedule', value: 5 },
        { name: 'sundaySchedule', value: 6 }
    ];

    return (
        <>
            {
                !isMobile &&
                <div className="table-cell cell-nickname">Name</div>
            }
            {
                dayOfWeeks.map(item => {
                    const today = dayjs().format(DateFormat);
                    const date = dayjs(fromDate, DateFormat).add(item.value, 'day').format(DateFormat);
                    return (
                        <div className={`table-cell cell-shift ${dayjs(today).isSame(date) ? 'text-danger' : ''}`} key={`dayOfWeeks${item.value}`}>
                            <div className="cell-control-wrapper">
                                <div className="font-weight-bold">{dayjs(date, DateFormat).format('DD MMM')}</div>
                                <div>{dayjs(date, DateFormat).format('ddd')}</div>
                            </div>
                        </div>
                    );
                })
            }
        </>
    );
};

export default THead;