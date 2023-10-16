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

    const renderTableRows = (items) => {
        return items.map((item, i) => (
            <Fragment key={i}>
                <tr id={`row_${i}`} className={item.level === 0 ? 'root-row' : 'child-row'}>
                    <td className="align-left">
                        {item.level > 0 && (
                            <Fragment>
                                {Array.from({ length: item.level.length - 1 }).map((_, i) => (
                                    <span key={i} style={{ opacity: 0 }} className="icon-child-level" >&#9495;</span>
                                ))}
                                <span className="icon-child-level">&#9495;</span>
                            </Fragment>
                        )}
                        {item.functionName}
                    </td>
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
                {item.childs && renderTableRows(item.childs)}
            </Fragment>
        ));
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
                        renderTableRows(data.functions)
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