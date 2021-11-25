import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import Select from 'react-select';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import LangHelper from '@jsroot/common/langHelper';
import { PolicyOptions } from '@jsroot/common/constants';
import { validateFormik } from '../common.functions';

export class Create extends Component {
    constructor(props) {
        super(props);
        this.state = {
            siteOptions: null,
            site: null,
            parentOptions: null,
            parent: null
        };
    }

    componentDidMount() {
        this.handleSiteOptions();
        this.handleParentOptions(this.props.functions);
    }

    handleSiteOptions = () =>{
        const {params, sites} = this.props;
        const siteOptions = sites.map(item =>{
            return {
                value: item.siteId,
                label: item.siteName
            };
        });
        const site = siteOptions.filter(x=>x.value === params)[0];
        this.setState({siteOptions, site});
    }

    handleParentOptions = parents =>{
        const defaultParent = [{
            functionId: null,
            functionName: 'None'
        }];
        const parentCopy = [...defaultParent, ...parents];
        const parentOptions = parentCopy.map(item =>{
            return {
                value: item.functionId,
                label: item.functionName
            };
        });
        const parent = parentOptions[0];
        this.setState({parentOptions, parent});
    }

    handleCloseModalDialog = () => {
        this.props.setCreateModalDialog({ show: false });
        if (this.props.createFunctionResult) {
            this.props.setCreateFunctionResult(null);
        }
    };

    handleChangeSite = site =>{
        this.setState({site});
        this.props.getParentById(site.value).then(data => { this.handleParentOptions(data.data); });
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const {site, parent} = this.state;
        const params = {
            functionName: values.functionName,
            siteId: site.value,
            parentId: parent.value,
            policy: values.policy.value,
            displayOrder: values.order,
            description: values.description,
            isActive: values.isActive
        };

        this.props.setGetSiteParams(params.siteId);
        this.props.createFunction(params).then(() => {
            if (this.props.createFunctionResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.getFunctionById(params.siteId);
                }, 3000);
            } else {
                setSubmitting(false);
            }
        });
    };

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        const {siteOptions, site, parent, parentOptions} = this.state;
        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.createFunctionResult &&
                        <ResponseMessage isSuccessful={this.props.createFunctionResult.isSuccessful}
                            message={this.props.createFunctionResult.message || LangHelper.getResource('CreateFunctionSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="functionName">
                            {LangHelper.getResource('FunctionName')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.functionName}
                                name="functionName" id="functionName"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.functionName && errors.functionName}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="site">
                            {LangHelper.getResource('Site')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className='react-select-container' classNamePrefix="react-select" isSearchable={false}
                                options={siteOptions} value={site}
                                onChange={value => this.handleChangeSite(value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="description">
                            {LangHelper.getResource('Description')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.description}
                                name="description" id="description"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.description && errors.description}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="policy">
                            {LangHelper.getResource('Policy')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className='react-select-container' classNamePrefix="react-select" isSearchable={false}
                                options={PolicyOptions} value={values.policy}
                                onChange={value => setFieldValue('policy', value)}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="parent">
                            {LangHelper.getResource('Parent')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className='react-select-container' classNamePrefix="react-select" isSearchable
                                options={parentOptions} value={parent}
                                onChange={value => this.setState({parent: value})}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" htmlFor="order">
                            {LangHelper.getResource('Order')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="number" size="sm" value={values.order}
                                name="order" id="order"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.order && errors.order}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3"
                            htmlFor="isActive"
                        >
                            {LangHelper.getResource('Status')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Check type="checkbox"
                                checked={values.isActive}
                                name="isActive" id="isActive"
                                className="shadow-none"
                                onChange={handleChange}
                            />
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
        const initialValues = {
            functionName: '',
            description: '',
            policy: PolicyOptions[1],
            order: 0,
            isActive: true
        };

        return (
            <Modal show
                backdrop="static" keyboard={false} size="lg" centered
                onHide={this.handleCloseModalDialog}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{LangHelper.getResource('CreateFunction')}</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={initialValues}
                    validate={validateFormik}
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
    const { sites, functions, params, createFunctionResult } = state;
    return { sites, functions, params, createFunctionResult };
};

export default connect(mapStateToProps, mapDispatchToProps)(Create);