import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import { ActionCreators } from '../../action';
import LangHelper from '@jsroot/common/langHelper';

const UnitGrid = ({ departments, jobLevels, unitSelected }) => {
    const renderJobLevel = sectionName => {
        const cells = [];
        jobLevels.map(jobLevel => {
            cells.push(
                <tr key={`col-jobLevel-${sectionName}-${jobLevel}`}>
                    <td>{jobLevel}</td>
                </tr>
            );
        });
        cells.push(
            <tr key={`col-jobLevel-total-${sectionName}`}>
                <td className="data-section"></td>
            </tr>
        );
        return cells;
    };

    const renderSection = deptItem => {
        let deptRowSpan = 2;
        const sectionRowSpan = 10;
        const sectionCells = [];
        deptItem.sections.map(sectionName => {
            deptRowSpan += sectionRowSpan;
            sectionCells.push(
                <tr key={`col-section-${deptItem.departmentName}-${sectionName}`}>
                    <td rowSpan={sectionRowSpan} className="data-section">{sectionName}</td>
                </tr>
            );
            sectionCells.push(renderJobLevel(sectionName));
        });
        return { sectionCells, deptRowSpan };
    };

    const renderDepartment = () => {
        let unitRowSpan = 3;
        const deptCells = [];
        departments.map(deptItem => {
            const { sectionCells, deptRowSpan } = renderSection(deptItem);
            deptCells.push(
                <tr key={`col-dept-${unitSelected.value}-${deptItem.departmentName}`}>
                    <td rowSpan={deptRowSpan} className="data-unit">{deptItem.departmentName}</td>
                </tr>
            );
            deptCells.push(sectionCells);
            deptCells.push(
                <tr key={`col-total-${unitSelected.value}-${deptItem.departmentName}`} className="data-section data-unit">
                    <td>{LangHelper.getResource('Total')}</td>
                    <td></td>
                </tr>
            );
            unitRowSpan += deptRowSpan - 1;
        });
        return { deptCells, unitRowSpan };
    };

    const renderbody = () => {
        const cells = [];
        const { deptCells, unitRowSpan } = renderDepartment();
        cells.push(
            <tr key={`col-unit-${unitSelected.value}`}>
                <td rowSpan={unitRowSpan} className="data-unit">{unitSelected.value}</td>
            </tr>
        );
        cells.push(deptCells);

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
                    <th className="job-level">{LangHelper.getResource('JobLevel')}</th>
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