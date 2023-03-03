import React from 'react';
import { Button, Form } from 'react-bootstrap';
import * as dayjs from 'dayjs';
import LangHelper from '@jsroot/common/langHelper';

const Filter = ({ fromDate, toDate, onPrev, onNext, onThisWeek }) => {
    const mindate = dayjs().day(1).subtract(7, 'day').format('YYYY-MMM-DD');
    const maxdate = dayjs().day(1).add(28, 'day').format('YYYY-MMM-DD');
    const value = `${dayjs(fromDate).format('DD MMM')} - ${dayjs(toDate).format('DD MMM')}`;

    return (
        <Form className="form-filter" >
            <Form.Row bsPrefix="form-row">
                <Form.Group>
                    <div className="week-bar">
                        <button className="btn btn-primary btn-sm btn-prev-date"
                            disabled={dayjs(fromDate).isSame(mindate)}
                            onClick={onPrev}
                        >
                            <i className="fa fa-chevron-left" />
                        </button>
                        <Form.Control type="text" size="sm" className="text-center" value={value} readOnly={true} />
                        <button className="btn btn-primary btn-sm btn-next-date"
                            disabled={dayjs(fromDate).isSame(maxdate)}
                            onClick={onNext}
                        >
                            <i className="fa fa-chevron-right" />
                        </button>
                    </div>
                </Form.Group>
                <Form.Group className="ml-3">
                    <Button type="button" variant="primary" size="sm" onClick={onThisWeek}>
                        {LangHelper.getResource('ThisWeek')}
                    </Button>
                </Form.Group>
            </Form.Row>
        </Form>
    );
};

export default Filter;