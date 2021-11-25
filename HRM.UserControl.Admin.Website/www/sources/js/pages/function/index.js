import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { configureStore } from '@reduxjs/toolkit';

import Layout from '@jsroot/components/Layout';
import functionReducer from './functionSlice';
import AppContainer from './components/AppContainer';

const store = configureStore({
    reducer: functionReducer
});

ReactDOM.render(
    <Provider store={store}>
        <Layout>
            <AppContainer />
        </Layout>
    </Provider>,
    document.getElementById('app-container'));