import React, { useState } from 'react';
import Row from './Row';
import TotalRow from './TotalRow';
import GrandTotalRow from './GrandTotalRow';
import LangHelper from '@jsroot/common/langHelper';

const Grid = ({ employeeInfos, data, detail, onShowDetail, jobLevelsType }) => {
    const { headers, supervisors, employees, totalSupervisor, totalEmployee, grandTotal, totalJobTypeSupervisor, totalJobTypeloyeEmployee } = data;

    const showHideJobs = () => {
        const showHides = [];
        totalJobTypeSupervisor.map(item => {
            showHides.push({
                key: item.name,
                show: false
            })
        });

        totalJobTypeloyeEmployee.map(item => {
            showHides.push({
                key: item.name,
                show: false
            })
        });
        return showHides;
    }

    const showHides = headers.map(gunit => {
        return {
            key: gunit.name,
            show: false
        };
    });
    const [showHideOptions, setShowHideOptions] = useState(showHides);
    const [showHideJobOptions, setShowHideJobOptions] = useState(showHideJobs);

    const handleShowHideCells = unit => {
        const options = [...showHideOptions];
        const index = options.findIndex(x => x.key === unit);
        options[index] = { ...options[index], show: !options[index].show };
        setShowHideOptions(options);
    };

    const handleShowHideJobCells = job => {
        const options = [...showHideJobOptions];
        const index = options.findIndex(x => x.key === job);
        options[index] = { ...options[index], show: !options[index].show };
        setShowHideJobOptions(options);
    };

    const handleShowDetail = (event, jobLevel, unitName, isLocal) => {
        if (!detail.show) {
            const emps = employeeInfos.filter(emp => emp.jobLevel === jobLevel && emp.unitName === unitName && emp.isLocal === isLocal);
            onShowDetail(event.target, emps);
        }
    };

    const renderRows = (rows, jobLevelsType, totalJobType) => {
        const trs = [];
        jobLevelsType.map(jobType => {
            trs.push(<Row key={jobType}
                data={rows}
                jobType={jobType}
                totalJobType={totalJobType}
                showHideOptions={showHideOptions}
                showHideJobOptions={showHideJobOptions}
                handleShowHideJobCells={handleShowHideJobCells}
                onClick={handleShowDetail} />);
        });

        return trs;
    };

    return (
        <table className="table group-by-country">
            <thead>
                <tr>
                    <td rowSpan={3} colSpan={2} className="col-job-level">{LangHelper.getResource('JobLevel')}</td>
                    {
                        headers.map(gunit => {
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);
                            const colSpan = (showHideOption.show ? gunit.cells.length * 2 : 0) + 2;

                            return (
                                <td key={gunit.name} className="group-unit" colSpan={colSpan}>
                                    <span>{gunit.name}</span>
                                    <span
                                        className={`fa ${showHideOption.show ? 'fa-minus-square-o' : 'fa-plus-square-o'}`}
                                        onClick={() => handleShowHideCells(gunit.name)}
                                    />
                                </td>
                            );
                        })
                    }
                </tr>
                <tr>
                    {
                        headers.map(gunit => {
                            const cols = [];
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);
                            if (showHideOption.show) {
                                gunit.cells.map(unit => {
                                    cols.push(<td key={`${gunit.name}-${unit.name}`} colSpan={2}>{unit.name}</td>);
                                });
                            }

                            cols.push(<td key={`${gunit.name}-total`} colSpan={2}>Total</td>);

                            return cols;
                        })
                    }
                </tr>
                <tr>
                    {
                        headers.map(gunit => {
                            const cols = [];
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);
                            if (showHideOption.show) {
                                gunit.cells.map(unit => {
                                    cols.push(<td key={`${gunit.name}-${unit.name}-expat`}>Expat</td>);
                                    cols.push(<td key={`${gunit.name}-${unit.name}-local`}>Local</td>);
                                });
                            }

                            cols.push(<td key={`${gunit.name}-total-expat`}>Expat</td>);
                            cols.push(<td key={`${gunit.name}-total-local`}>Local</td>);

                            return cols;
                        })
                    }
                </tr>
            </thead>
            <tbody>
                <GrandTotalRow data={grandTotal} showHideOptions={showHideOptions} />
                {renderRows(supervisors, jobLevelsType.Supervisor, totalJobTypeSupervisor)}
                <TotalRow data={totalSupervisor} showHideOptions={showHideOptions} />
                {renderRows(employees, jobLevelsType.Employee, totalJobTypeloyeEmployee)}
                <TotalRow data={totalEmployee} showHideOptions={showHideOptions} />
            </tbody>
        </table>
    );
};

export default Grid;