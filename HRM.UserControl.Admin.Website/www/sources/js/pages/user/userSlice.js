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
        createUserResult: null,
        resetPasswordModalDialog: { show: false },
        resetPasswordResult: null
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
        },
        setResetPasswordModalDialog: (state, action) => {
            state.resetPasswordModalDialog = action.payload;
        },
        setResetPasswordResult: (state, action) => {
            state.resetPasswordResult = action.payload;
        }
    }
});

const { actions, reducer } = userSlice;

export const {
    setGetAllUserParams,
    setUserGridData,
    setCreateModalDialog,
    setCreateUserResult,
    setResetPasswordModalDialog,
    setResetPasswordResult
} = actions;

export default reducer;