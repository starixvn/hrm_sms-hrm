import React from 'react';
import { createRoot } from 'react-dom/client';
import Layout from '@jsroot/components/Layout';
import AppContainer from './components/detail/AppContainer';

const root = createRoot(document.getElementById('detail-app-container'));
root.render(
    <Layout>
        <AppContainer />
    </Layout>
);