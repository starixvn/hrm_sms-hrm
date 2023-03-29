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
        const todoist = this.props.todoist;
        const processRequestDialog = this.state.processRequestDialog;

        return (
            <>
                <div className="todoist-items">
                    {
                        todoist && todoist.length > 0 &&
                        <>
                            {
                                todoist.map(item =>
                                    <p key={`hrm-todoist-${item.id}`} className="todoist-item green-border-left">
                                        <span className="txt">{item.name}</span>
                                        {
                                            item.actions &&
                                            item.actions.map(action =>
                                                <button key={`hrm-todoist-${item.id}-action-${action.id}`}
                                                    className="btn btn-primary"
                                                    onClick={event => this.onClick(event, item.siteId, action)}
                                                >
                                                    {action.name}
                                                </button>
                                            )
                                        }
                                    </p>
                                )
                            }
                        </>
                    }
                    {
                        (!todoist || todoist.length === 0) &&
                        <p className="green-border-left">
                            <span>
                                Nothing to do
                            </span>
                        </p>
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