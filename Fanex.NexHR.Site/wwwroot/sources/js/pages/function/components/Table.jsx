import React, { Fragment } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError, confirm, alert } from '../../../shared/common.functions';

export const Table = (props) => {
    const { data } = props;
    console.log(data)

    const checkActive = (id, permission) => {
        for (const element of data.functionPermissions) {
            if (element.functionId === id) {
                return permission == 'read' ? element.isRead : element.isHost;
            }
        }
        return false;
    };

    const handleCheckboxChange = () => {
    };

    return (
        <Fragment>
            <table className="table table-custom">
                <thead>
                    <tr>
                        <th rowSpan="2">Function</th>
                        <th colSpan="2">Role Permission</th>
                    </tr>
                    <tr>
                        <th className="th-read">Read</th>
                        <th>Host</th>
                    </tr>
                </thead>
                <tbody>
                    {data.functions ? (
                        data.functions.map((item, i) => (
                            <React.Fragment key={i}>
                                <tr id={`row_${i}`} className={item.level === 0 ? 'root-row' : 'child-row'}>
                                    <td className="align-left">{item.functionName}</td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={checkActive(item.functionId, 'read')}
                                            onChange={handleCheckboxChange}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={checkActive(item.functionId, 'host')}
                                            onChange={handleCheckboxChange}
                                        />
                                    </td>
                                </tr>
                                {item.childs ? (
                                    item.childs.map((item, i) => (
                                        <React.Fragment key={i}>
                                            <tr id={`row_${i}`} className={item.level === 0 ? 'root-row' : 'child-row'}>
                                                <td className="align-left">
                                                    <span class="icon-child-level">&#9495;</span>
                                                    {item.functionName}</td>
                                                <td>
                                                    <input
                                                        type="checkbox"
                                                        checked={checkActive(item.functionId, 'read')}
                                                        onChange={handleCheckboxChange}
                                                    />
                                                </td>
                                                <td>
                                                    <input
                                                        type="checkbox"
                                                        checked={checkActive(item.functionId, 'host')}
                                                        onChange={handleCheckboxChange}
                                                    />
                                                </td>
                                            </tr>
                                        </React.Fragment>
                                    ))
                                ) : ''}
                            </React.Fragment>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="3">No information available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </Fragment>
    )
}