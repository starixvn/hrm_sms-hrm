import React, { PureComponent } from 'react';
import { Dropdown } from 'react-bootstrap';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

export default class Header extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            menus: [],
            nickname: '',
            expended: false
        };
    }

    async componentDidMount() {
        const url = Site.resolveClientUrl('auth/getHeader');
        await DataService.postDataAsync(url).then(response => {
            const { menus, nickname } = response.data;
            this.setState({ menus, nickname });
        });
    }

    handleToggle = () => {
        this.setState(prevState => {
            return { expended: !prevState.expended };
        });
    };

    isActive = (menuItem, currentUrl) => (menuItem.link !== null && menuItem.link.toLowerCase().localeCompare(currentUrl) === 0)
        || menuItem.childrenMenus.filter(child => child.link.toLowerCase().localeCompare(currentUrl) === 0).length > 0;

    render() {
        const { menus, nickname, expended } = this.state;
        const currentUrl = document.getElementById('currentUrl').value.toLowerCase();
        return (
            <nav className="navbar navbar-inverse">
                <div className="pull-left left-bar">
                    <a className="navbar-brand" href={Site.resolveClientUrl('home/index')}>
                        <img alt="logo" src={Site.resolveClientUrl('css/images/logo2.svg')} alt="User Control" />
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
                                if (item.childrenMenus.length > 0) {
                                    return (
                                        <li key={index} className={`dropdown ${activeClass}`}>
                                            <a>{item.name} <i className="fa fa-angle-down" /></a>
                                            <ul className="dropdown-menu">
                                                {
                                                    item.childrenMenus.map(child => {
                                                        isActive = child.link.toLowerCase().localeCompare(currentUrl) === 0;
                                                        return (
                                                            <li key={child.menuId} className={`dropdown ${isActive ? 'active' : ''}`}>
                                                                <a href={child.link}>{child.name}</a>
                                                            </li>
                                                        );
                                                    })
                                                }
                                            </ul>
                                        </li>
                                    );
                                } else {
                                    return (
                                        <li key={index} className={`${activeClass}`}>
                                            <a href={item.link}>{item.name}</a>
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
                                <i className="fa fa-user" style={{ marginTop: 10 }} />
                            </span>
                        </Dropdown.Toggle>
                        <Dropdown.Menu>
                            <Dropdown.Item href={Site.resolveClientUrl('auth/logout')}><i className="fa fa-power-off" /> Sign Out</Dropdown.Item>
                        </Dropdown.Menu>
                    </Dropdown>
                </div>
            </nav>
        );
    }
}