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
            const trees = [];
            for (let i = 0; i < menu.level; i++) {
                if (i === menu.level - 1) {
                    trees.push(<span key={i}>&#9492;&#9472;</span>);
                } else {
                    trees.push(<span key={i} className="invisible">&#9492;&#9472;</span>);
                }
            }

            const row = <ListGroup.Item key={menu.menuItemId} className={!menu.url ? 'none' : '' }>
                {
                    menu.url &&
                    <>
                        {trees.map(item => item)}
                        <Card.Link href={menu.url}> {menu.menuItemName}</Card.Link>
                    </>
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

    render() {
        const { sites } = this.state;
        return (
            <div className="sites">
                {
                    sites.map(site => {
                        return (
                            <Card key={site.siteId}>
                                <Card.Header>{site.siteName}</Card.Header>
                                <ListGroup variant="flush">
                                    {
                                        site.menuItems.map(menu => {
                                            return (
                                                <Fragment key={menu.menuItemId}>
                                                    <ListGroup.Item className={!menu.url ? 'none' : ''}>
                                                        {menu.url && <Card.Link href={menu.url}>{menu.menuItemName}</Card.Link>}
                                                        {!menu.url && <span>{menu.menuItemName}</span>}
                                                    </ListGroup.Item>
                                                    {this.renderChildMenus(menu.childs)}
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