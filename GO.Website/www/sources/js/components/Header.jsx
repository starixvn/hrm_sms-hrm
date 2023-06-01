import React, { Component } from 'react';
import { Dropdown } from 'react-bootstrap';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

export default class Header extends Component {
    constructor(props) {
        super(props);

        this.state = {
            user: null,
            sites: [],
            menus: [],
            expended: false
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

        url = Site.resolveClientUrl('home/getMenus');
        await DataService.getDataAsync(url).then(response => {
            this.setState({ menus: response.data });
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

    handleToggle = () => {
        this.setState(prevState => {
            return { expended: !prevState.expended };
        });
    };

    isActive = (menuItem, currentUrl) => (menuItem.url !== null && menuItem.url.toLowerCase().localeCompare(currentUrl) === 0)
        || menuItem.childs.filter(child => child.url.toLowerCase().localeCompare(currentUrl) === 0).length > 0;

    render() {
        const { user, sites, menus, expended } = this.state;
        const currentUrl = window.location.pathname.toLowerCase();

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
                        <div className={`pull-left nav-hamberger ${expended ? 'expended' : ''}`}>
                            <div className="toggle-btn" onClick={this.handleToggle}>
                                <span className="lines" />
                            </div>
                            <ul className="nav navbar-nav">
                                {
                                    menus.map((item, index) => {
                                        let isActive = this.isActive(item, currentUrl);
                                        const activeClass = isActive ? 'active' : '';
                                        if (item.childs.length > 0) {
                                            return (
                                                <li key={item.menuItemId} className={`dropdown ${activeClass}`}>
                                                    <a>{item.menuItemName} <i className="fa fa-angle-down" /></a>
                                                    <ul className="dropdown-menu">
                                                        {
                                                            item.childs.map(child => {
                                                                isActive = child.url.toLowerCase().localeCompare(currentUrl) === 0;
                                                                return (
                                                                    <li key={child.menuItemId} className={`dropdown ${isActive ? 'active' : ''}`}>
                                                                        <a href={child.url}>{child.menuItemName}</a>
                                                                    </li>
                                                                );
                                                            })
                                                        }
                                                    </ul>
                                                </li>
                                            );
                                        } else {
                                            return (
                                                <li key={item.menuItemId} className={`${activeClass}`}>
                                                    <a href={item.url}>{item.menuItemName}</a>
                                                </li>
                                            );
                                        }
                                    })
                                }
                            </ul>
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