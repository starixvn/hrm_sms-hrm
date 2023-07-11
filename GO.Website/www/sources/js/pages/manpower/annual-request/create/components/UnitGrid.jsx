import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../../action';
import LangHelper from '@jsroot/common/langHelper';

const UnitGrid = ({ departments, jobLevels, unitSelected }) => {
    const renderJobLevel = itemSection => {
        const cellJobLevel = [];
        jobLevels.map(itemJobLevel => {
            cellJobLevel.push(
                <tr key={`columnJobLevel-${itemSection + itemJobLevel}`}>
                    <td>{itemJobLevel}</td>
                </tr>
            );
        });
        cellJobLevel.push(
            <tr key={`columnJobLevelToTal-${itemSection}`}>
                <td className="data-section"></td>
            </tr>
        );
        return cellJobLevel;
    };

    const renderSection = itemDepartment => {
        let rowSpanDepartment = 2;
        const rowSpanSection = 10;
        const cellSections = [];
        itemDepartment.sections.map(itemSection => {
            rowSpanDepartment += rowSpanSection;
            const cellJobLevel = renderJobLevel(itemSection);
            cellSections.push(
                <tr key={`columnSection-${itemDepartment.departmentName + itemSection}`}>
                    <td rowSpan={rowSpanSection}
                        className="data-section">{itemSection}</td>
                </tr>
            );
            cellSections.push(cellJobLevel);
        });
        return { cellSections, rowSpanDepartment };
    };

    const renderDepartment = () => {
        let rowSpanUnit = 3;
        const cellDepartments = [];
        departments.map(itemDepartment => {
            const { cellSections, rowSpanDepartment } = renderSection(itemDepartment);
            cellDepartments.push(
                <tr key={`columnDeparment-${unitSelected.value + itemDepartment.departmentName}`}>
                    <td rowSpan={rowSpanDepartment}
                        className="data-unit">{itemDepartment.departmentName}</td>
                </tr>
            );
            cellDepartments.push(cellSections);
            cellDepartments.push(
                <tr key={`columnTotal-${unitSelected.value + itemDepartment.departmentName}`} className="data-section data-unit">
                    <td>{LangHelper.getResource('Total')}</td>
                    <td></td>
                </tr>
            );
            rowSpanUnit += rowSpanDepartment - 1;
        });
        return {cellDepartments, rowSpanUnit};
    };

    const renderbody = () => {
        const cells = [];
        const { cellDepartments, rowSpanUnit } = renderDepartment();
        cells.push(
            <tr key={`columnUnit-${unitSelected.value}`}>
                <td rowSpan={rowSpanUnit} className="data-unit" >{unitSelected.value}</td>
            </tr>
        );
        cells.push(cellDepartments);

        return cells;
    };

    if (departments.length === 0) {
        return null;
    }

    return (
        <table className="table table-bordered table-unit">
            <thead>
                <tr>
                    <th>{LangHelper.getResource('Unit')}</th>
                    <th>{LangHelper.getResource('Department')}</th>
                    <th>{LangHelper.getResource('Section')}</th>
                    <th className="jobLevel">{LangHelper.getResource('JobLevel')}</th>
                </tr>
            </thead>
            <tbody>
                {renderbody()}
            </tbody>
        </table>
    );
};
const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { departments, jobLevels, unitSelected } = state;
    return { departments, jobLevels, unitSelected };
};

export default connect(mapStateToProps, mapDispatchToProps)(UnitGrid);