import React from 'react';
import { Dropdown, OverlayTrigger, Tooltip } from 'react-bootstrap';
import * as dayjs from 'dayjs';
import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ filter, onPrev, onNext, onToday, onChangeCompany }) => {
    const DateFormat = 'YYYY-MM-DD';

    const mindate = dayjs().day(1).subtract(7, 'day').format(DateFormat);
    const maxdate = dayjs().day(1).add(28, 'day').format(DateFormat);
    const fromDate = dayjs(filter.fromDate, DateFormat);
    const toDate = dayjs(filter.toDate, DateFormat);

    let value = '';
    if (fromDate.year() === toDate.year()) {
        if (fromDate.month() === toDate.month()) {
            value = fromDate.format('MMMM YYYY');
        } else {
            value = `${fromDate.format('MMM')} - ${toDate.format('MMM')} ${fromDate.format('YYYY')}`;
        }
    } else {
        value = `${fromDate.format('DD MMM YYYY')} - ${toDate.format('DD MMM YYYY')}`;
    }

    const companies = [
        { companyId: 1, name: 'Nexcel' },
        { companyId: 3, name: 'SMS' },
        { companyId: 5, name: 'CTI' },
        { companyId: 6, name: 'Lexfin' }
    ];

    return (
        <div className="row-filter">
            <OverlayTrigger
                placement="right"
                overlay={
                    <Tooltip id="tooltip-company">{LangHelper.getResource('Company')}</Tooltip>
                }
            >
                <Dropdown>
                    <Dropdown.Toggle as="div" className="dropdown-company" id="dropdown-company">
                        <i className="fa fa-filter" aria-hidden="true"></i>
                    </Dropdown.Toggle>
                    <Dropdown.Menu>
                        <Dropdown.Item className={`${filter.companyId === null ? 'selected' : ''}`}
                            onClick={() => onChangeCompany(null)}
                        >-- All --</Dropdown.Item>
                        {
                            companies.map(item =>
                                <Dropdown.Item key={item.companyId}
                                    className={`${filter.companyId === item.companyId ? 'selected' : ''}`}
                                    onClick={() => onChangeCompany(item.companyId)}
                                >{item.name}</Dropdown.Item>)
                        }
                    </Dropdown.Menu>
                </Dropdown>
            </OverlayTrigger>

            <button className="btn btn-sm btn-today" onClick={onToday}>
                {LangHelper.getResource('Today')}
            </button>
            {
                dayjs(fromDate, DateFormat).isSame(mindate) &&
                <button className="btn btn-sm btn-prev-week"
                    disabled={dayjs(fromDate, DateFormat).isSame(mindate)}
                    onClick={onPrev}
                >
                    <i className="fa fa-chevron-left" />
                </button>
            }
            {
                dayjs(fromDate, DateFormat).isAfter(mindate) &&
                <OverlayTrigger
                    placement="bottom"
                    overlay={
                        <Tooltip id="tooltip-prev-week">{LangHelper.getResource('PreviousWeek')}</Tooltip>
                    }
                >
                    <button className="btn btn-sm btn-prev-week"
                        disabled={dayjs(fromDate, DateFormat).isSame(mindate)}
                        onClick={onPrev}
                    >
                        <i className="fa fa-chevron-left" />
                    </button>
                </OverlayTrigger>
            }
            {
                dayjs(fromDate, DateFormat).isSame(maxdate) &&
                <button className="btn btn-sm btn-next-week"
                    disabled={dayjs(fromDate, DateFormat).isSame(maxdate)}
                    onClick={onNext}
                >
                    <i className="fa fa-chevron-right" />
                </button>
            }
            {
                dayjs(fromDate, DateFormat).isBefore(maxdate) &&
                <OverlayTrigger
                    placement="bottom"
                    overlay={
                        <Tooltip id="tooltip-next-week">{LangHelper.getResource('NextWeek')}</Tooltip>
                    }
                >
                    <button className="btn btn-sm btn-next-week"
                        disabled={dayjs(fromDate, DateFormat).isSame(maxdate)}
                        onClick={onNext}
                    >
                        <i className="fa fa-chevron-right" />
                    </button>
                </OverlayTrigger>
            }
            <div className="txt">{value}</div>
        </div>
    );
};

export default Filter;