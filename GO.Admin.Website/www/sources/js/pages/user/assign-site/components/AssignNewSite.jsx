import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import Select from 'react-select';
import { ActionCreators } from '../../actions';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import LangHelper from '@jsroot/common/langHelper';

export class AssignNewSite extends Component {
    constructor(props) {
        super(props);

        this.state = { selectedSite: null };
    }

    componentDidMount() {
        this.props.getAvailableSiteByUserId(window.selectedUser.userId);
    }

    handleCloseModalDialog = () => {
        this.props.setAssignNewSiteModalDialog({ show: false });
        if (this.props.assignNewSiteResult) {
            this.props.setAssignNewSiteResult(null);
        }
    };

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const params = {
            siteId: values.site.value,
            userId: window.selectedUser.userId,
            roleIds: values.roles.filter(item => item.checked).map(item => item.roleId).join(',')
        };
        this.props.assignSiteToUser(params).then(() => {
            if (this.props.assignNewSiteResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.getSiteByUserId(window.selectedUser.userId);
                }, 3000);
            } else {
                setSubmitting(false);
            }
        });
    };

    handleChangeSite = site => {
        this.props.getRoleBySiteId(site.value);
        this.setState({ selectedSite: site });
    };

    handleChangeRole = (roleId, event) => {
        const roles = [...this.formik.values.roles];
        const index = roles.findIndex(x => x.roleId === roleId);
        roles[index].checked = event.target.checked;
        this.formik.setFieldValue('roles', roles);
    };

    validateFormik = values => {
        const errors = {};

        if (values.site === null) {
            errors.site = LangHelper.formatString(
                LangHelper.getResource('FieldRequired'),
                LangHelper.getResource('Site'));
        }

        return errors;
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleSubmit,
        isSubmitting
    }) => {
        const siteOptions = this.props.availableSites.map(item => { return { value: item.siteId, label: item.siteName }; });

        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.assignNewSiteResult &&
                        <ResponseMessage isSuccessful={this.props.assignNewSiteResult.isSuccessful}
                            message={this.props.assignNewSiteResult.message || LangHelper.getResource('AssignSiteSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required">
                            {LangHelper.getResource('Site')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className={`react-select-container ${errors && errors.site ? 'is-invalid' : ''}`}
                                classNamePrefix="react-select" isSearchable={false}
                                options={siteOptions} value={values.site} placeholder={LangHelper.getResource('PleaseSelect')}
                                onChange={value => this.handleChangeSite(value)}
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
        const { siteRoles } = this.props;
        const initialValues = {
            site: this.state.selectedSite,
            roles: []
        };
        siteRoles.forEach(item => {
            initialValues.roles.push({ ...item, checked: item.isDefault });
        });

        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.handleCloseModalDialog}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('AssignNewSite')}</Modal.Title>
                </Modal.Header>
                <Formik enableReinitialize
                    innerRef={el => this.formik = el}
                    validate={this.validateFormik}
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
    const { siteRoles, availableSites, assignNewSiteModalDialog, assignNewSiteResult } = state;
    return { siteRoles, availableSites, assignNewSiteModalDialog, assignNewSiteResult };
};

export default connect(mapStateToProps, mapDispatchToProps)(AssignNewSite);