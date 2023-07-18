import React from 'react';

import Site from '@jsroot/common/site';

const AccessDenied = () => {
    return (
        <div className="page-error page-error-405">
            <div className="page-content vertical-align-middle">
                <div className="img-block">
                    <img className="door" src={Site.resolveClientUrl('css/images/405/door.svg')} alt="door.svg" />
                    <div className="seal-slideshow">
                        <div className="mover-1"></div>
                    </div>

                    <img className="signage" src={Site.resolveClientUrl('css/images/405/signage.svg')} alt="signage.svg" />
                </div>
                <p className="error-advise">Oops. 405, ACCESS DENIED</p>
                <p className="small-text">
                    Sorry about that, but you don't have permission to access this page.
                </p>
            </div>
        </div>
    );
};

export default AccessDenied;