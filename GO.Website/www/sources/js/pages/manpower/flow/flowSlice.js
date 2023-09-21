import { createSlice } from '@reduxjs/toolkit';
import { ManpowerFlow } from '@jsroot/common/constants';

const siteSlice = createSlice({
    name: 'manpowerFlow',
    initialState: {
        units: [],
        selectedUnit: null,
        flows: [],
        showDeleteModalDialog: false
    },
    reducers: {
        setUnits: (state, action) => {
            state.units = action.payload;
        },
        setSelectedUnit: (state, action) => {
            state.selectedUnit = action.payload;
        },
        setFlows: (state, action) => {
            state.flows = action.payload;
        },
        addFlowItem: (state, action) => {
            const { processId, employee } = action.payload;
            const flow = {
                processId,
                siteId: employee.siteId,
                siteName: employee.siteName,
                companyId: employee.companyId,
                companyName: employee.companyName,
                empId: employee.empId,
                code: employee.code,
                nickname: employee.nickname,
                fullName: employee.fullName,
                isActive: employee.isActive,
                isDeleted: employee.isDeleted
            };
            if (flow.processId === ManpowerFlow.UnitHead) {
                state.flows.push(flow);
            }
            else {
                const index = state.flows.findIndex(x => x.processId === processId);
                if (index < 0) {
                    state.flows.push(flow);
                } else {
                    if (state.flows[index].siteId !== employee.siteId || state.flows[index].empId !== employee.empId) {
                        state.flows[index] = flow;
                    }
                }
            }
        },
        removeFlowItem: (state, action) => {
            const { processId, employee } = action.payload;
            const index = state.flows.findIndex(x => x.processId === processId && x.siteId === employee.siteId && x.empId === employee.empId);
            state.flows.splice(index, 1);
        },
        setShowDeleteModalDialog: (state, action) => {
            state.showDeleteModalDialog = action.payload;
        }
    }
});

const { actions, reducer } = siteSlice;

export const {
    setUnits,
    setSelectedUnit,
    setFlows,
    addFlowItem,
    removeFlowItem,
    setShowDeleteModalDialog
} = actions;

export default reducer;