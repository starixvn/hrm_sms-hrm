import React from 'react';
import CellNickname from './CellNickname';
import CellShift from './CellShift';

const Row = ({ isMobile, data }) => {
    return (
        <>
            {
                isMobile &&
                <CellNickname data={data} isMobile={isMobile} />
            }
            <div className={`table-row`}>
                {
                    !isMobile &&
                    <CellNickname data={data} isMobile={isMobile} />
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