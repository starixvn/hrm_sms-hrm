import React from 'react';

const UnitList = ({ units, onClick }) => {
    return (
        <div className="unit-list">
            <div className="header">Unit List</div>
            <div className="content">
                {
                    units.map(unit => {
                        return (
                            <div key={unit.name} onClick={() => onClick(unit)}>
                                <span className="mr-2">{unit.name}</span>
                                {unit.hasFlow && <i className="fa fa-check icon-check" />}
                            </div>
                        );
                    })
                }
            </div>
        </div>
    );
};

export default UnitList;