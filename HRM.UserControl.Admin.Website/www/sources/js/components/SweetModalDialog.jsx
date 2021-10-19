import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { Modal } from 'react-bootstrap';
import DraggableModalDialog from './DraggableModalDialog';

class SweetModalDialog extends Component {
    static propTypes = {
        backdropClassName: PropTypes.string,
        buttons: PropTypes.arrayOf(PropTypes.element),
        className: PropTypes.string,
        contents: PropTypes.element.isRequired,
        onClose: PropTypes.func.isRequired,
        size: PropTypes.oneOf(['sm', 'lg', 'xl']),
        title: PropTypes.string.isRequired
    };

    static defaultProps = {
        backdropClassName: 'sweet-alert-modal-backdrop',
        className: 'sweet-alert-modal',
        buttons: [],
        size: 'sm',
        title: '',
        contents: <Fragment />,
        onClose: function () {
            // nothing
        }
    };

    constructor(props) {
        super(props);
    }

    render() {
        const { size, title, contents, buttons, backdropClassName, className } = this.props;
        return (
            <Modal show backdrop="static" dialogAs={DraggableModalDialog}
                backdropClassName={backdropClassName} className={className}
                keyboard={false} size={size} centered
                onHide={this.props.onClose}
            >
                <Modal.Header closeButton>
                    <Modal.Title as="div">{title}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {contents}
                </Modal.Body>
                {
                    buttons && buttons.length > 0 &&
                    <Modal.Footer>
                        {
                            buttons.map((btnItem, index) => <Fragment key={index}>{btnItem}</Fragment>)
                        }
                    </Modal.Footer>
                }
            </Modal>
        );
    }
}

export default SweetModalDialog;