import React from 'react';
import { createRoot } from 'react-dom/client';
import Layout from '@jsroot/components/Layout';
import AppContainer from './components/AppContainer';

const root = createRoot(document.getElementById('app-container'));
root.render(
    <Layout>
        <AppContainer />
    </Layout>
);