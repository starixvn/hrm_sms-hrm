import { createSlice } from '@reduxjs/toolkit';
import { Equal } from '@jsroot/common/string.funcs';

const annualRequestSlice = createSlice({
    name: 'annualRequestSlice',
    initialState: {
        period: '',
        unitSelected: { value: null, label: null },
        departments: [],
        companies: [],
        request: null,
        submitModalDialog: false,
        verifyApproveModalDialog: false,
        resetModalDialog: false,
        requestDetails: [],
        logDetails: [],
        jobLevels: [],
        logDetailModalDialog: {
            show: false,
            logs: [],
            requestDetail: null
        }
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
                if (departments.findIndex(x => Equal(x.departmentName, item.departmentName)) === -1) {
                    const sections = [];
                    action.payload.map(detail => {
                        if (Equal(detail.departmentName, item.departmentName)
                            && sections.findIndex(section => Equal(section, detail.sectionName)) === -1) {
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

            const index = state.requestDetails.findIndex(x => x.unitName === unitName && Equal(x.departmentName, departmentName)
                && Equal(x.sectionName, sectionName) && x.jobLevel === jobLevel && x.companyName === companyName);
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
        setLogDetails: (state, action) => {
            state.logDetails = action.payload;
        },
        setSubmitModalDialog: (state, action) => {
            state.submitModalDialog = action.payload;
        },
        setverifyApproveModalDialog: (state, action) => {
            state.verifyApproveModalDialog = action.payload;
        },
        setResetModalDialog: (state, action) => {
            state.resetModalDialog = action.payload;
        },
        setLogDetailModalDialog: (state, action) => {
            state.logDetailModalDialog = action.payload;
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
    setLogDetails,
    addRequestDetailsItem,
    setSubmitModalDialog,
    setverifyApproveModalDialog,
    setResetModalDialog,
    setLogDetailModalDialog
} = actions;

export default reducer;