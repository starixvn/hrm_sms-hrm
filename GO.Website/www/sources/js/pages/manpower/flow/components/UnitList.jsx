import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../actions';

const UnitList = ({ units, selectedUnit, setSelectedUnit, getFlowByUnit }) => {
    const handleSelectUnit = unit => {
        if (selectedUnit === null || selectedUnit.name !== unit.name) {
            setSelectedUnit(unit);

            getFlowByUnit({ unitName: unit.name });
        }
    };

    return (
        <div className="unit-list">
            <div className="header">Unit List</div>
            <div className="content">
                {
                    units.map(unit => {
                        return (
                            <div key={unit.name} onClick={() => handleSelectUnit(unit)}>
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

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { units, selectedUnit } = state;
    return { units, selectedUnit };
};

export default connect(mapStateToProps, mapDispatchToProps)(UnitList);