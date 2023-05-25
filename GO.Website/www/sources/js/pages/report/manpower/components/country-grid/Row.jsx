import React from 'react';

const Row = ({ data, onClick }) => {
    const renderCell = (value, jobLevel, country, gunit) =>
        value > 0 ? <div className="cursor-pointer" onClick={event => onClick(event, jobLevel, country, gunit)}>{value}</div> : '';

    return (
        <tr>
            <td className="text-center col-job-level">{data.name}</td>
            {
                data.cells.map(country => {
                    const cells = [];
                    country.cells.map(unit => {
                        cells.push(<td key={`${data.name}-${country.name}-${unit.name}`} className="text-center">
                            {renderCell(unit.total, data.name, country.name, unit.name)}
                        </td>);
                    });

                    return cells;
                })
            }
        </tr>
    );
};

export default Row;