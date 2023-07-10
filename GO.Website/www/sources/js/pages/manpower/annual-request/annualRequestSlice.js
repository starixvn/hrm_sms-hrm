import { createSlice } from '@reduxjs/toolkit';

const annualRequestSlice = createSlice({
    name: 'annualRequestSlice',
    initialState: {
        period: '',
        unitSelected: { value: null, label: null },
        departments: [],
        companies: [],
        request: {
            requestId: null,
            currentProcessId: null,
            verifierId: null,
            approverId: null,
            creator: null,
            statusId: null,
            isEdit: false
        },
        annualRequestFormDetails: [],
        jobLevels: []
    },
    reducers: {
        setRequest: (state, action) => {
            state.request = {
                requestId: action.payload.requestId,
                currentProcessId: action.payload.currentProcessId,
                verifierId: action.payload.verifierId,
                approverId: action.payload.approverId,
                creator: action.payload.creator,
                statusId: action.payload.statusId,
                isEdit: action.payload.isEdit
            };
        },
        setDepartments: (state, action) => {
            const departments = [];
            action.payload.map(item => {
                if (departments.findIndex(x => x.departmentName === item.departmentName) === -1) {
                    const sections = [];
                    action.payload.map(detail => {
                        if (detail.departmentName === item.departmentName && 
                            sections.findIndex(section => section === detail.sectionName) === -1) {
                            sections.push(detail.sectionName);
                        }
                    });
                    departments.push({
                        departmentName: item.departmentName,
                        sections: sections
                    });
                }
            });
            state.departments = departments;
        },
        setPeriod: (state, action) => {
            state.period = action.payload;
        },
        setUnitSelected: (state, action) => {
            state.unitSelected = action.payload;
        },
        addAnnualRequestFormDetailsItem: (state, action) => {
            const { param } = action.payload;
            const ColumnExpatAddition = 'expatAddition', ColumnLocalAddition = 'localAddition', ColumnTotalAddition = 'totalAddition';
            const index = state.annualRequestFormDetails.findIndex(x => x.unitName === param.unitName && x.departmentName === param.departmentName
                && x.sectionName === param.sectionName && x.jobLevel === param.jobLevel && x.companyName === param.companyName);
            if (index >= 0 && index <= state.annualRequestFormDetails.length) {
                state.annualRequestFormDetails[index][param.columnName] = param.value;
                const expatAddition = state.annualRequestFormDetails[index][ColumnExpatAddition];
                const localAddition = state.annualRequestFormDetails[index][ColumnLocalAddition];
                state.annualRequestFormDetails[index][ColumnTotalAddition] = expatAddition + localAddition;
            }
        },
        setCompanies: (state, action) => {
            const companies = [];
            action.payload.map(item => {
                if (companies.indexOf(item.companyName) === -1) {
                    companies.push(item.companyName)
                }
            });
            state.companies = companies;
        },
        setJobLevels: (state, action) => {
            const jobLevels = [];
            action.payload.map(item => {
                if (jobLevels.indexOf(item.jobLevel) === -1) {
                    jobLevels.push(item.jobLevel)
                }
            });
            state.jobLevels = jobLevels;
        },
        setAnnualRequestFormDetails: (state, action) => {
            state.annualRequestFormDetails = action.payload;
        }
    }
});

const { actions, reducer } = annualRequestSlice;

export const {
    setRequest,
    setDepartments,
    setPeriod,
    setCompanies,
    setJobLevels,
    setUnitSelected,
    setAnnualRequestFormDetails,
    addAnnualRequestFormDetailsItem
} = actions;

export default reducer;