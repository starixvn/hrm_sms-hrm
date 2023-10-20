import React from 'react';
import { Fragment } from 'react';
import { resolveClientUrl } from '../../../../shared/common.functions';

export const ExtensionControl = () => {
    const handleCreateExtension = () => {
        window.location = resolveClientUrl("ExtensionNo/Create");
    }

    return (
        <Fragment>
            {
                window.hasPermission &&
                <div className="gap-btn">
                    <a className="btn btn-request" onClick={ handleCreateExtension }>
                        <span className="fa fa-plus"></span> New Extension Number
                    </a>
                    <a className="btn btn-primary"> Manage External Group</a >
                </div>
            }
        </Fragment>
    );
}