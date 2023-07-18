import React from 'react';

import Site from '@jsroot/common/site';

const PageNotFound = () => {
    return (
        <div className="page-error page-error-404">
            <div className="page-content vertical-align-middle">
                <header>
                    <div className="img-block">
                        <img className="img-main" src={Site.resolveClientUrl('css/images/404/404.svg')} alt="404.svg" />
                        <img className="balloon" src={Site.resolveClientUrl('css/images/404/balloon.svg')} alt="balloon.svg" />
                        <img className="small-cloud" src={Site.resolveClientUrl('css/images/404/cloud-small.svg')} alt="cloud-small.svg" />
                        <img className="big-cloud" src={Site.resolveClientUrl('css/images/404/cloud-big.svg')} alt="cloud-big.svg" />
                        <img className="wind-bottom" src={Site.resolveClientUrl('css/images/404/wind.svg')} alt="wind.svg" />
                        <img className="wind-top" src={Site.resolveClientUrl('css/images/404/wind.svg')} alt="wind.svg" />
                    </div>
                </header>
                <p className="error-advise">Oops. The page you are looking for doesn't exist.</p>
            </div>
        </div>
    );
};

export default PageNotFound;