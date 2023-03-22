import React from 'react';
import { Col, Form, Row } from 'react-bootstrap';
import LangHelper from '@jsroot/common/langHelper';

const UserInfo = () => {
    const selectedUser = window.selectedUser;
    return (
        <Form className="user-info mb-3 p-3 border">
            <Form.Group as={Row} className="mb-1">
                <Col sm="6">
                    <Form.Label column sm="4">
                        {LangHelper.getResource('Username')}:
                    </Form.Label>
                    <Form.Label column sm="7" className="font-weight-bold">{selectedUser.username}</Form.Label>
                </Col>
                <Col sm="6">
                    <Form.Label column sm="4">
                        {LangHelper.getResource('Code')}:
                    </Form.Label>
                    <Form.Label column sm="7" className="font-weight-bold">{selectedUser.code}</Form.Label>
                </Col>
            </Form.Group>
            <Form.Group as={Row} className="mb-0">
                <Col sm="6">
                    <Form.Label column sm="4">
                        {LangHelper.getResource('Nickname')}:
                    </Form.Label>
                    <Form.Label column sm="7" className="font-weight-bold">{selectedUser.nickname}</Form.Label>
                </Col>
                <Col sm="6">
                    <Form.Label column sm="4">
                        {LangHelper.getResource('FullName')}:
                    </Form.Label>
                    <Form.Label column sm="7" className="font-weight-bold">{selectedUser.fullName}</Form.Label>
                </Col>
            </Form.Group>
        </Form>
    );
};

export default UserInfo;