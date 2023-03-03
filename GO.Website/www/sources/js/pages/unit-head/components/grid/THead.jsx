import React from 'react';
import * as dayjs from 'dayjs';

const THead = ({ isMobile, fromDate }) => {
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
                    const today = dayjs().format('YYYY-MMM-DD');
                    const date = dayjs(fromDate).add(item.value, 'day').format('YYYY-MMM-DD');
                    return (
                        <div className={`table-cell cell-shift ${dayjs(today).isSame(date) ? 'text-danger' : ''}`} key={`dayOfWeeks${item.value}`}>
                            <div className="cell-control-wrapper">
                                <div className="font-weight-bold">{dayjs(date).format('DD MMM')}</div>
                                <div>{dayjs(date).format('ddd')}</div>
                            </div>
                        </div>
                    );
                })
            }
        </>
    );
};

export default THead;