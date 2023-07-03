import { createSlice } from '@reduxjs/toolkit';

const annualRequestSlice = createSlice({
    name: 'annualRequestSlice',
    initialState: {
        period: '',
        unitSelected: { value: null, label: null },
        departments: [],
        companies: [],
        request: null,
        annualRequestFormDetails: [],
        jobLevels: []
    },
    reducers: {
        setRequest: (state, action) => {
            state.request = {
                currentProcessId: action.payload.currentProcessId,
                verifierId: action.payload.verifierId,
                approverId: action.payload.approverId,
                creator: action.payload.creator,
                isEdit: action.payload.isEdit
            };
        },
        setDepartments: (state, action) => {
            state.departments = action.payload;
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