import { createSlice } from '@reduxjs/toolkit';

const siteSlice = createSlice({
    name: 'manpowerForecast',
    initialState: {
        period: {
            periodProcessId: 1,
            fromDate: new Date('2023-06-01'),
            toDate: new Date('2023-07-01')
        },
        units: [{
            unitId: 1,
            unitName: 'BU 02'
        }],
        department: [{
                unitId: 1,
                departmentId: 1,
                departmentName: 'BC2'
            }],
        section: [{
                departmentId: 1,
                sectionId: 1,
                sectionName: 'Payment'
            },
            {
                departmentId: 1,
                sectionId: 2,
                sectionName: 'CSD'
            },
            {
                departmentId: 1,
                sectionId: 3,
                sectionName: 'Marketing'
            },
            {
                departmentId: 1,
                sectionId: 4,
                sectionName: 'Risk'
            }],
        jobLevel: [{
                jobLevelId: 1,
                jobLevelName: 'M4/P4'
            },
            {
                jobLevelId: 2,
                jobLevelName: 'M3/P3'
            },
            {
                jobLevelId: 3,
                jobLevelName: 'M2/P2'
            },
            {
                jobLevelId: 4,
                jobLevelName: 'M1/P1'
            },
            {
                jobLevelId: 5,
                jobLevelName: 'E3'
            },
            {
                jobLevelId: 6,
                jobLevelName: 'E2'
            },
            {
                jobLevelId: 7,
                jobLevelName: 'E1'
            },
            {
                jobLevelId: 8,
                jobLevelName: 'E0'
            }],
        companies: [{
                companyId: 1,
                companyName: 'CTI'
            },
            {
                companyId: 3,
                companyName: 'Nexcel'
            }]
    },
    reducers: {
        setUnits: (state, action) => {
            state.units = action.payload;
        },
        setDepartment: (state, action) => {
            state.department = action.payload;
        },
        setSection: (state, action) => {
            state.section = action.payload;
        },
        setJobLevel: (state, action) => {
            state.jobLevel = action.payload;
        },
        setCompanies: (state, action) => {
            state.companies = action.payload;
        },
    }
});

const { actions, reducer } = siteSlice;

export const {
    setUnits,
    setDepartment,
    setSection,
    setJobLevel,
    setCompanies
} = actions;

export default reducer;