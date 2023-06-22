import { createSlice } from '@reduxjs/toolkit';

const annualRequestSlice = createSlice({
    name: 'annualRequestSlice',
    initialState: {
        period: '',
        units: [],
        companies: [],
        annualRequestFormDetails:[]
    },
    reducers: {
        setUnits: (state, action) => {
            state.units = action.payload;
        },
        setPeriod: (state, action) => {
            state.period = action.payload;
        },
        addAnnualRequestFormDetailsItem: (state, action) => {
            const { param } = action.payload;
            const index = state.annualRequestFormDetails.findIndex(x => x.unitName === param.unitName && x.departmentName === param.departmentName
                && x.sectionName === param.sectionName && x.jobLevel === param.jobLevel && x.companyName === param.companyName);
            if (index >= 0 && index <= state.annualRequestFormDetails.length) {
                state.annualRequestFormDetails[index][param.columnName] = parseInt(param.value, 10);
            }
        },
        setCompanies: (state, action) => {
            state.companies = action.payload;
        },
        setAnnualRequestFormDetails: (state, action) => {
            state.annualRequestFormDetails = action.payload;
        }
    }
});

const { actions, reducer } = annualRequestSlice;

export const {
    setUnits,
    setPeriod,
    setCompanies,
    setAnnualRequestFormDetails,
    addAnnualRequestFormDetailsItem
} = actions;

export default reducer;