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
            siteOptions: null,
            site: null
        };
    }
    
    componentDidMount() {
        this.handleSiteOptions();
    }

    componentDidUpdate(prevProps){
        if ( this.props.params !== prevProps.params ) {
            const siteFilter = this.props.sites.filter(x=>x.siteId === this.props.params).map(item =>{
                return {
                    value: item.siteId,
                    label: item.siteName
                };
            });

            this.setState({site: siteFilter[0]});
        }
    }

    handleSiteOptions = () =>{
        const siteOptions = this.props.sites.map(item =>{
            return {
                value: item.siteId,
                label: item.siteName
            };
        });
        const site = siteOptions[0];
        this.setState({siteOptions, site});
        this.props.setGetSiteParams(site.value);
        this.props.getFunctionById(site.value);
    }

    handleSubmit = event => {
        event.preventDefault();
        this.props.setGetSiteParams(this.state.site.value);
        this.props.getFunctionById(this.state.site.value);
    };

    handleOpenCreateModalDialog = () => {
        this.props.setCreateModalDialog({ show: true });
    };

    handleChange = (name, value) => {
        this.setState({ [name]: value });
    };

    render() {
        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Site')}</Form.Label>
                        <Select className='react-select-container' classNamePrefix="react-select" isSearchable={false}
                            options={this.state.siteOptions} value={this.state.site}
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