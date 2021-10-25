import { createSlice } from '@reduxjs/toolkit';
import { DefaultPageSize } from '@jsroot/common/constants';

const userSlice = createSlice({
    name: 'user',
    initialState: {
        getAllUserParams: {
            pageIndex: 1,
            pageSize: DefaultPageSize
        },
        userGridData: null,
        createModalDialog: { show: false },
        createUserResult: null
    },
    reducers: {
        setGetAllUserParams: (state, action) => {
            state.getAllUserParams = action.payload;
        },
        setUserGridData: (state, action) => {
            state.userGridData = action.payload;
        },
        setCreateModalDialog: (state, action) => {
            state.createModalDialog = action.payload;
        },
        setCreateUserResult: (state, action) => {
            state.createUserResult = action.payload;
        }
    }
});

const { actions, reducer } = userSlice;

export const {
    setGetAllUserParams,
    setUserGridData,
    setCreateModalDialog,
    setCreateUserResult
} = actions;

export default reducer;