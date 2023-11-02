import React, { useState } from 'react';

import { resolveClientUrl } from '../../../../shared/common.functions';

const Filter = ({ onSubmit }) => {
    const [status, setStatus] = useState('');

    const handleCreateUnit = () => {
        window.location = resolveClientUrl('Unit/Create');
    };

    const handleChangeStatus = event => {
        setStatus(event.target.value);
    };

    const handleSubmit = event => {
        event.preventDefault();
        onSubmit(status)
    };

    return (
        <form className="filter" onSubmit={handleSubmit}>
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form label--xs">
                        <label className="control-label">Status</label>
                    </div>
                    <div className="select-form">
                        <select className="form-control" onChange={handleChangeStatus}>
                            <option value=''>All</option>
                            <option value='true'>Active</option>
                            <option value='false'>Inactive</option>
                        </select>
                    </div>
                </div>
                <div className="form-group form-group--m">
                    <button type="submit" className="btn btn-primary">Search</button>
                    {
                        window.hasPermission === 'True' &&
                        <a className="btn btn-request" onClick={handleCreateUnit}>
                            <span className="fa fa-plus" />
                            &nbsp;New Unit
                        </a>
                    }
                </div>
            </div>
        </form>
    );
};

export default Filter;