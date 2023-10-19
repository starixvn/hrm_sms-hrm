import React from 'react';

import { renderNumber } from '../../common.funcs';

const TotalRow = ({ data }) => {
    return (
        <>
            <tr className="row-total">
                <td className="text-center" colSpan={2} rowSpan={2}>{data.name}</td>
                {
                    data.cells.map(country => {
                        const cells = [];
                        country.cells.map(unit => {
                            cells.push(<td key={`total-super-${country.name}-${unit.name}`} className="text-center">{renderNumber(unit.total)}</td>);
                        });
                        return cells;
                    })
                }
            </tr>
            <tr className="row-total">
                {
                     data.cells.map(country => {
                        const cells = [];
                        let total = 0;
                        country.cells.map(unit => {
                            total += unit.total;
                        });
                         cells.push(<td colSpan={3} key={`total-super-${country.name}`} className="text-center">{renderNumber(total)}</td>);
                        return cells;
                    })
                }
            </tr>
        </>
    );
};

export default TotalRow;