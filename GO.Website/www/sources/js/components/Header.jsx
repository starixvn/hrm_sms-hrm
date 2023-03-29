import React, { Component } from 'react';
import { Dropdown } from 'react-bootstrap';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

export default class Header extends Component {
    constructor(props) {
        super(props);

        this.state = {
            user: null,
            sites: []
        };
    }

    async componentDidMount() {
        let url = Site.resolveClientUrl('home/getUserInfo');
        await DataService.postDataAsync(url).then(response => {
            this.setState({ user: response.data });
        });

        url = Site.resolveClientUrl('home/getSites');
        await DataService.getDataAsync(url).then(response => {
            this.setState({ sites: response.data });
        });
    }

    onClick = async (event, menuUrl) => {
        event.preventDefault();

        let url = Site.resolveClientUrl('sso/gettime');
        await DataService.getDataAsync(url).then(response => {
            url = `${menuUrl}&time=${response.data}`;
            window.open(url, '_blank', 'noopener,noreferrer');
        });
    };

    render() {
        const { user, sites } = this.state;
        let nickname = '';
        if (user) {
            nickname = user.nickname ? user.nickname : user.username;
        }

        return (
            <>
                <div className="navbar-fixed">
                    <nav className="navbar">
                        <div className="pull-left left-bar">
                            <a className="navbar-brand" href={Site.resolveClientUrl('home/index')}>
                                <img src={Site.resolveClientUrl('css/images/logo.svg')} alt="GO System" />
                            </a>
                        </div>
                        <div className="user-info pull-right">
                            <Dropdown>
                                <Dropdown.Toggle id="dropdown-user">
                                    {nickname}
                                    <span className="user-avatar">
                                        <i className="fa fa-user" style={{ marginTop: 8 }} />
                                    </span>
                                </Dropdown.Toggle>
                                <Dropdown.Menu>
                                    <Dropdown.Item href={Site.resolveClientUrl('auth/logout')}><i className="fa fa-power-off" /> Sign Out</Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>
                        </div>
                    </nav>
                </div>
                <ul className="side-nav fixed">
                    {
                        sites.map(site => <li key={site.siteId}>
                            <a onClick={event => this.onClick(event, site.siteURL)}>
                                {site.siteName}
                            </a>
                        </li>)
                    }
                </ul>
            </>
        );
    }
}