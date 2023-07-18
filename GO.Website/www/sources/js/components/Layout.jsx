import axios from 'axios';
import React, { useEffect, useState } from 'react';
import { ToastContainer, Slide } from 'react-toastify';

import PageNotFound from './error/PageNotFound';
import AccessDenied from './error/AccessDenied';
import InternalError from './error/InternalError';

import Site from '@jsroot/common/site';

const Layout = ({ children }) => {
    const [errorStatus, setErrorStatus] = useState(null);

    const interceptor = axios.interceptors.response.use(response => {
        return response;
    }, error => {
        switch (error.response.status) {
            case 401:
                window.location = 'auth/logout';
                break;

            case 404:
            case 405:
            case 500:
                Site.unblockUI();
                setErrorStatus(error.response.status);
                break;
        }

        return Promise.reject(error);
    });

    useEffect(() => {
        return () => {
            axios.interceptors.response.eject(interceptor);
        };
    }, []);

    if (errorStatus !== null) {
        switch (errorStatus) {
            case 404:
                return <PageNotFound />;

            case 405:
                return <AccessDenied />;

            case 500:
                return <InternalError />;
        }
    }

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
                theme="colored"
            />
            {children}
        </>
    );
};

export default Layout;