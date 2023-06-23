import React from 'react';

const GrandTotalRow = ({ data, showHideOptions }) => {
    return (
        <tr className="row-total">
            <td className="text-center">{data.name}</td>
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
    );
};

export default GrandTotalRow;