import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import { resolveClientUrl, axiosError } from '../../../shared/common.functions';

export const Table = (props) => {
    const { data } = props;
    //  console.log(data)

    const [checkboxes, setCheckboxes] = useState([]);

    const onSubmit = (e) => {
        axios.post(resolveClientUrl(`Function/AssignPermission`), {
            roleId: roleId
        }).then(response => {
            const { data } = response;
            if (data.IsSuccessful) {
                toastr.success('Successfully!', '', toastOptions());
            } else {
                toastr.error(data.Message, '', toastOptions());
            }
        }).catch(axiosError);
    }

    useEffect(() => {
        setCheckboxes(functionList(data.functions));
    }, [data.functions])

    function functionList(functions) {
        const updatedList = functions ? functions.map((item1) => {
            const functionPermission = data.functionPermissions ? data.functionPermissions.find((item2) => item2.functionId === item1.functionId) : null;
            return {
                functionId: item1.functionId,
                functionName: item1.functionName,
                isRead: functionPermission ? functionPermission.isRead : false,
                isHost: functionPermission ? functionPermission.isHost : false,
                level: item1.level,
                policy: item1.policy,
                childs: item1.childs.length > 0 ? [functionList(item1.childs)] : []
            };
        }) : null;

        return updatedList;
    }

    const handleCheckboxChange = (event, functionId, permission) => {
        const updatedCheckboxes = checkboxes.map((checkbox) => {
            if (checkbox.functionId === functionId) {
                if (permission === 'read') {
                    const updatedCheckbox = { ...checkbox, isRead: event.target.checked };
                    return updatedCheckbox;
                } else if (permission === 'host') {
                    return { ...checkbox, isHost: event.target.checked };
                }
            }

            if (checkbox.childs.length > 0) {
                checkboxes.childs = checkbox.childs.map((checkboxChild) => ({
                    ...checkboxChild,
                    isRead: event.target.checked
                }));
            }
            return checkbox;
        });

        setCheckboxes(updatedCheckboxes);
    };

    const renderTableRows = (items) => {
        const disabled = data.isHost ? '' : "disabled";

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
                        {(!item.policy || item.policy == window.read) &&
                            <input
                                type="checkbox"
                                checked={item.isRead}
                                onChange={(event) => handleCheckboxChange(event, item.functionId, 'read')}
                                disabled={disabled}
                            />
                        }
                    </td>
                    <td>
                        {(!item.policy || item.policy == window.host) &&
                            <input
                                type="checkbox"
                                checked={item.isHost}
                                onChange={(event) => handleCheckboxChange(event, item.functionId, 'host')}
                                disabled={disabled}
                            />
                        }
                    </td>
                </tr>
                {item.childs && item.childs.length > 0 && renderTableRows(item.childs[item.childs.length - 1])}
            </Fragment>
        ));
    };

    return (
        <Fragment>
            <form onSubmit={onSubmit}>
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
                        {checkboxes.length > 0 ?
                            renderTableRows(checkboxes)
                            : (
                                <tr>
                                    <td colSpan="3">No information available</td>
                                </tr>
                            )}
                    </tbody>
                </table>
                {
                    data.isHost &&
                    <div className="form-group">
                        <button type="submit" className="btn btn-primary">Save</button>
                    </div>
                }
            </form>
        </Fragment>
    )
}