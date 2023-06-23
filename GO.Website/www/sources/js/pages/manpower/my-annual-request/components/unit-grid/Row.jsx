import React from 'react';

const Row = ({ data, showHideOptions, onClick }) => {
    const renderCell = (value, jobLevel, unitName, isLocal) =>
        value > 0 ? <div className="cursor-pointer" onClick={event => onClick(event, jobLevel, unitName, isLocal)}>{value}</div> : '';

    return (
        <tr>
            <td className="text-center col-job-level">{data.name}</td>
            {
                data.cells.map(gunit => {
                    const cells = [];
                    let totalLocal = 0,
                        totalExpat = 0;
                    const showHideOption = showHideOptions.find(x => x.key === gunit.name);

                    gunit.cells.map(unit => {
                        totalLocal += unit.totalLocal;
                        totalExpat += unit.totalExpat;

                        if (showHideOption.show) {
                            cells.push(
                                <td key={`${data.name}-${gunit.name}-${unit.name}-expat`} className="text-center">
                                    {renderCell(unit.totalExpat, data.name, unit.name, false)}
                                </td>
                            );
                            cells.push(<td key={`${data.name}-${gunit.name}-${unit.name}-local`} className="text-center">
                                {renderCell(unit.totalLocal, data.name, unit.name, true)}
                            </td>);
                        }
                    });

                    cells.push(<td key={`${data.name}-${gunit.name}-total-expat`} className="text-center col-total">{totalExpat}</td>);
                    cells.push(<td key={`${data.name}-${gunit.name}-total-local`} className="text-center col-total">{totalLocal}</td>);

                    return cells;
                })
            }
        </tr>
    );
};

export default Row;