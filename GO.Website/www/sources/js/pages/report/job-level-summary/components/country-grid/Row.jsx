import React from 'react';

const Row = ({ data, jobType, totalJobType, showHideOptions, handleShowHideCells, onClick }) => {
    const renderCell = (value, jobLevel, country, gunit) =>
        value > 0 ? <div className="cursor-pointer" onClick={event => onClick(event, jobLevel, country, gunit)}>{value}</div> : '';

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
                cell.push(<td key={`${item.name}-totalrow-jobtotal-name`} colSpan={2} className="text-center col-job-level bold">Total {jobType}</td>);
                item.cells.map(country => {
                    let totalCompany = 0;
                    country.cells.map(gunit => {
                        totalCompany += gunit.total;
                    });
                    cell.push(<td colSpan={3} key={`${country.name}-${jobType}-total`} className="text-center col-total">{totalCompany}</td>);
                })
                total.push(<tr key={`${item.name}-totalrow-jobtotal`}>{cell}</tr>);
            }
        });
        return total;
    }

    const renderTotal = () => {
        const total = [];
        totalJobType.map(item => {
            if (item.name.slice(0, 2) === jobType && isMultipleData) {
                let cell = [];
                cell.push(<td key={`${item.name}-totalrow-name`} className="text-center col-job-level">Total</td>);
                item.cells.map(country => {
                    country.cells.map(gunit => {
                        cell.push(<td key={`${country.name}-${gunit.name}-${jobType}-total`} className="text-center col-total">{gunit.total}</td>);
                    });
                })
                
                total.push(<tr key={`${item.name}-totalrow`} >{cell}</tr>);
            }
        });
        return total;
    }

    const renderbody = () => {
        const row = [];
        data.map(item => {
            if (item.name.slice(0, 2) === jobType) {
                const showHideOption = showHideOptions.find(x => x.key === jobType);
                let cells = [];
                if (row.length === 0) {
                    cells.push(<td key={`${item.name}-jobtype-row-name`} className={`${isMultipleData ? '' : 'text-center'} col-job-level bold`} rowSpan={getRowSpanJobType()}>
                        {
                            isMultipleData &&
                            <span className={`fa ${showHideOption.show ? 'fa-minus-square-o' : 'fa-plus-square-o'}`}
                                onClick={() => handleShowHideCells(jobType)}/>
                        }
                        {jobType}
                    </td>);
                }

                if (showHideOption.show || !isMultipleData) {
                    cells.push(<td key={`${item.name}-jobtype-data`} className="text-center col-job-level">
                        {isMultipleData ? item.name : 'Total'}
                    </td>);

                    item.cells.map(country => {
                        country.cells.map(unit => {
                            cells.push(<td key={`${item.name}-${country.name}-${unit.name}`} className="col-total text-center">
                                {renderCell(unit.total, item.name, country.name, unit.name)}
                            </td>);
                        });
                    })
                }
                row.push(<tr key={`${item.name}-row-data`}>{cells}</tr>);
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