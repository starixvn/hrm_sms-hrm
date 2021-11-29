import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Button, Col, Form, Modal, Row } from 'react-bootstrap';
import { Formik } from 'formik';
import FormikErrorMessage from '@jsroot/components/FormikErrorMessage';
import ResponseMessage from '@jsroot/components/ResponseMessage';
import Select from 'react-select';
import LangHelper from '@jsroot/common/langHelper';
import { validateFormik } from '../common.functions';

export class Update extends Component {
    constructor(props) {
        super(props);
        this.state = {
            siteOptions: null,
            site: null,
            parentOptions: null,
            parent: null,
            functionOptions: [],
            func: null
        };
    }

    componentDidMount() {
        this.handleSiteOptions();
        this.props.getFunctionById(this.props.params).then(response =>{
            this.handleFunctionOptions(response.data);
        });
        this.handleParentOptions(this.props.menus);
    }

    handleSiteOptions = () =>{
        const {menuSelected, sites} = this.props;
        const siteOptions = sites.map(item =>{
            return {
                value: item.siteId,
                label: item.siteName
            };
        });
        const site = siteOptions.filter(x=>x.value === menuSelected.siteId)[0];
        this.setState({siteOptions, site});
    }

    handleFunctionOptions = functions => {
        const defaultFunction = [{
            functionId: null,
            functionName: 'None'
        }];
        const functionCopy = [...defaultFunction, ...functions];
        const functionOptions = functionCopy.map(item =>{
            return {
                value: item.functionId,
                label: item.functionName
            };
        });
        const funcFilter = functionOptions.filter(x=>x.value === this.props.menuSelected.functionId);
        const func = (this.props.menuSelected.functionId === null || funcFilter.length === 0) ? functionOptions[0] : funcFilter[0];
        this.setState({functionOptions, func});
    }

    handleParentOptions = parents =>{
        const defaultParent = [{
            menuItemId: null,
            menuItemName: 'None'
        }];
        const menuSelected = this.props.menuSelected;
        const parentCopy = [...defaultParent, ...parents];
        const parentOptions = parentCopy.map(item =>{
            return {
                value: item.menuItemId,
                label: item.menuItemName
            };
        });
        const parentFilter = parentOptions.filter(x=>x.value === menuSelected.parentId);
        const parent = (menuSelected.parentId === null || parentFilter.length === 0) ? parentOptions[0] : parentFilter[0];
        this.setState({parentOptions, parent});
    }


    handleCloseModalDialog = () => {
        this.props.setUpdateModalDialog({ show: false });
        if (this.props.updateMenuResult) {
            this.props.setUpdateMenuResult(null);
        }
    };

    handleChangeSite = site =>{
        this.setState({site});
        this.props.getParentById(site.value).then(response => { this.handleParentOptions(response.data); });
        this.props.getFunctionById(site.value).then(response =>{this.handleFunctionOptions(response.data);});
    }

    handleSubmit = (values, { setSubmitting }) => {
        setSubmitting(true);
        const {site, parent, func} = this.state;
        const params = {
            menuItemId: this.props.menuSelected.menuItemId,
            menuItemName: values.menuItemName,
            functionId: func.value,
            siteId: site.value,
            parentId: parent.value,
            url: values.url,
            displayOrder: values.order,
            description: values.description,
            isActive: values.isActive
        };

        this.props.updateMenu(params).then(() => {
            if (this.props.updateMenuResult.isSuccessful) {
                setTimeout(() => {
                    this.handleCloseModalDialog();
                    this.props.setSiteParams(params.siteId);
                    this.props.getMenuById(params.siteId);
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
        isSubmitting
    }) => {
        const {siteOptions, site, parent, parentOptions, functionOptions , func} = this.state;
        return (
            <Form noValidate onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormikErrorMessage touched={touched} errors={errors} fieldNames={Object.keys(values)} />
                    {
                        this.props.updateMenuResult &&
                        <ResponseMessage isSuccessful={this.props.updateMenuResult.isSuccessful}
                            message={this.props.updateMenuResult.message || LangHelper.getResource('UpdateMenuSuccessfully')}
                        />
                    }
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="menuItemName">
                            {LangHelper.getResource('MenuName')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.menuItemName}
                                name="menuItemName" id="menuItemName"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.menuItemName && errors.menuItemName}
                                onChange={handleChange}
                            />
                        </Col>
                    </Form.Group>
                    <Form.Group as={Row}>
                        <Form.Label column sm="4" md="3" className="required" htmlFor="site">
                            {LangHelper.getResource('Site')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className={`react-select-container ${errors.siteId && errors.siteId ? 'is-invalid' : ''}`}
                                classNamePrefix="react-select" isSearchable={false}
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
                        <Form.Label column sm="4" md="3" htmlFor="url">
                            {LangHelper.getResource('URL')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Control type="text" size="sm" value={values.url}
                                name="url" id="url"
                                className="shadow-none" autoComplete="off"
                                isInvalid={touched.url && errors.url}
                                onChange={handleChange}
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
                        <Form.Label column sm="4" md="3" htmlFor="function">
                            {LangHelper.getResource('Function')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Select className='react-select-container' classNamePrefix="react-select" isSearchable
                                options={functionOptions} value={func}
                                onChange={value => this.setState({func: value})}
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
                        <Form.Label column sm="4" md="3" htmlFor="isActive">
                            {LangHelper.getResource('Active')}
                        </Form.Label>
                        <Col sm="8" md="9">
                            <Form.Check type="checkbox"
                                checked={values.isActive}
                                name="isActive" id="isActive"
                                className='shadow-none'
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
        const { menuSelected } = this.props;
        const initialValues = {
            menuItemName: menuSelected.menuItemName,
            url: menuSelected.url,
            description: menuSelected.description,
            order: menuSelected.displayOrder,
            isActive: menuSelected.isActive
        };
        return (
            <>
                <Modal show
                    backdrop="static" keyboard={false} size="lg" centered
                    onHide={this.handleCloseModalDialog}
                >
                    <Modal.Header closeButton>
                        <Modal.Title as="div">{LangHelper.getResource('UpdateMenu')}</Modal.Title>
                    </Modal.Header>
                    <Formik
                        innerRef={el => this.formik = el}
                        initialValues={initialValues}
                        validate={values =>validateFormik(values, this.state.site.value)}
                        onSubmit={this.handleSubmit}
                    >
                        {this.renderForm}
                    </Formik>
                </Modal>
            </>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { params, sites, menus, functions, menuSelected, updateMenuResult } = state;
    return { params, sites, menus, functions, menuSelected, updateMenuResult };
};

export default connect(mapStateToProps, mapDispatchToProps)(Update);