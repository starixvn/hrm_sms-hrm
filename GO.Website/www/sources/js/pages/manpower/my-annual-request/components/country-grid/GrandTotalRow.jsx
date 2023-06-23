import React from 'react';

const GrandTotalRow = ({ data }) => {
    return (
        <tr className="row-total">
            <td className="text-center">{data.name}</td>
            {
                data.cells.map(country =>
                    <td key={`grand-total-${country.name}`} className="text-center" colSpan={3}>{country.cells[0].total}</td>
                )
            }
        </tr>
    );
};

export default GrandTotalRow;