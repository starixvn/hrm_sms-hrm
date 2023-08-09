import React from 'react';
import { Provider } from 'react-redux';
import { createRoot } from 'react-dom/client';
import { configureStore } from '@reduxjs/toolkit';

import Layout from '@jsroot/components/Layout';
import AppContainer from './components/AppContainer';
import roleAssignmentReducer from './roleAssignmentSlice';

const store = configureStore({
    reducer: roleAssignmentReducer
});

const root = createRoot(document.getElementById('app-container'));
root.render(
    <Provider store={store}>
        <Layout>
            <AppContainer />
        </Layout>
    </Provider>
);