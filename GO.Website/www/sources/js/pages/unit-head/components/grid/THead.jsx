import React from 'react';
import * as dayjs from 'dayjs';

import Filter from './Filter';

const THead = ({ isMobile, filter, onPrev, onNext, onToday, onChangeCompany }) => {
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
        <div className={`table-header`}>
            <Filter filter={filter}
                onPrev={onPrev}
                onNext={onNext}
                onToday={onToday}
                onChangeCompany={onChangeCompany}
            />
            <div className="row-dayofweek">
                {
                    !isMobile &&
                    <div className="table-cell cell-nickname"></div>
                }
                {
                    dayOfWeeks.map(item => {
                        const today = dayjs().format(DateFormat);
                        const date = dayjs(filter.fromDate, DateFormat).add(item.value, 'day').format(DateFormat);
                        return (
                            <div className={`table-cell cell-shift ${dayjs(today).isSame(date) ? 'today' : ''}`} key={`dayOfWeeks${item.value}`}>
                                <div className="cell-control-wrapper">
                                    <div className="dayofweek">{dayjs(date, DateFormat).format('ddd')}</div>
                                    <div className="dayofmonth"><div>{dayjs(date, DateFormat).format('D')}</div></div>
                                </div>
                            </div>
                        );
                    })
                }
            </div>
        </div>
    );
};

export default THead;