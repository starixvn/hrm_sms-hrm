import React from 'react';
import { connect } from 'react-redux';

import LangHelper from '@jsroot/common/langHelper';

const UnitGrid = ({ units, department, section, jobLevel }) => {
    const renderJobLevel = () => {
        const cellJobLevel = [];
        jobLevel.map(itemJobLevel => {
            cellJobLevel.push(
                <tr>
                    <td>{itemJobLevel.jobLevelName}</td>
                </tr>
            );
        });
        cellJobLevel.push(
            <tr>
                <td className="data-section"></td>
            </tr>
        );
        return cellJobLevel;
    };

    const renderSection = itemDepartment => {
        let rowSpanDepartment = 2;
        const rowSpanSection = 10;
        const cellSections = [];
        section.map(itemSection => {
            if (itemSection.departmentId === itemDepartment.departmentId) {
                rowSpanDepartment += rowSpanSection;
                const cellJobLevel = renderJobLevel();
                cellSections.push(
                    <tr>
                        <td rowSpan={rowSpanSection} className="data-section">{itemSection.sectionName}</td>
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
        department.map(itemDepartment => {
            if (itemDepartment.unitId === itemUnit.unitId) {
                const { cellSections, rowSpanDepartment } = renderSection(itemDepartment);
                cellDepartments.push(
                    <tr>
                        <td rowSpan={rowSpanDepartment} className="data-unit">{itemDepartment.departmentName}</td>
                    </tr>
                );
                cellDepartments.push(cellSections);
                cellDepartments.push(
                    <tr className="data-section data-unit">
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
                <tr>
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
    const { units, department, section, jobLevel } = state;
    return { units, department, section, jobLevel };
};

export default connect(mapStateToProps, null)(UnitGrid);