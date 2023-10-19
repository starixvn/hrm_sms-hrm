import React from 'react';

import { renderNumber } from '../../common.funcs';

const GrandTotalRow = ({ data, showHideOptions }) => {
    return (
        <>
            <tr className="row-total">
                <td className="text-center" colSpan={2} rowSpan={2}>{data.name}</td>
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
                                cells.push(<td key={`grand-total-${gunit.name}-${unit.name}-expat`} className="text-center">{renderNumber(unit.totalExpat)}</td>);
                                cells.push(<td key={`grand-total-${gunit.name}-${unit.name}-local`} className="text-center">{renderNumber(unit.totalLocal)}</td>);
                            }
                        });

                        cells.push(<td key={`grand-total-${gunit.name}-expat`} className="text-center">{renderNumber(totalExpat)}</td>);
                        cells.push(<td key={`grand-total-${gunit.name}-local`} className="text-center">{renderNumber(totalLocal)}</td>);

                        return cells;
                    })
                }
            
            </tr>
            <tr className="row-total">
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
                                cells.push(<td colSpan={2} key={`grand-total-${gunit.name}-${unit.name}-total`} className="text-center">
                                    {renderNumber(unit.totalExpat + unit.totalLocal)}
                                </td>);
                            }
                        });

                        cells.push(<td colSpan={2} key={`grand-total-${gunit.name}-total`} className="text-center">{renderNumber(totalExpat + totalLocal)}</td>);

                        return cells;
                    })
                }
            </tr>
        </>
    );
};

export default GrandTotalRow;