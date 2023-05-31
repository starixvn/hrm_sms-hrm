import React from 'react';
import { createRoot } from 'react-dom/client';
import Header from './components/Header';

if (document.getElementById('header')) {
    const root = createRoot(document.getElementById('header'));
    root.render(<Header />);
}