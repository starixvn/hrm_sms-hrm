import React, { Fragment, PureComponent } from 'react';
import { ToastContainer, Slide } from 'react-toastify';

export default class Layout extends PureComponent {
    render() {
        return (
            <Fragment>
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
                {this.props.children}
            </Fragment>
        );
    }
}