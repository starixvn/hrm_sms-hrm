import React from 'react';

import { renderNumber } from '../../common.funcs';

const Row = ({ data, jobType, totalJobType, showHideOptions, showHideJobOptions, handleShowHideJobCells, onClick }) => {
    const renderCell = (value, jobLevel, unitName, isLocal) =>
        value > 0 ? <div className="cursor-pointer" onClick={event => onClick(event, jobLevel, unitName, isLocal)}>{value}</div> : '';
    const isMultipleData = data.filter(obj => obj.name.slice(0, 2) === jobType).length > 1;

    const getRowSpanJobType = () => {
        let countColSpan = 0;
        data.map(item => {
            if (item.name.slice(0, 2) === jobType) {
                countColSpan++
            };
        });
        return isMultipleData ? countColSpan + 1 : countColSpan;
    };

    const renderJobTotal = () => {
        const total = [];
        totalJobType.map(item => {
            if (item.name.slice(0, 2) === jobType && isMultipleData) {
                const cells = [];
                cells.push(<td key={`${item.name}-totaljobtype`} colSpan={2} className="text-center col-job-level bold">Total {jobType}</td>);
                item.cells.map(gunit => {
                    let totalLocal = 0,
                        totalExpat = 0;
                    const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                    gunit.cells.map(unit => {
                        totalLocal += unit.totalLocal;
                        totalExpat += unit.totalExpat;

                        if (showHideOption.show) {
                            cells.push(<td colSpan={2} key={`${item.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                {renderNumber(unit.totalExpat + unit.totalLocal)}
                            </td>);
                        }
                    });

                    cells.push(<td colSpan={2} key={`${item.name}-${gunit.name}-total-expat`} className="text-center col-total">{renderNumber(totalExpat + totalLocal)}</td>);
                });
                total.push(<tr key={`${item.name}-totaljobType`}>{cells}</tr>);
            }
        });
        return total;
    }

    const renderTotal = () => {
        const total = [];
        totalJobType.map(item => {
            if (item.name.slice(0, 2) === jobType && isMultipleData) {
                const cells = [];
                cells.push(<td key={`${item.name}-totaljobLevel`} className="text-center col-job-level">Total</td>);
                item.cells.map(gunit => {
                    let totalLocal = 0,
                        totalExpat = 0;
                    const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                    gunit.cells.map(unit => {
                        totalLocal += unit.totalLocal;
                        totalExpat += unit.totalExpat;

                        if (showHideOption.show) {
                            cells.push(
                                <td key={`${item.name}-${gunit.name}-${unit.name}-expat`} className="text-center">
                                    {renderNumber(unit.totalExpat)}
                                </td>
                            );
                            cells.push(<td key={`${item.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                {renderNumber(unit.totalLocal)}
                            </td>);
                        }
                    });

                    cells.push(<td key={`${item.name}-${gunit.name}-total-expat`} className="text-center col-total">{renderNumber(totalExpat)}</td>);
                    cells.push(<td key={`${item.name}-${gunit.name}-total-local`} className="text-center col-total">{renderNumber(totalLocal)}</td>);
                });
                total.push(<tr key={`${item.name}-totaljoblevel`}>{cells}</tr>);
            }
        });
        return total;
    }

    const renderBody = () => {
        const rows = [];
        data.map(item => {
            if (item.name.slice(0, 2) === jobType) {
                const showHideJobOption = showHideJobOptions.find(x => x.key === jobType);
                const cells = [];
                if (rows.length === 0) {
                    cells.push(<td key={`${item.name}-jobtype`} className={`${isMultipleData ? '' : 'text-center'} col-job-level bold`} rowSpan={getRowSpanJobType()}>
                        {
                           isMultipleData &&
                           <span className={`fa ${showHideJobOption.show ? 'fa-minus-square-o' : 'fa-plus-square-o'}`}
                                onClick={() => handleShowHideJobCells(jobType)} />
                        }
                        {jobType}
                    </td>);
                }
                if (showHideJobOption.show || !isMultipleData) {
                    cells.push(<td key={`${item.name}-data`} className="text-center col-job-level">
                        {isMultipleData ? item.name : 'Total'}
                    </td>);
                    item.cells.map(gunit => {
                        let totalLocal = 0,
                            totalExpat = 0;
                        const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                        gunit.cells.map(unit => {
                            totalLocal += unit.totalLocal;
                            totalExpat += unit.totalExpat;

                            if (showHideOption.show) {
                                cells.push(
                                    <td key={`${item.name}-${gunit.name}-${unit.name}-expat`} className="text-center">
                                        {renderCell(unit.totalExpat, item.name, unit.name, false)}
                                    </td>
                                );
                                cells.push(<td key={`${item.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                    {renderCell(unit.totalLocal, item.name, unit.name, true)}
                                </td>);
                            }
                        });

                        cells.push(<td key={`${item.name}-${gunit.name}-total-expat`} className="text-center col-total">{renderNumber(totalExpat)}</td>);
                        cells.push(<td key={`${item.name}-${gunit.name}-total-local`} className="text-center col-total">{renderNumber(totalLocal)}</td>);
                    });
                }

                rows.push(<tr key={`${item.name}-row-data`}>{cells}</tr>);
            }
        });
        return rows;
    }

    return (
        <>
            {renderBody()}
            {renderTotal()}
            {renderJobTotal()}
        </>
    );
};

export default Row;