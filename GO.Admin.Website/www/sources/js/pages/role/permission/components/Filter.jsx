import React, { Component } from 'react';
import { Form } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';
import { ActionCreators } from '../../actions';
import LangHelper from '@jsroot/common/langHelper';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            site: null,
            role: null
        };
    }

    componentDidUpdate(prevProps) {
        const { sites, roles } = this.props;
        if (prevProps.sites !== sites && this.props.sites.length > 0) {
            const site = window.selectedSiteId
                ? sites.find(item => item.siteId === window.selectedSiteId) : sites[0];
            this.setState({ site: { value: site.siteId, label: site.siteName } }, () => {
                this.getRoleBySiteId(site.siteId);
            });
        }

        if (prevProps.roles !== roles && sites.length > 0) {
            const role = roles.length ? { value: roles[0].roleId, label: roles[0].name } : null;
            this.setState({ role }, () => {
                const params = this.getParams();
                this.props.setGetRolePermissionParams(params);
                this.props.getRolePermission(params);
            });
        }
    }

    getParams = () => {
        const { site, role } = this.state;
        return {
            siteId: site ? site.value : 0,
            roleId: role ? role.value : 0
        };
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value }, () => {
            if (name === 'site') {
                this.setState({
                    role: null
                }, () => {
                    this.getRoleBySiteId(value.value);
                });
            } else {
                const params = this.getParams();
                this.props.setGetRolePermissionParams(params);
                this.props.getRolePermission(params);
            }
        });
    };

    getRoleBySiteId = siteId => {
        const params = { siteId, isActive: true };
        this.props.getRoleBySiteId(params);
    };

    render() {
        const { site, role } = this.state;
        const siteOptions = this.props.sites.map(item => { return { value: item.siteId, label: item.siteName }; });
        const roleOptions = this.props.roles.map(item => { return { value: item.roleId, label: item.name }; });

        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Site')}</Form.Label>
                        <Select className={`react-select-container`} classNamePrefix="react-select"
                            options={siteOptions} value={site}
                            onChange={value => this.handleChange('site', value)}
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Role')}</Form.Label>
                        <Select className={`react-select-container`} classNamePrefix="react-select"
                            options={roleOptions} value={role}
                            onChange={value => this.handleChange('role', value)}
                        />
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { sites, roles } = state;
    return { sites, roles };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);