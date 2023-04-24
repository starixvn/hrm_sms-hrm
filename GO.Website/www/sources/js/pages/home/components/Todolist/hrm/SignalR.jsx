import React, { Component } from 'react';
import { JsonHubProtocol, HubConnectionBuilder, HttpTransportType, LogLevel } from '@microsoft/signalr';

export default class SignalR extends Component {
    constructor(props) {
        super(props);

        this.connection = null;
    }

    componentDidMount() {
        const options = {
            transport: HttpTransportType.WebSockets,
            logMessageContent: false,
            skipNegotiation: true,
            logger: LogLevel.None
        };

        this.connection = new HubConnectionBuilder()
            .withUrl('/todolist/hubs', options)
            .withHubProtocol(new JsonHubProtocol())
            .build();

        this.connection.onclose(async () => {
            await this.start();
        });

        this.connection.on('RefreshHrm', () => {
            this.props.refreshHrm();
        });

        this.start();
    }

    componentWillUnmount() {
        this.connection.stop();
    }

    start = async () => {
        try {
            await this.connection.start().then(() => {
                console.log('SignalR connected!'); //NOSONAR
            });
        } catch (err) {
            console.log(err); //NOSONAR
        }
    };

    render() {
        return <span />;
    }
}