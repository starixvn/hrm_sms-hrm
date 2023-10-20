import React from 'react';
import { resolveClientUrl } from '../../../../shared/common.functions';

export const Filter = (props) => {
    const { setChoose, submit } = props;
    const handleCreateDepartment = () => {
        window.location = resolveClientUrl("Department/Create");
    }
    return (
        <form className="filter" onSubmit={submit}>
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form label--xs">
                        <label className="control-label">Status</label>
                    </div>
                    <div className="select-form">
                        <select className="form-control" onChange={e => setChoose(e.target.value)}>
                            <option value=''>All</option>
                            <option value='true'>Active</option>
                            <option value='false'>Inactive</option>
                        </select>
                    </div>
                </div>
                {
                    window.hasPermission &&
                    <div className="form-group form-group--m">
                        <button type="submit" className="btn btn-primary">Search</button>
                        <a className="btn btn-request" onClick={handleCreateDepartment}>
                            <span className="fa fa-plus" />
                            &nbsp;New Department
                        </a>
                    </div>
                }
            </div>
        </form>
    )
};