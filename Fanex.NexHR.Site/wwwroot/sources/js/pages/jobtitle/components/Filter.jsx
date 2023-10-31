import React, { Fragment } from 'react';
import { resolveClientUrl } from '../../../shared/common.functions';

export const Filter = (props) => {
    const { keyword, setKeyword, searchKeyword, setCreateModal } = props;

    const handleKeyPress = (event) => {
        if (event.key === 'Enter') {
            searchKeyword();
        }
    }

    const handleExportExcel = () => {
        window.location.href = resolveClientUrl('jobTitle/ExportExcel?jobTitle=' + keyword);
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
                            onChange={e => setKeyword(e.target.value)}
                            onKeyPress={handleKeyPress} />
                    </div>
                </div>
                <div className="form-group form-group--m">
                    <button type="submit" className="btn btn-primary" onClick={searchKeyword}>Search</button>
                    {
                        window.hasPermission === 'True' &&
                        <Fragment>
                            <button type="submit" className="btn btn-excel" onClick={handleExportExcel}>Excel</button>
                            <button className="btn btn-request" type="button" onClick={() => setCreateModal(true)}>
                                <span className="fa fa-plus" /> &nbsp;New Job Title
                            </button>
                        </Fragment>
                    }
                </div>
            </div>
        </div>
    )
};