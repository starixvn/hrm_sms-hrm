import React, { Fragment } from 'react';
import { resolveClientUrl } from '../../../shared/common.functions';

export const Filter = (props) => {
    const { setChoose, submit } = props;

    const handleCreateModal = () => {
        window.location = resolveClientUrl("Department/Create");
    }
    return (
        <form className="filter" onSubmit={submit}>
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Keyword</label>
                    </div>
                    <div className="input-form">
                        <input name="name" className="form-control" placeholder="job title" autoComplete="off" />
                    </div>
                </div>
                <div className="form-group form-group--m">
                    <button type="submit" className="btn btn-primary">Search</button>
                    {
                        window.hasPermission &&
                        <Fragment>
                            <button type="submit" className="btn btn-excel">Excel</button>
                            <button className="btn btn-request" type="button" onClick={handleCreateModal}>
                                <span className="fa fa-plus" /> &nbsp;New Job Title
                            </button>
                        </Fragment>
                    }
                </div>
            </div>
        </form>
    )
};