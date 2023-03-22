import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';
import { ActionCreators } from '../actions';
import { ActiveStatusOptions } from '@jsroot/common/constants';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';

export class Filter extends Component {
    constructor(props) {
        super(props);
        this.state = {
            site: null,
            status: ActiveStatusOptions[0]
        };
    }

    componentDidUpdate(prevProps) {
        if (prevProps.sites !== this.props.sites && this.props.sites.length > 0) {
            const site = this.props.sites[0];
            this.setState({ site: { value: site.siteId, label: site.siteName } }, () => {
                const params = this.getParams();
                this.props.setGetRoleParams(params);
                this.props.getRoleBySiteId(params);
            });
        }
    }

    getParams = () => {
        const { site, status } = this.state;
        return {
            siteId: site ? site.value : 0,
            isActive: status.value
        };
    };

    handleSubmit = event => {
        event.preventDefault();
        const params = this.getParams();
        this.props.setGetRoleParams(params);
        this.props.getRoleBySiteId(params);
    };

    handleOpenCreateModalDialog = () => {
        this.props.setCreateModalDialog({ show: true });
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    render() {
        const { getRoleParams, sites } = this.props;
        const siteOptions = sites.map(item => {
            return { value: item.siteId, label: item.siteName };
        });

        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Site')}</Form.Label>
                        <Select className='react-select-container' classNamePrefix="react-select" isSearchable={false}
                            options={siteOptions} value={this.state.site}
                            onChange={value => this.handleChange('site', value)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Status')}</Form.Label>
                        <Select className='react-select-container' classNamePrefix="react-select" isSearchable={false}
                            options={ActiveStatusOptions} value={this.state.status}
                            onChange={value => this.handleChange('status', value)}
                        />
                    </Form.Group>
                    <Form.Group className="group-button">
                        <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={this.handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                        {
                            window.hasHostPermissionOnRole &&
                            <Button variant="primary" size="sm" className="mr-3" onClick={this.handleOpenCreateModalDialog}>
                                {LangHelper.getResource('Create')}
                            </Button>
                        }
                        <Button variant="primary" size="sm"
                            onClick={() => window.location = Site.resolveClientUrl(`role/permission?siteId=${getRoleParams.siteId}`)}
                        >
                            {LangHelper.getResource('RolePermission')}
                        </Button>
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { getRoleParams, sites } = state;
    return { getRoleParams, sites };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);