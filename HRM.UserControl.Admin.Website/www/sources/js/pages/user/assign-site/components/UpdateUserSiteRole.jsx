import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import { ActionCreators } from '../../actions';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import LangHelper from '@jsroot/common/langHelper';

export class UpdateUserSiteRole extends Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {
        this.props.setSiteRoles([]);
        const { site } = this.props.updateUserSiteRoleModalDialog;
        this.props.getRoleBySiteId(site.siteId);
    }

    handleCloseModalDialog = () => {
        this.props.setUpdateUserSiteRoleModalDialog({ show: false });
        if (this.props.setUpdateUserSiteRoleResult) {
            this.props.setUpdateUserSiteRoleResult(null);
        }
    };

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const params = {
            siteId: values.siteId,
            userId: window.selectedUserId,
            roleIds: values.roles.filter(item => item.checked).map(item => item.roleId).join(',')
        };
        this.props.updateUserSiteRole(params).then(() => {
            if (this.props.updateUserSiteRoleResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.getSiteByUserId(window.selectedUserId);
                }, 3000);
            } else {
                setSubmitting(false);
            }
        });
    };

    handleChangeRole = (roleId, event) => {
        const roles = [...this.formik.values.roles];
        const index = roles.findIndex(x => x.roleId === roleId);
        roles[index].checked = event.target.checked;
        this.formik.setFieldValue('roles', roles);
    };

    renderForm = ({
        values,
        handleSubmit,
        isSubmitting
    }) => {
        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    {
                        this.props.updateUserSiteRoleResult &&
                        <ResponseMessage isSuccessful={this.props.updateUserSiteRoleResult.isSuccessful}
                            message={this.props.updateUserSiteRoleResult.message || LangHelper.getResource('AssignRoleSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3">
                            {LangHelper.getResource('Site')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.siteName}
                                className="shadow-none" disabled
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3">{LangHelper.getResource('Role')}</Form.Label>
                        <Col sm="8" md="9">
                            {
                                values.roles.map(item => {
                                    return (
                                        <Form.Check key={item.roleId} type="checkbox"
                                            checked={item.checked} label={item.name}
                                            disabled={item.isDefault}
                                            onChange={event => this.handleChangeRole(item.roleId, event)}
                                        />
                                    );
                                })
                            }
                            
                        </Col>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button type="submit" variant="primary" size="sm" disabled={isSubmitting}>
                        {LangHelper.getResource('Save')}
                    </Button>
                    <Button variant="secondary" size="sm" onClick={this.handleCloseModalDialog}>
                        {LangHelper.getResource('Cancel')}
                    </Button>
                </Modal.Footer>
            </Form>
        );
    };

    render() {
        const { updateUserSiteRoleModalDialog, siteRoles } = this.props;
        if (siteRoles.length === 0) {
            return null;
        }

        const initialValues = {
            siteId: updateUserSiteRoleModalDialog.site.siteId,
            siteName: updateUserSiteRoleModalDialog.site.siteName,
            roles: []
        };
        siteRoles.forEach(item => {
            const role = updateUserSiteRoleModalDialog.site.roles.find(x => x.roleId == item.roleId);
            initialValues.roles.push({ ...item, checked: item.isDefault || (role !== undefined && role !== null) });
        });

        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.handleCloseModalDialog}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('AssignRole')}</Modal.Title>
                </Modal.Header>
                <Formik
                    innerRef={el => this.formik = el}
                    initialValues={initialValues}
                    onSubmit={this.handleSubmit}
                >
                    {this.renderForm}
                </Formik>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { updateUserSiteRoleModalDialog, siteRoles, updateUserSiteRoleResult } = state;
    return { updateUserSiteRoleModalDialog, siteRoles, updateUserSiteRoleResult };
};

export default connect(mapStateToProps, mapDispatchToProps)(UpdateUserSiteRole);