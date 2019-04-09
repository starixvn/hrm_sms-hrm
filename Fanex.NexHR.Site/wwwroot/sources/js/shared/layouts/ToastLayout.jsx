import React from 'react';
import { ToastContainer, Slide } from 'react-toastify';

const ToastLayout = props => {
    return (
        <ToastContainer
            position="top-center"
            autoClose={3000}
            hideProgressBar={false}
            transition={Slide}
            closeOnClick={false}
            draggable={false}
        >
            {props.children}
        </ToastContainer>
    );
};

export default ToastLayout;