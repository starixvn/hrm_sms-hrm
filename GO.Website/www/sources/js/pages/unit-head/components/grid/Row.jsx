import React from 'react';
import CellNickname from './CellNickname';
import CellShift from './CellShift';

const Row = ({ isMobile, data }) => {
    return (
        <>
            {
                isMobile &&
                <div className={`user-row`}>
                    <div className="font-weight-bold">
                        {data.fullName} {`${data.nickname ? ` (${data.nickname})` : ''}`}
                    </div>
                    <div>
                        <span className="department bg-secondary">{data.departmentCode}</span>
                    </div>
                </div>
            }
            <div className={`table-row`}>
                {
                    !isMobile &&
                    <CellNickname data={data} />
                }
                <CellShift data={data.mondaySchedule}
                    scheduleAttrName={'mondaySchedule'}
                />
                <CellShift data={data.tuesdaySchedule}
                    scheduleAttrName={'tuesdaySchedule'}
                />
                <CellShift data={data.wednesdaySchedule}
                    scheduleAttrName={'wednesdaySchedule'}
                />
                <CellShift data={data.thursdaySchedule}
                    scheduleAttrName={'thursdaySchedule'}
                />
                <CellShift data={data.fridaySchedule}
                    scheduleAttrName={'fridaySchedule'}
                />
                <CellShift data={data.saturdaySchedule}
                    scheduleAttrName={'saturdaySchedule'}
                />
                <CellShift data={data.sundaySchedule}
                    scheduleAttrName={'sundaySchedule'}
                />
            </div>
        </>
    );
};

export default Row;