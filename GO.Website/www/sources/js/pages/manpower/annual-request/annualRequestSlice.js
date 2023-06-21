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
    setAnnualRequestFormDetails
} = actions;

export default reducer;