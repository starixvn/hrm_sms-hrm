import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';

export default class TooltipRequestInfo extends PureComponent {
    render() {
        return (
            <OverlayTrigger trigger={['hover']} placement={'top'}
                overlay={
                    <Tooltip id={this.props.id}>
                        {this.props.requestInfo.details}
                    </Tooltip>
                }
            >
                {
                    this.props.isLeaveHalfDay
                        ? <span className={`halfdaycode ${this.props.requestInfo.classOfLeave}`}>{this.props.requestInfo.leaveCode}</span>
                        : <i className="fa fa-info-circle" />
                }
            </OverlayTrigger>
        );
    }
}