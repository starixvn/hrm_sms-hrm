import React from 'react';

import Site from '@jsroot/common/site';

const InternalError = () => {
    return (
        <div className="page-error page-error-500">
            <div className="page-content vertical-align-middle">
                <header>
                    <div className="img-block">
                        <img src={Site.resolveClientUrl('css/images/500/desktop.svg')} alt="desktop.svg" />
                        <div className="screen">
                            <img src={Site.resolveClientUrl('css/images/500/bluscreen.svg')} alt="bluscreen.svg" />
                            <img className="error" src={Site.resolveClientUrl('css/images/500/Error.svg')} alt="Error.svg" />
                        </div>
                        <img className="leave" src={Site.resolveClientUrl('css/images/500/leave.svg')} alt="leave.svg" />
                    </div>
                </header>
                <p className="big-text">500</p>
                <p className="error-advise text-uppercase">internal error</p>
                <p className="small-text">Oh dear, it looks like something went wrong. We'll take a look at that shortly.</p>
            </div>
        </div>
    );
};

export default InternalError;