import React from 'react';
import { ToastContainer, Slide } from 'react-toastify';

const Layout = ({ children }) => {
    return (
        <>
            <ToastContainer
                limit={1}
                position='top-center'
                autoClose={3000}
                hideProgressBar={false}
                transition={Slide}
                closeOnClick={false}
                pauseOnHover
                draggable={false}
            />
            {children}
        </>
    );
};

export default Layout;