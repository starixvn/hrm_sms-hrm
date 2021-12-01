import React, { Component } from 'react';
import { Button, Form } from 'react-bootstrap';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import Select from 'react-select';
import { ActionCreators } from '../actions';
import LangHelper from '@jsroot/common/langHelper';

export class Filter extends Component {
    constructor(props) {
        super(props);
        this.state = {
            site: null
        };
    }
    
    componentDidMount() {
        this.handleSiteSelected();
    }

    componentDidUpdate(prevProps){
        if ( this.props.params !== prevProps.params ) {
            this.handleChangeParam();
        }
    }

    handleChangeParam = () => {
        const siteFilter = this.props.sites.filter(x=>x.siteId === this.props.params)[0];
        const site = {
            value: siteFilter.siteId,
            label: siteFilter.siteName
        };
        this.setState({site});
    }

    handleSiteSelected = () =>{
        const siteFirst = this.props.sites[0];
        const site = {
            value: siteFirst.siteId,
            label: siteFirst.siteName
        };
        this.setState({site});
        this.props.setSiteParams(site.value);
        this.props.getFunctionById(site.value);
    }

    handleSubmit = event => {
        event.preventDefault();
        const value = this.state.site.value;
        this.props.setSiteParams(value);
        this.props.getFunctionById(value);
    };

    handleOpenCreateModalDialog = () => {
        this.props.setCreateModalDialog({ show: true });
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    render() {
        const siteOptions = this.props.sites.map(item =>{
            return {
                value: item.siteId,
                label: item.siteName
            };
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
                        <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={this.handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                        <Button variant="primary" size="sm" onClick={this.handleOpenCreateModalDialog}>
                            {LangHelper.getResource('Create')}
                        </Button>
                    </Form.Group>
                </Form.Row>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { sites, params } = state;
    return { sites, params };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);