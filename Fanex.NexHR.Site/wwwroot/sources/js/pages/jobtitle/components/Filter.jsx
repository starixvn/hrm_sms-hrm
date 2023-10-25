import React, { Fragment } from 'react';
import { resolveClientUrl } from '../../../shared/common.functions';

export const Filter = (props) => {
    const { setKeyword, searchKeyword, setCreateModal} = props;

    const handleCreateModal = () => {
        setCreateModal(true);
    }

    return (
        <div className="filter">
            <div className="form-horizontal clearfix">
                <div className="form-group">
                    <div className="label-form">
                        <label className="control-label">Keyword</label>
                    </div>
                    <div className="input-form">
                        <input name="name"
                            className="form-control"
                            placeholder="job title"
                            autoComplete="off"
                            onChange={e => setKeyword(e.target.value)} />
                    </div>
                </div>
                <div className="form-group form-group--m">
                    <button type="submit" className="btn btn-primary" onClick={searchKeyword}>Search</button>
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
        </div>
    )
};