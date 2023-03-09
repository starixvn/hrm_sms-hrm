import React from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';
import * as dayjs from 'dayjs';
import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ fromDate, toDate, onPrev, onNext, onToday }) => {
    const DateFormat = 'YYYY-MM-DD';

    const mindate = dayjs().day(1).subtract(7, 'day').format(DateFormat);
    const maxdate = dayjs().day(1).add(28, 'day').format(DateFormat);

    let value = '';
    if (dayjs(fromDate, DateFormat).year() === dayjs(toDate, DateFormat).year()) {
        if (dayjs(fromDate, DateFormat).month() === dayjs(toDate, DateFormat).month()) {
            value = dayjs(fromDate, DateFormat).format('MMMM YYYY');
        } else {
            value = `${dayjs(fromDate, DateFormat).format('MMM')} - ${dayjs(toDate, DateFormat).format('MMM')} ${dayjs(fromDate, DateFormat).format('YYYY')}`;
        }
    } else {
        value = `${dayjs(fromDate, DateFormat).format('DD MMM YYYY')} - ${dayjs(toDate, DateFormat).format('DD MMM YYYY')}`;
    }

    return (
        <div class="row-filter">
            <button className="btn btn-sm btn-today" onClick={onToday}>
                {LangHelper.getResource('Today')}
            </button>
            <OverlayTrigger
                placement="bottom"
                overlay={
                    <Tooltip id="tooltip-prev-week">Previous week</Tooltip>
                }
            >
                <button className="btn btn-sm btn-prev-week"
                    disabled={dayjs(fromDate, DateFormat).isSame(mindate)}
                    onClick={onPrev}
                >
                    <i className="fa fa-chevron-left" />
                </button>
            </OverlayTrigger>
            <OverlayTrigger
                placement="bottom"
                overlay={
                    <Tooltip id="tooltip-next-week">Previous week</Tooltip>
                }
            >
                <button className="btn btn-sm btn-next-week"
                    disabled={dayjs(fromDate, DateFormat).isSame(maxdate)}
                    onClick={onNext}
                >
                    <i className="fa fa-chevron-right" />
                </button>
            </OverlayTrigger>
            <div className="txt">{value}</div>
        </div>
    );
};

export default Filter;