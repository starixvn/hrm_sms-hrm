import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';
import { ActionCreators } from '../../actions';
import LangHelper from '@jsroot/common/langHelper';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            site: null
        };
    }

    componentDidUpdate(prevProps) {
        if (prevProps.userSites !== this.props.userSites && this.props.userSites.length > 0) {
            const site = this.props.userSites[0];
            this.setState({ site: { value: site.siteId, label: site.siteName } }, () => {
                const params = this.getParams();
                this.props.getUserPermission(params);
            });
        }
    }

    getParams = () => {
        return {
            userId: window.selectedUserId,
            siteId: this.state.site.value
        };
    };

    handleSubmit = event => {
        event.preventDefault();
        const params = this.getParams();
        this.props.getUserPermission(params);
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    render() {
        const { site } = this.state;
        const siteOptions = this.props.userSites.map(item => { return { value: item.siteId, label: item.siteName }; });

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
                        <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={this.handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { userSites } = state;
    return { userSites };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);