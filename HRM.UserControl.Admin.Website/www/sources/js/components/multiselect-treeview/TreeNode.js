import React, { PureComponent, Fragment } from 'react';

export default class TreeNode extends PureComponent {
    handleChange = event => {
        const itemkey = this.props.data.key;
        this.props.onSelect(itemkey, event.target.checked);
    }

    render() {
        const { id, data, floatData } = this.props;
        const itemIndex = floatData.findIndex(item => item.key === data.key);

        if (itemIndex < 0) {
            return null;
        }

        return (
            <Fragment>
                <div className="checkbox-wrapper">
                    {
                        data.items && data.items.length > 0 &&
                        <span className="collapse-expand">
                            <span className={floatData[itemIndex].expanded ? 'expanded' : 'collapsed'}
                                onClick={() => this.props.onCollaseExpand(floatData[itemIndex])}
                            />
                        </span>
                    }
                    <div>
                        <input type="checkbox" id={`${id}-${data.key}`}
                            checked={floatData[itemIndex].checked}
                            onChange={this.handleChange}
                        />
                        <label htmlFor={`${id}-${data.key}`}> {data.text}</label>
                    </div>
                </div>
                {
                    data.items && data.items.length > 0 &&
                    <ul>
                        {
                            data.items.map(item => {
                                const childIndex = floatData.findIndex(child => child.key === item.key);
                                const isLastChild = item.items === null || item.items.length === 0;

                                return floatData[childIndex].visible ?
                                    <li key={item.key} className={`multiselect-treeview-item is-child ${isLastChild ? 'last-child' : ''}`}>
                                        <TreeNode id={id}
                                            data={item}
                                            floatData={floatData}
                                            onSelect={this.props.onSelect}
                                            onCollaseExpand={this.props.onCollaseExpand}
                                        />
                                    </li> : null;
                            })
                        }
                    </ul>
                }
            </Fragment>
        );
    }
}