import React, { Component } from 'react';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import ProcessRequestDialog from './ProcessRequestDialog';

export default class HRM extends Component {
    constructor(props) {
        super(props);

        this.state = {
            processRequestDialog: {
                open: false,
                action: null
            }
        };
    }

    onClick = async (event, siteId, action) => {
        event.preventDefault();

        if (action.isRedirect) {
            const url = Site.resolveClientUrl('home/processAction');
            const params = { siteId, action };
            await DataService.postDataAsync(url, params).then(response => {
                window.open(response.data, '_blank', 'noopener,noreferrer');
            });
        } else {
            this.setState({
                processRequestDialog: {
                    open: true,
                    siteId,
                    action
                }
            });
        }
    };

    handleCloseRequestDialog = () => {
        this.setState({
            processRequestDialog: {
                open: false,
                siteId: null,
                action: null
            }
        });
    };

    render() {
        const todolist = this.props.todolist;
        const processRequestDialog = this.state.processRequestDialog;

        return (
            <>
                <div className="todolist-items">
                    {
                        todolist && todolist.length > 0 &&
                        <>
                            {
                                todolist.map(item =>
                                    <div key={`hrm-todolist-${item.id}`} className="todolist-item green-border-left">
                                        <div className="txt" dangerouslySetInnerHTML={{ __html: item.name }} />
                                        {
                                            item.actions &&
                                            <div className="todolist-actions">
                                                {
                                                    item.actions.map(action =>
                                                        <button key={`hrm-todolist-${item.id}-action-${action.id}`}
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
                        </>
                    }
                    {
                        (!todolist || todolist.length === 0) &&
                        <div className="todolist-items">
                            <p className="todolist-item green-border-left">
                                <span className="txt">
                                    Nothing to do
                                </span>
                            </p>
                        </div>
                    }
                </div>
                {
                    processRequestDialog.open &&
                    <ProcessRequestDialog
                        siteId={processRequestDialog.siteId}
                        action={processRequestDialog.action}
                        onClose={this.handleCloseRequestDialog}
                    />
                }
            </>
        );
    }
}