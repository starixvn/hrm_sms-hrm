import React, { useState, Fragment } from 'react';
import Row from './Row';
import TotalRow from './TotalRow';
import GrandTotalRow from './GrandTotalRow';
import LangHelper from '@jsroot/common/langHelper';

const Grid = ({ employeeInfos, data, detail, onShowDetail, jobLevelsType }) => {
    const { headers, supervisors, employees, totalSupervisor, totalEmployee, grandTotal, totalJobTypeSupervisor, totalJobTypeEmployee } = data;

    const showHides = () => {
        const showHides = [];
        totalJobTypeSupervisor.map(item => {
            showHides.push({
                key: item.name,
                show: false
            })
        });

        totalJobTypeEmployee.map(item => {
            showHides.push({
                key: item.name,
                show: false
            })
        });
        return showHides;
    }

    const [showHideOptions, setShowHideOptions] = useState(showHides);

    const handleShowHideCells = unit => {
        const options = [...showHideOptions];
        const index = options.findIndex(x => x.key === unit);
        options[index] = { ...options[index], show: !options[index].show };
        setShowHideOptions(options);
    };

    const handleShowDetail = (event, jobLevel, country, gunit) => {
        if (!detail.show) {
            const units = headers.find(item => item.name === country).cells.find(item => item.name === gunit).cells;
            const emps = employeeInfos.filter(emp => emp.jobLevel === jobLevel && emp.country === country && units.findIndex(unit => unit.name === emp.unitName) >= 0);
            onShowDetail(event.target, emps);
        }
    };

    const renderRows = (rows, jobLevelsType, totalJobType) => {
        const trs = [];
        jobLevelsType.map(jobType => {
            trs.push(<Row key={jobType}
                data={rows}
                jobType={jobType}
                handleShowHideCells={handleShowHideCells}
                totalJobType={totalJobType}
                showHideOptions={showHideOptions}
                onClick={handleShowDetail} />);
        });

        return trs;
    };

    return (
        <table className="table group-by-country">
            <thead>
                <tr>
                    <td rowSpan={2} colSpan={2} className="col-job-level">{LangHelper.getResource('JobLevel')}</td>
                    {
                        headers.map(country => <td key={country.name} colSpan={3}>{country.name}</td>)
                    }
                </tr>
                <tr>
                    {
                        headers.map(country => <Fragment key={country.name}>
                            <td className="col-total">PU Total</td>
                            <td className="col-total">BU Total</td>
                            <td className="col-total">SU Total</td>
                        </Fragment>)
                    }
                </tr>
            </thead>
            <tbody>
                <GrandTotalRow data={grandTotal} totalSupervisor={totalSupervisor} totalEmployee={totalEmployee} />
                {renderRows(supervisors, jobLevelsType.Supervisor, totalJobTypeSupervisor)}
                <TotalRow data={totalSupervisor} />
                {renderRows(employees, jobLevelsType.Employee, totalJobTypeEmployee)}
                <TotalRow data={totalEmployee} />
            </tbody>
        </table>
    );
};

export default Grid;