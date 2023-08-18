import React, { Fragment } from 'react';
import THead from './THead';
import Row from './Row';
import TotalRow from './TotalRow';
import GrandTotalRow from './GrandTotalRow';
import LangHelper from '@jsroot/common/langHelper';

const Grid = ({ employeeInfos, data, detail, onShowDetail, jobLevelsType }) => {
    const { headers, supervisors, employees, totalSupervisor, totalEmployee, grandTotal } = data;

    const handleShowDetail = (event, jobLevel, country, gunit) => {
        if (!detail.show) {
            const units = headers.find(item => item.name === country).cells.find(item => item.name === gunit).cells;
            const emps = employeeInfos.filter(emp => emp.jobLevel === jobLevel && emp.country === country && units.findIndex(unit => unit.name === emp.unitName) >= 0);
            onShowDetail(event.target, emps);
        }
    };

    const renderRows = rows => {
        const trs = [];
        rows.map(item => {
            trs.push(<Row key={item.name} data={item} onClick={handleShowDetail} />);
        });

        return trs;
    };

    return (
        <table className="table group-by-country">
            <THead data={headers} />
            <tbody>
                <GrandTotalRow data={grandTotal} />
                {renderRows(supervisors)}
                <TotalRow data={totalSupervisor} />
                {renderRows(employees)}
                <TotalRow data={totalEmployee} />
            </tbody>
        </table>
    );
};

export default Grid;