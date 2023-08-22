import React from 'react';

const Row = ({ data, jobType, totalJobType, showHideOptions, showHideJobOptions, handleShowHideJobCells, onClick }) => {
    const renderCell = (value, jobLevel, unitName, isLocal) =>
        value > 0 ? <div className="cursor-pointer" onClick={event => onClick(event, jobLevel, unitName, isLocal)}>{value}</div> : '';
    const isMultipleData = data.filter(obj => obj.name.slice(0, 2) === jobType).length > 1;

    const getRowSpanJobType = () => {
        let countColSpan = 0;
        data.map(item => {
            if (item.name.slice(0, 2) === jobType) countColSpan++;
        });
        return isMultipleData ? countColSpan + 1 : countColSpan;
    };

    const renderJobTotal = () => {
        const total = [];
        totalJobType.map(item => {
            if (item.name.slice(0, 2) === jobType && isMultipleData) {
                let cell = [];
                cell.push(<td colSpan={2} className="text-center col-job-level bold">Total {jobType}</td>);
                item.cells.map(gunit => {
                    let totalLocal = 0,
                        totalExpat = 0;
                    const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                    gunit.cells.map(unit => {
                        totalLocal += unit.totalLocal;
                        totalExpat += unit.totalExpat;

                        if (showHideOption.show) {
                            cell.push(<td colSpan={2} key={`${item.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                {unit.totalExpat + unit.totalLocal}
                            </td>);
                        }
                    });

                    cell.push(<td colSpan={2} key={`${data.name}-${gunit.name}-total-expat`} className="text-center col-total">{totalExpat + totalLocal}</td>);
                });
                total.push(<tr>{cell}</tr>);
            }
        });
        return total;
    }

    const renderTotal = () => {
        const total = [];
        totalJobType.map(item => {
            if (item.name.slice(0, 2) === jobType && isMultipleData) {
                let cell = [];
                cell.push(<td className="text-center col-job-level">Total</td>);
                item.cells.map(gunit => {
                    let totalLocal = 0,
                        totalExpat = 0;
                    const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                    gunit.cells.map(unit => {
                        totalLocal += unit.totalLocal;
                        totalExpat += unit.totalExpat;

                        if (showHideOption.show) {
                            cell.push(
                                <td key={`${item.name}-${gunit.name}-${unit.name}-expat`} className="text-center">
                                    {unit.totalExpat}
                                </td>
                            );
                            cell.push(<td key={`${item.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                {unit.totalLocal}
                            </td>);
                        }
                    });

                    cell.push(<td key={`${data.name}-${gunit.name}-total-expat`} className="text-center col-total">{totalExpat}</td>);
                    cell.push(<td key={`${data.name}-${gunit.name}-total-local`} className="text-center col-total">{totalLocal}</td>);
                });
                total.push(<tr>{cell}</tr>);
            }
        });
        return total;
    }

    const renderbody = () => {
        const row = [];
        data.map(item => {
            if (item.name.slice(0, 2) === jobType) {
                const showHideJobOption = showHideJobOptions.find(x => x.key === jobType);
                let cell = [];
                if (row.length === 0) {
                    cell.push(<td key={`${isMultipleData ? '' : 'text-center'} col-job-level bold`} className={`${isMultipleData ? '' : 'text-center'} col-job-level bold`} rowSpan={getRowSpanJobType()}>
                        {
                            isMultipleData &&
                            <span className={`fa ${showHideJobOption.show ? 'fa-minus-square-o' : 'fa-plus-square-o'}`}
                                onClick={() => handleShowHideJobCells(jobType)} />
                        }
                        {jobType}
                    </td>);
                }
                if (showHideJobOption.show || !isMultipleData) {
                    cell.push(<td className="text-center col-job-level">
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
                                cell.push(
                                    <td key={`${item.name}-${gunit.name}-${unit.name}-expat`} className="text-center">
                                        {renderCell(unit.totalExpat, item.name, unit.name, false)}
                                    </td>
                                );
                                cell.push(<td key={`${item.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                    {renderCell(unit.totalLocal, item.name, unit.name, true)}
                                </td>);
                            }
                        });

                        cell.push(<td key={`${data.name}-${gunit.name}-total-expat`} className="text-center col-total">{totalExpat}</td>);
                        cell.push(<td key={`${data.name}-${gunit.name}-total-local`} className="text-center col-total">{totalLocal}</td>);
                    });
                }
               
                row.push(<tr>{cell}</tr>);
            }
        });
        return row;
    }

    return (
        <>
            {
                renderbody()
            }
            {
                renderTotal()
            }
            {
                renderJobTotal()
            }
        </>
    );
};

export default Row;