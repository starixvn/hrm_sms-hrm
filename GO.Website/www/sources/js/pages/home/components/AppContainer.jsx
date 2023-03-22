import React, { Component, Fragment } from 'react';
import { Card, ListGroup } from 'react-bootstrap';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

class AppContainer extends Component {
    constructor(props) {
        super(props);

        this.state = {
            sites: []
        };
    }

    async componentDidMount() {
        const url = Site.resolveClientUrl('home/getSiteMenus');
        await DataService.getDataAsync(url).then(response => {
            this.setState({ sites: response.data });
        });
    }

    renderChildMenus = menus => {
        const rows = [];
        for (const menu of menus) {
            const row = <ListGroup.Item key={menu.menuItemId} className={!menu.url ? 'none' : ''}>
                {
                    menu.url &&
                    <Card.Link onClick={event => this.onClick(event, menu.url)}> {menu.menuItemName}</Card.Link>
                }
                {!menu.url && <span>{menu.menuItemName}</span>}
            </ListGroup.Item>;

            rows.push(row);

            if (menu.childs.length > 0) {
                rows.push(...this.renderChildMenus(menu.childs));
            }
        }

        return rows;
    };

    onClick = async (event, menuUrl) => {
        event.preventDefault();

        let url = Site.resolveClientUrl('sso/gettime');
        await DataService.getDataAsync(url).then(response => {
            url = `${menuUrl}&time=${response.data}`;
            window.open(url, '_blank', 'noopener,noreferrer');
        });
    };

    handleDropdown = event => {
        event.currentTarget.classList.toggle('show-sub-item');
    };

    render() {
        const { sites } = this.state;
        return (
            <div className="sites">
                {
                    sites.map(site => {
                        return (
                            <Card key={site.siteId}>
                                <Card.Header>
                                    <a onClick={event => this.onClick(event, site.siteURL)}>{site.siteName}</a>
                                </Card.Header>
                                <ListGroup variant="flush">
                                    {
                                        site.menuItems.map(menu => {
                                            return (
                                                <Fragment key={menu.menuItemId}>
                                                    <div className="sub-group-item">
                                                        <ListGroup.Item className={`parent-item ${!menu.url ? '' : 'none'}`} onClick={this.handleDropdown}>
                                                            {
                                                                menu.url &&
                                                                <Card.Link onClick={event => this.onClick(event, menu.url)} >
                                                                    {menu.menuItemName}
                                                                </Card.Link>
                                                            }
                                                            {!menu.url && <span>{menu.menuItemName}<i className="fa fa-angle-down"></i></span>}
                                                        </ListGroup.Item>
                                                        <div className={!menu.url ? 'sub-item' : ''}>{this.renderChildMenus(menu.childs)}</div>
                                                    </div>
                                                </Fragment>
                                            );
                                        })
                                    }
                                </ListGroup>
                            </Card>
                        );
                    })
                }
            </div>
        );
    }
}

export default AppContainer;