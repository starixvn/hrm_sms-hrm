import React, { Fragment, useEffect, useState } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError } from '../../../../shared/common.functions';
import { Table } from './Table';
export const AppContainer = () => {
    const [data, setData] = useState([]);

    const handleGetAllEnternalGroup = () => {
        axios(resolveClientUrl('ExtensionNo/GetAllExternalGroup')).then(response => {
            const { data } = response;
            if (data) {
                setData(response.data)
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetAllEnternalGroup();
    }, [])

    return (
        <Fragment>
            <a className="btn btn-request">
                <span className="fa fa-plus"></span> New External Group
            </a>
            <div id="dialog-form">
                <form className="form-horizontal">
                    <div className="form-group">
                        <div className="label-form">Group Name</div>
                        <div className="input-form">
                            <input type="text" name="TeamName" className="form-control" autoComplete="off" />
                        </div>
                    </div>
                </form>
            </div>
            <Table data={data}/>
        </Fragment>
    )
};