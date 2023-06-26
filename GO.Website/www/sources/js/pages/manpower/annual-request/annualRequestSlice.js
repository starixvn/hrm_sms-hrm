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