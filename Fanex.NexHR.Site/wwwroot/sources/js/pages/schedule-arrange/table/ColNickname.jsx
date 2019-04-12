import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip, Popover } from 'react-bootstrap';

export default class ColNickname extends PureComponent {
    constructor(props) {
        super(props);

        this.state = {
            remark: props.remark !== null ? props.remark : '',
            showPopover: false
        };
    }

    handleShowPopover = () => {
        const { isHost, showRemarkPopover } = this.props;

        if (isHost && !this.state.showPopover && !showRemarkPopover) {
            this.setState({ showPopover: true });
            this.props.onChangeShowRemarkPopover(true);
        }
    }

    handleHidePopover = () => {
        this.setState({ showPopover: false });
    }

    handleChangeRemark = event => {
        this.setState({ remark: event.target.value });
    }

    handleUpdateRemark = () => {
        const remark = this.state.remark.trim();
        if (remark.length > 0) {
            this.handleHidePopover();
            this.props.onChangeShowRemarkPopover(false);
            this.props.onUpdateRemark(this.props.employeeId, this.state.remark.trim());
        }
    }

    render() {
        const { employeeId, nickname, remark } = this.props;

        return (
            <div className="table-cell cell-nickname">
                <div className="nickname">{nickname}</div>
                <div className="remark-wrapper">
                    <i className={`icon-remark ${remark !== null && remark.length > 0 ? 'has-remark' : ''} fa fa-commenting-o`}
                        title="Remark"
                        onClick={this.handleShowPopover}
                    />
                    <OverlayTrigger trigger={['hover']} placement={'top'}
                        overlay={<Popover id={`remark${employeeId}`}>{remark}</Popover>}
                    >
                        <span className="remark">{remark}</span>
                    </OverlayTrigger>
                    {
                        this.state.showPopover &&
                        <div className="remark-popover">
                            <div className="input-field">
                                <label>Remark</label>
                                <textarea maxLength="200" autoFocus value={this.state.remark} onChange={this.handleChangeRemark} />
                            </div>
                            <div className="btn-control">
                                <a className="btn btn-primary" onClick={this.handleUpdateRemark}>Save</a>
                                <a className="btn btn-default" onClick={this.handleHidePopover}>Cancel</a>
                            </div>
                        </div>
                    }
                </div>
            </div>
        );
    }
}