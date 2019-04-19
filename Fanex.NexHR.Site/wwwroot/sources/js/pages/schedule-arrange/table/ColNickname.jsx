import React, { PureComponent } from 'react';
import { OverlayTrigger, Tooltip, Dropdown, MenuItem } from 'react-bootstrap';

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
        this.props.onChangeShowRemarkPopover(false);
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

    handleMoveUp = () => {
        const { employeeId } = this.props;

        this.props.onCustomizeOrder('up', employeeId);
    }

    handleMoveDown = () => {
        const { employeeId } = this.props;
        this.props.onCustomizeOrder('down', employeeId);
    }

    handleMoveTop = () => {
        const { employeeId } = this.props;
        this.props.onCustomizeOrder('top', employeeId);
    }

    handleMoveBottom = () => {
        const { employeeId } = this.props;
        this.props.onCustomizeOrder('bottom', employeeId);
    }

    render() {
        const { isOrderMode, employeeId, nickname, remark } = this.props;

        return (
            <div className="table-cell cell-nickname">
                <div className="cell-nickname-wrapper">
                    <div className="info">
                        <div className="nickname">{nickname}</div>
                        <div className="remark-wrapper">
                            <i className={`icon-remark ${remark !== null && remark.length > 0 ? 'has-remark' : ''} fa fa-commenting-o`}
                                title="Remark"
                                onClick={this.handleShowPopover}
                            />
                            <OverlayTrigger trigger={['hover']} placement={'top'}
                                overlay={<Tooltip id={`remark${employeeId}`}>{remark}</Tooltip>}
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
                    {
                        isOrderMode &&
                        <div className="customize-order">
                            <Dropdown id={`dropdown${employeeId}`} componentClass="div">
                                <Dropdown.Toggle noCaret useAnchor>
                                    <OverlayTrigger trigger={['hover']} placement={'top'}
                                        overlay={<Tooltip id={`customizeOrder${employeeId}`}>Customize order</Tooltip>}
                                    >
                                        <i className="icon icon-sortable" />
                                    </OverlayTrigger>
                                </Dropdown.Toggle>
                                <Dropdown.Menu>
                                    <MenuItem onClick={this.handleMoveUp}>Up</MenuItem>
                                    <MenuItem onClick={this.handleMoveDown}>Down</MenuItem>
                                    <MenuItem onClick={this.handleMoveTop}>Top</MenuItem>
                                    <MenuItem onClick={this.handleMoveBottom}>Bottom</MenuItem>
                                </Dropdown.Menu>
                            </Dropdown>
                        </div>
                    }
                </div>
            </div>
        );
    }
}