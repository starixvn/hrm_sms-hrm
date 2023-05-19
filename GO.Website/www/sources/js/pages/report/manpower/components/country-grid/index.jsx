import React, { Fragment } from 'react';
import LangHelper from '@jsroot/common/langHelper';

const Grid = ({ data }) => {
    const { headers, supervisors, employees, totalSupervisor, totalEmployee, grandTotal } = data;

    const renderRows = rows => {
        const trs = [];
        rows.map(item => {
            trs.push(
                <tr key={item.name} >
                    <td className="text-center col-job-level">{item.name}</td>
                    {
                        item.cells.map(country => {
                            const cells = [];
                            country.cells.map(unit => {
                                cells.push(<td key={`${item.name}-${country.name}-${unit.name}`} className="text-center">
                                    {unit.total > 0 ? unit.total : ''}
                                </td>);
                            });

                            return cells;
                        })
                    }
                </tr>
            );
        });

        return trs;
    };

    const renderTotalRow = row => {
        return (
            <tr className="row-total">
                <td className="text-center">{row.name}</td>
                {
                    row.cells.map(country => {
                        const cells = [];
                        country.cells.map(unit => {
                            cells.push(<td key={`total-super-${country.name}-${unit.name}`} className="text-center" >{unit.total}</td>);
                        });
                        return cells;
                    })
                }
            </tr>
        );
    };

    return (
        <table className="table group-by-country">
            <thead>
                <tr>
                    <td rowSpan={2} className="col-job-level">{LangHelper.getResource('JobLevel')}</td>
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
                {renderRows(supervisors)}
                {renderTotalRow(totalSupervisor)}
                {renderRows(employees)}
                {renderTotalRow(totalEmployee)}
                <tr className="row-total">
                    <td className="text-center">{grandTotal.name}</td>
                    {
                        grandTotal.cells.map(country =>
                            <td key={`grand-total-${country.name}`} className="text-center" colSpan={3}>{country.cells[0].total}</td>
                        )
                    }
                </tr>
            </tbody>
        </table>
    );
};

export default Grid;