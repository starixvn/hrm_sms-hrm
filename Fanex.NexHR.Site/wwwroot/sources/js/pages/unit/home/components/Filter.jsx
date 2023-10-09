import React from 'react';

export const Filter = (props) => {
    const { setChoose, submit } = props;
    return (
        <form className="filter" onSubmit={submit }>
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
                    window.hasPermission === 'True' &&
                    <div className="form-group form-group--m">
                        <button type="submit" className="btn btn-primary">Search</button>
                        <a className="btn btn-request">
                            <span className="fa fa-plus">
                            </span>
                            &nbsp;New Unit
                        </a>
                    </div>
                }
            </div>
        </form>
    )
};