import React from 'react';

const TotalRow = ({ data }) => {
    return (
        <tr className="row-total">
            <td className="text-center">{data.name}</td>
            {
                data.cells.map(country => {
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

export default TotalRow;