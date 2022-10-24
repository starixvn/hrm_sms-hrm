import React, { PureComponent } from 'react';
import { Dropdown } from 'react-bootstrap';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

export default class Header extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            user: null
        };
    }

    async componentDidMount() {
        const url = Site.resolveClientUrl('home/getUserInfo');
        await DataService.postDataAsync(url).then(response => {
            this.setState({ user: response.data });
        });
    }

    render() {
        const { user } = this.state;
        let nickname = '';
        if (user) {
            nickname = user.username ? user.username : user.nickname;
        }

        return (
            <nav className="navbar navbar-inverse">
                <div className="pull-left left-bar">
                    <a className="navbar-brand" href={Site.resolveClientUrl('home/index')}>
                        <img src={Site.resolveClientUrl('css/images/logo2.svg')} alt="GO System" />
                    </a>
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