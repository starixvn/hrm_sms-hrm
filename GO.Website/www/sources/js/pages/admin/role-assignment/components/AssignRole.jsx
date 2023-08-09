import React, { useState } from 'react';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { toast } from 'react-toastify';

import { ActionCreators } from '../actions';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

const AssignRole = ({ userRoleFilterParams, updateUserRoleModalDialog, roles, getUserRoles, setUpdateUserRoleModalDialog, assignRole }) => {
    const tmpRoles = roles.map(role => {
        return {
            ...role,
            checked: updateUserRoleModalDialog.user.roles.findIndex(item => item.roleId === role.roleId) >= 0
        };
    });

    const [userRoles, setUserRoles] = useState(tmpRoles);

    const handleClose = () => {
        setUpdateUserRoleModalDialog({
            show: false
        });
    };

    const handleChangeRole = (event, index) => {
        const newArr = [...userRoles];
        newArr[index].checked = event.target.checked;
        setUserRoles(newArr);
    };

    const handleSubmit = () => {
        Site.blockUI();
        const params = {
            userId: updateUserRoleModalDialog.user.userId,
            roleIds: userRoles.filter(item => item.checked).map(item => item.roleId).join(',')
        };
        assignRole(params).then(response => {
            if (response.data.isSuccessful) {
                toast.success(LangHelper.getResource('UpdateSuccessfully'));
                handleClose();
                getUserRoles(userRoleFilterParams);
            } else {
                toast.error(response.data.message);
            }

            Site.unblockUI();
        });
    };

    return (
        <Modal show backdrop="static" keyboard={false} size="sm" centered onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title as="div">{`Update ${updateUserRoleModalDialog.user.nickname}'s Role`}</Modal.Title>
            </Modal.Header>
            <Form id="assign-user-role-form" className="assign-user-role-form" noValidate>
                <Modal.Body>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3">{LangHelper.getResource('Role')}</Form.Label>
                        <Col sm="8" md="9">
                            {
                                userRoles.map((role, index) =>
                                    <Form.Check key={role.roleId} type="checkbox"
                                        checked={role.checked} label={role.name}
                                        onChange={event => handleChangeRole(event, index)}
                                    />
                                )
                            }
                        </Col>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="primary" size="sm" onClick={handleSubmit}>{LangHelper.getResource('Submit')}</Button>
                    <Button variant="secondary" size="sm" onClick={handleClose}>{LangHelper.getResource('Cancel')}</Button>
                </Modal.Footer>
            </Form>
        </Modal>
    );
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { userRoleFilterParams, roles, updateUserRoleModalDialog } = state;
    return { userRoleFilterParams, roles, updateUserRoleModalDialog };
};

export default connect(mapStateToProps, mapDispatchToProps)(AssignRole);