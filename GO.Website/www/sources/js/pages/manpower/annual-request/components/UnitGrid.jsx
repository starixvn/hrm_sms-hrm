import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';

const UnitGrid = ({ units }) => {
    const renderJobLevel = itemSection => {
        const cellJobLevel = [];
        itemSection.jobLevels.map(itemJobLevel => {
            cellJobLevel.push(
                <tr key={`columnJobLevel-${itemSection.sectionName + itemJobLevel}`}>
                    <td>{itemJobLevel}</td>
                </tr>
            );
        });
        cellJobLevel.push(
            <tr key={`columnJobLevelToTal-${itemSection.sectionName}`}>
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
            if (itemSection.departmentId === itemDepartment.departmentId) {
                rowSpanDepartment += rowSpanSection;
                const cellJobLevel = renderJobLevel(itemSection);
                cellSections.push(
                    <tr key={`columnSection-${itemDepartment.departmentName + itemSection.sectionName}`}>
                        <td rowSpan={rowSpanSection}
                            className="data-section">{itemSection.sectionName}</td>
                    </tr>
                );
                cellSections.push(cellJobLevel);
            }
        });
        return { cellSections, rowSpanDepartment };
    };

    const renderDepartment = itemUnit => {
        let rowSpanUnit = 4;
        const cellDepartments = [];
        itemUnit.departments.map(itemDepartment => {
            if (itemDepartment.unitId === itemUnit.unitId) {
                const { cellSections, rowSpanDepartment } = renderSection(itemDepartment);
                cellDepartments.push(
                    <tr key={`columnDeparment-${itemUnit.unitName + itemDepartment.departmentName}`}>
                        <td rowSpan={rowSpanDepartment}
                            className="data-unit">{itemDepartment.departmentName}</td>
                    </tr>
                );
                cellDepartments.push(cellSections);
                cellDepartments.push(
                    <tr key={`columnTotal-${itemUnit.unitName + itemDepartment.departmentName}`} className="data-section data-unit">
                        <td>{LangHelper.getResource('Total')}</td>
                        <td></td>
                    </tr>
                );
                rowSpanUnit += rowSpanDepartment - 1;
            }
        });
        return {cellDepartments, rowSpanUnit};
    };

    const renderbody = () => {
        const cells = [];
        units.map(itemUnit => {
            const { cellDepartments, rowSpanUnit } = renderDepartment(itemUnit);
            cells.push(
                <tr key = {`columnUnit-${itemUnit.unitName}`}>
                    <td rowSpan={rowSpanUnit} className="data-unit" >{itemUnit.unitName}</td>
                </tr>
            );
            cells.push(cellDepartments);
        });

        return cells;
    };
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

const mapStateToProps = state => {
    const { units } = state;
    return { units };
};

export default connect(mapStateToProps, null)(UnitGrid);