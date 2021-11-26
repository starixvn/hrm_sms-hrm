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
            this.handleUpdateSiteOption();
        }
    }

    handleUpdateSiteOption = () =>{
        const siteFilter = this.props.sites.filter(x=>x.siteId === this.props.params).map(item =>{
            return {
                value: item.siteId,
                label: item.siteName
            };
        });

        this.setState({site: siteFilter[0]});
    }

    handleSubmit = event => {
        event.preventDefault();
        const siteId = this.state.site.value;
        this.props.setSiteParams(siteId);
        this.props.getMenuById(siteId);
    };

    handleSiteOptions = () =>{
        const siteOptions = this.props.sites.map(item =>{
            return {
                value: item.siteId,
                label: item.siteName
            };
        });
        const site = siteOptions[0];
        this.setState({siteOptions, site});
        this.props.setSiteParams(site.value);
        this.props.getMenuById(site.value);
    }

    handleOpenCreateModal = () => {
        this.props.setCreateModalDialog({ show: true });
    };

    handleChange = value => {
        this.setState({ site: value });
    };

    render() {
        return (
            <Form className="form-filter" onSubmit={this.handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Site')}</Form.Label>
                        <Select classNamePrefix="react-select" className='react-select-container'  isSearchable={false}
                            value={this.state.site} options={this.state.siteOptions} 
                            onChange={value => this.handleChange(value)}

                        />
                    </Form.Group>
                    <Form.Group>
                        <Button className="mr-3" type="submit" variant="primary" size="sm"  onClick={this.handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button>
                        <Button size="sm" variant="primary" onClick={this.handleOpenCreateModal}>
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