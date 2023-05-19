import React, { useState } from 'react';
import LangHelper from '@jsroot/common/langHelper';

const Grid = ({ data }) => {
    const { headers, supervisors, employees, totalSupervisor, totalEmployee, grandTotal } = data;

    const showHides = headers.map(gunit => {
        return {
            key: gunit.name,
            show: false
        };
    });
    const [showHideOptions, setStateValues] = useState(showHides);

    const onClick = unit => {
        const options = [...showHideOptions];
        const index = options.findIndex(x => x.key === unit);
        options[index] = { ...options[index], show: !options[index].show };
        setStateValues(options);
    };

    const renderRows = rows => {
        const trs = [];
        rows.map(item => {
            trs.push(
                <tr key={item.name}>
                    <td className="text-center col-job-level">{item.name}</td>
                    {
                        item.cells.map(gunit => {
                            const cells = [];
                            let totalLocal = 0,
                                totalExpat = 0;
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                            gunit.cells.map(unit => {
                                totalLocal += unit.totalLocal;
                                totalExpat += unit.totalExpat;

                                if (showHideOption.show) {
                                    cells.push(<td key={`${item.name}-${gunit.name}-${unit.name}-expat`} className="text-center">
                                        {unit.totalExpat > 0 ? unit.totalExpat : ''}
                                    </td>);
                                    cells.push(<td key={`${item.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                        {unit.totalLocal > 0 ? unit.totalLocal : ''}
                                    </td>);
                                }
                            });

                            cells.push(<td key={`${item.name}-${gunit.name}-total-expat`} className="text-center col-total">{totalExpat}</td>);
                            cells.push(<td key={`${item.name}-${gunit.name}-total-local`} className="text-center col-total">{totalLocal}</td>);

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
                    row.cells.map(gunit => {
                        const cells = [];
                        let totalLocal = 0,
                            totalExpat = 0;
                        const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                        gunit.cells.map(unit => {
                            totalLocal += unit.totalLocal;
                            totalExpat += unit.totalExpat;

                            if (showHideOption.show) {
                                cells.push(<td key={`total-super-${gunit.name}-${unit.name}-expat`} className="text-center" >{unit.totalExpat}</td>);
                                cells.push(<td key={`total-super-${gunit.name}-${unit.name}-local`} className="text-center" >{unit.totalLocal}</td>);
                            }
                        });

                        cells.push(<td key={`total-super-${gunit.name}-expat`} className="text-center">{totalExpat}</td>);
                        cells.push(<td key={`total-super-${gunit.name}-local`} className="text-center">{totalLocal}</td>);

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
                    <td rowSpan={3} className="col-job-level">{LangHelper.getResource('JobLevel')}</td>
                    {
                        headers.map(gunit => {
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);
                            const colSpan = (showHideOption.show ? gunit.cells.length * 2 : 0) + 2;

                            return (
                                <td key={gunit.name} className="group-unit" colSpan={colSpan}>
                                    <span>{gunit.name}</span>
                                    <span
                                        className={`fa ${showHideOption.show ? 'fa-minus-square-o' : 'fa-plus-square-o'}`}
                                        onClick={() => onClick(gunit.name)}
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
                {renderRows(supervisors)}
                {renderTotalRow(totalSupervisor)}
                {renderRows(employees)}
                {renderTotalRow(totalEmployee)}
                <tr className="row-total">
                    <td className="text-center">{grandTotal.name}</td>
                    {
                        grandTotal.cells.map(gunit => {
                            const cells = [];
                            let totalLocal = 0,
                                totalExpat = 0;
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                            gunit.cells.map(unit => {
                                totalLocal += unit.totalLocal;
                                totalExpat += unit.totalExpat;

                                if (showHideOption.show) {
                                    cells.push(<td key={`grand-total-${gunit.name}-${unit.name}-expat`} className="text-center" >{unit.totalExpat}</td>);
                                    cells.push(<td key={`grand-total-${gunit.name}-${unit.name}-local`} className="text-center" >{unit.totalLocal}</td>);
                                }
                            });

                            cells.push(<td key={`grand-total-${gunit.name}-expat`} className="text-center">{totalExpat}</td>);
                            cells.push(<td key={`grand-total-${gunit.name}-local`} className="text-center">{totalLocal}</td>);

                            return cells;
                        })
                    }
                </tr>
            </tbody>
        </table>
    );
};

export default Grid;