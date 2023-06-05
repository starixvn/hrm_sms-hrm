import React from 'react';
import { Cell, Row } from '@jsroot/components/react-grid/index';
import dayjs from 'dayjs';

const RowItem = data => {
    const celcost = [];
    data.period.settings.map(item => {
        celcost.push(<Cell key={item.fromDate} className="text-center">{dayjs(item.fromDate).format('DD MMM,YYYY')}</Cell>);
        celcost.push(<Cell key={item.toDate} className="text-center">{dayjs(item.toDate).format('DD MMM,YYYY')}</Cell>);
    });

    return (
        <Row>
            <Cell className="text-center">{data.period.rowNum}</Cell>
            <Cell className="text-center">{data.period.name}</Cell>
            {celcost}
            <Cell className="text-center">x</Cell>
            <Cell className="text-center">x</Cell>
        </Row>
    );
};

export default RowItem;