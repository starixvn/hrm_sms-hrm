import React, { useState } from 'react';
import { Cell, Row } from '@jsroot/components/react-grid/index';
import LangHelper from '@jsroot/common/langHelper';

const RowItem = ({ employee, index, onChangeStatus }) => {
    const [excluded, setStatus] = useState(employee.isExcluded);

    const handleChangeStatus = () => {
        setStatus(!excluded);
        onChangeStatus(employee.siteId, employee.empId, !excluded);
    };

    return (
        <Row>
            <Cell className="text-center">{index}</Cell>
            <Cell className="text-center">{employee.siteName}</Cell>
            <Cell className="text-center">{employee.username}</Cell>
            <Cell className="text-center">{employee.code}</Cell>
            <Cell className="text-center">{employee.nickname}</Cell>
            <Cell>{employee.fullName}</Cell>
            <Cell className="text-center">{employee.isLocal ? LangHelper.getResource('Local') : LangHelper.getResource('Expat')}</Cell>
            <Cell>{employee.unitName}</Cell>
            <Cell>{employee.departmentName}</Cell>
            <Cell>{employee.sectionName}</Cell>
            <Cell>{employee.teamName}</Cell>
            <Cell className="text-center">
                <input type='checkbox'
                    checked={!excluded}
                    onChange={handleChangeStatus}
                />
            </Cell>
        </Row>
    );
};

export default RowItem;