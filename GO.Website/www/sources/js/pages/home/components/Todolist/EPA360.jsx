import React, { Component } from 'react';
import { Scrollbars } from 'react-custom-scrollbars-2';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import SkeletonTodoItem from '../SkeletonTodoItem';

export default class EPA360 extends Component {
    constructor(props) {
        super(props);

        this.state = {
            todolist: null
        };
    }

    async componentDidMount() {
        await this.getTodolist();
    }

    getTodolist = async () => {
        const url = Site.resolveClientUrl('Todolist/GetEpa360Todolist');
        await DataService.getDataAsync(url).then(response => {
            this.setState({ todolist: response.data });
        });
    };

    onClick = async (event, siteId, action) => {
        event.preventDefault();

        if (action.isRedirect) {
            const url = Site.resolveClientUrl('Todolist/processAction');
            const params = { siteId, action };
            await DataService.postDataAsync(url, params).then(response => {
                window.open(response.data, '_blank', 'noopener,noreferrer');
            });
        }
    };

    render() {
        const { todolist } = this.state;

        return (
            <div className="card">
                <div className="card-content">
                    <div className="card-title">
                        <span className="txt">360° EPA</span>
                    </div>
                    <div className="card-body">
                        <Scrollbars autoHeight autoHeightMax={500}>
                            {
                                todolist === null &&
                                <SkeletonTodoItem />
                            }
                            {
                                todolist !== null &&
                                <div className="todolist-items">
                                    {
                                        todolist.length > 0 &&
                                        todolist.map(item =>
                                            <div key={`epa-todolist-${item.id}`} className="todolist-item green-border-left">
                                                <div className="txt" dangerouslySetInnerHTML={{ __html: item.name }} />
                                                {
                                                    item.actions &&
                                                    <div className="todolist-actions">
                                                        {
                                                            item.actions.map(action =>
                                                                <button key={`epa-todolist-${item.id}-action-${action.id}`}
                                                                    className="btn btn-primary"
                                                                    onClick={event => this.onClick(event, item.siteId, action)}
                                                                >
                                                                    {action.name}
                                                                </button>
                                                            )
                                                        }
                                                    </div>
                                                }
                                            </div>
                                        )
                                    }
                                    {
                                        todolist.length === 0 &&
                                        <p className="todolist-item green-border-left">
                                            <span className="txt">
                                                Nothing to do
                                            </span>
                                        </p>
                                    }
                                </div>
                            }
                        </Scrollbars>
                    </div>
                </div>
            </div>
        );
    }
}