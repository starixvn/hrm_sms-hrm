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
        submitModalDialog: false,
        verifyModalDialog: false,
        approveModalDialog: false,
        resetModalDialog: false,
        requestDetails: [],
        jobLevels: []
    },
    reducers: {
        setRequest: (state, action) => {
            state.request = {
                requestId: action.payload.requestId,
                currentProcessId: action.payload.currentProcessId,
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
                        if (detail.departmentName === item.departmentName
                            && sections.findIndex(section => section === detail.sectionName) === -1) {
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
        addRequestDetailsItem: (state, action) => {
            const { companyName, unitName, departmentName, sectionName, jobLevel, columnName, value } = action.payload;

            const index = state.requestDetails.findIndex(x => x.unitName === unitName && x.departmentName === departmentName
                && x.sectionName === sectionName && x.jobLevel === jobLevel && x.companyName === companyName);
            if (index >= 0) {
                state.requestDetails[index][columnName] = value;

                const expatAddition = state.requestDetails[index]['expatAddition'];
                const localAddition = state.requestDetails[index]['localAddition'];
                state.requestDetails[index]['totalAddition'] = expatAddition + localAddition;
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
        setRequestDetails: (state, action) => {
            state.requestDetails = action.payload;
        },
        setSubmitModalDialog: (state, action) => {
            state.submitModalDialog = action.payload;
        },
        setVerifyModalDialog: (state, action) => {
            state.verifyModalDialog = action.payload;
        },
        setApproveModalDialog: (state, action) => {
            state.approveModalDialog = action.payload;
        },
        setResetModalDialog: (state, action) => {
            state.resetModalDialog = action.payload;
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
    setRequestDetails,
    addRequestDetailsItem,
    setSubmitModalDialog,
    setVerifyModalDialog,
    setApproveModalDialog,
    setResetModalDialog
} = actions;

export default reducer;