import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';
import toastr from 'toastr';
import { resolveClientUrl, axiosError, toastOptions } from '../../../shared/common.functions';

export const Table = (props) => {
    const { data } = props;
    const [checkboxes, setCheckboxes] = useState([]);

    useEffect(() => {
        setCheckboxes(functionList(data.functions));
    }, [data.functions])

    const onSubmit = (e) => {
        e.preventDefault();
        axios.post(resolveClientUrl(`Function/AssignPermission`), {
            roleId: data.roleId,
            permissions: checkboxes
        }).then(response => {
            const { data } = response;
            if (data.isSuccessful) {
                toastr.success('Successfully!', '', toastOptions());
            } else {
                toastr.error(data.message, '', toastOptions());
            }
        }).catch(axiosError);
    }

    function getFunctionPermission(functionId) {
        return data.functionPermissions ? data.functionPermissions.find((item) => item.functionId === functionId) : null;
    }

    function createChildObject(item, functionPermission) {
        return {
            parentId: item.parentId,
            functionId: item.functionId,
            functionName: item.functionName,
            isRead: functionPermission ? functionPermission.isRead : false,
            isHost: functionPermission ? functionPermission.isHost : false,
            level: item.level,
            policy: item.policy
        };
    }

    function functionList(functions) {
        if (!functions) {
            return null;
        }

        const updatedList = functions.flatMap((item) => {
            const functionPermission = getFunctionPermission(item.functionId);

            const updatedChildList = item.childs.map((item2) => {
                const functionPermission2 = getFunctionPermission(item2.functionId);
                return createChildObject(item2, functionPermission2);
            });

            const updatedItem1 = {
                functionId: item.functionId,
                functionName: item.functionName,
                isRead: functionPermission ? functionPermission.isRead : false,
                isHost: functionPermission ? functionPermission.isHost : false,
                level: item.level,
                policy: item.policy,
                childIds: item.childs.length > 0 ? item.childs.map((itemChild) => {
                    return itemChild.functionId;
                }) : null
            };

            return [updatedItem1, ...updatedChildList];
        });

        return updatedList;
    }

    const handleCheckboxChange = (event, functionId, permission) => {
        const updatedCheckboxes = checkboxes.map((checkbox) => {
            if (checkbox.functionId === functionId) {
                if (permission === 'read') {
                    const updatedCheckbox = {
                        ...checkbox,
                        isRead: event.target.checked,
                        isHost: false
                    };
                    if (checkbox.childIds) {
                        updatedCheckbox.childIds.forEach((id) => {
                            const childCheckbox = checkboxes.find((child) => child.functionId === id);
                            if (childCheckbox) {
                                childCheckbox.isRead = event.target.checked;
                                childCheckbox.isHost = false;
                            }
                        });
                    }

                    if (checkbox.parentId) {
                        const childCheckbox = checkboxes.find((child) => child.functionId === checkbox.parentId);
                        if (!event.target.checked) {
                            let isFlagged = false;

                            childCheckbox.childIds.forEach((id) => {
                                if (id !== checkbox.functionId) {
                                    const childCheckbox2 = checkboxes.find((child) => child.functionId === id);
                                    if (childCheckbox2.isRead) {
                                        isFlagged = true;
                                        return;
                                    }
                                }
                            });
                            childCheckbox.isRead = isFlagged;
                            if (!isFlagged) {
                                childCheckbox.isHost = isFlagged;
                            }
                        }
                        else {
                            childCheckbox.isRead = true;
                        }
                    }

                    return updatedCheckbox;

                } else if (permission === 'host') {
                    const updatedCheckbox = {
                        ...checkbox,
                        isHost: event.target.checked,
                        isRead: event.target.checked ? true : checkbox.isRead
                    };
                    if (checkbox.childIds) {
                        updatedCheckbox.childIds.forEach((id) => {
                            const childCheckbox = checkboxes.find((child) => child.functionId === id);
                            if (childCheckbox) {
                                childCheckbox.isHost = event.target.checked;
                                childCheckbox.isRead = true;
                            }
                        });
                    }
                    if (checkbox.parentId) {
                        const childCheckbox = checkboxes.find((child) => child.functionId === checkbox.parentId);
                        if (!event.target.checked) {
                            let isFlagged = false;

                            childCheckbox.childIds.forEach((id) => {
                                if (id !== checkbox.functionId) {
                                    const childCheckbox2 = checkboxes.find((child) => child.functionId === id);
                                    if (childCheckbox2.isHost) {
                                        isFlagged = true;
                                        return;
                                    }
                                }
                            });
                            childCheckbox.isHost = isFlagged;
                        }
                        else {
                            childCheckbox.isHost = true;
                            childCheckbox.isRead = true;
                        }
                    }
                    return updatedCheckbox;
                }
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
                        {checkboxes ?
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