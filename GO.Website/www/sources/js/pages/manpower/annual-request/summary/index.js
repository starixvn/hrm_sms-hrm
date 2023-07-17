import React from 'react';
import { Provider } from 'react-redux';
import { createRoot } from 'react-dom/client';
import { configureStore } from '@reduxjs/toolkit';

import Layout from '@jsroot/components/Layout';
import AppContainer from './components/AppContainer';
import annualRequestSlice from '../annualRequestSlice';

const store = configureStore({
    reducer: annualRequestSlice
});

const root = createRoot(document.getElementById('app-container'));
root.render(
    <Provider store={store}>
        <Layout>
            <AppContainer />
        </Layout>
    </Provider>
);