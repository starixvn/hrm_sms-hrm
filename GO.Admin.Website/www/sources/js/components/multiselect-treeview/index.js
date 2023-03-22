import React from 'react';
import PropTypes from 'prop-types';

import buildData from './buildData';
import TreeNode from './TreeNode';
import './MultiselectTreeView.scss';

const updateParent = (floatData, parentKey, isChecked) => {
    const index = floatData.findIndex(item => item.key === parentKey),
        siblings = floatData.filter(item => item.parentKey === parentKey),
        selected = siblings.filter(item => item.checked);
    floatData[index] = { ...floatData[index], checked: siblings.length === selected.length };

    if (floatData[index].parentKey !== null && floatData[index].parentKey !== '') {
        updateParent(floatData, floatData[index].parentKey, isChecked);
    }
};

const updateChild = (floatData, itemkey, isChecked) => {
    const totalItems = floatData.length;
    for (let i = 0; i < totalItems; i++) {
        if (floatData[i].parentKey === itemkey) {
            floatData[i] = { ...floatData[i], checked: isChecked };
            updateChild(floatData, floatData[i].key, isChecked);
        }
    }
};

const getSelectedText = (props, floatData) => {
    const { texts, maxDisplayItemCount } = props;
    const items = floatData.filter(item => item.isLeaf),
        selected = items.filter(item => item.checked),
        selectedItemCount = selected.length;
    let text = '';

    if (selectedItemCount === 0) {
        text = texts.placeholder;
    } else if (items.length === selectedItemCount) {
        text = texts.all;
    } else if (selectedItemCount > maxDisplayItemCount) {
        text = texts.selectedItemCount.replace('#', selectedItemCount);
    } else if (selectedItemCount >= 1) {
        text = selected.map(item => item.text).join(', ');
    } else {
        text = '';
    }

    return text;
};

const getSelectedItems = (floatData) => {
    return floatData.filter(item => item.isLeaf && item.checked);
};

let id = 1;

export default class MultiselectTreeView extends React.Component {
    static propTypes = {
        data: PropTypes.arrayOf(PropTypes.object).isRequired,
        texts: PropTypes.object,
        isSelectedAll: PropTypes.bool,
        maxDisplayItemCount: PropTypes.number,
        hasSearchBox: PropTypes.bool,
        onChange: PropTypes.func
    }

    static defaultProps = {
        data: [],
        texts: {
            all: 'All',
            selectedItemCount: "# selected",
            placeholder: "Please select..."
        },
        isSelectedAll: false,
        maxDisplayItemCount: 1,
        hasSearchBox: false,
        onChange: function () { }
    }

    static getDerivedStateFromProps(props, state) {
        if (state.original === props.data) {
            return null;
        }

        return { ...buildData(props.data || [], props.isSelectedAll), original: props.data };
    }

    constructor(props) {
        super(props);
        this.id = `multiselect-treeview-${props.id || id++}`;
        this.state = {
            searchTerm: '',
            selectedText: props.texts.placeholder,
            opened: false
        };
    }

    componentDidMount() {
        document.addEventListener('click', this.handleDocumentClick);
    }

    componentWillUnmount() {
        document.removeEventListener('click', this.handleDocumentClick);
    }

    handleDocumentClick = (event) => {
        const clickOutside = this.wrapper && !this.wrapper.contains(event.target);
        if (clickOutside && this.state.opened) {
            this.setState({ opened: false });
        }
    }

    onToggle = () => {
        this.setState((prevState) => { return { opened: !prevState.opened }; });
    }

    handleSelectAll = event => {
        let floatData = [...this.state.floatData];
        const totalItems = floatData.length;
        for (let i = 0; i < totalItems; i++) {
            floatData[i] = { ...floatData[i], checked: event.target.checked };
        }

        this.props.onChange && this.props.onChange(getSelectedItems(floatData));
        this.setState({ floatData });
    }

    handleSelect = (itemkey, isChecked) => {
        let floatData = [...this.state.floatData];
        const index = floatData.findIndex(item => item.key === itemkey);
        floatData[index] = { ...floatData[index], checked: isChecked };

        updateChild(floatData, itemkey, isChecked);
        if (floatData[index].parentKey !== null && floatData[index].parentKey !== '') {
            updateParent(floatData, floatData[index].parentKey, isChecked);
        }

        this.props.onChange && this.props.onChange(getSelectedItems(floatData));
        this.setState({ floatData });
    }

    handleCollaseExpand = item => {
        let floatData = [...this.state.floatData];
        const itemkey = item.key,
            expanded = !item.expanded,
            totalItems = floatData.length;
        const index = floatData.findIndex(item => item.key === itemkey);
        floatData[index] = { ...floatData[index], expanded: expanded };

        for (let i = 0; i < totalItems; i++) {
            if (floatData[i].parentKey === itemkey) {
                floatData[i] = { ...floatData[i], visible: expanded };
            }
        }

        this.setState({ floatData });
    }

    handleSearch = value => {
        const _updateParent = function (floatData, itemKeys) {
            const totalKeys = itemKeys.length,
                parentKeys = [];
            for (let i = 0; i < totalKeys; i++) {
                const index = floatData.findIndex(item => item.key === itemKeys[i]),
                    siblings = floatData.filter(item => item.parentKey === itemKeys[i]);
                floatData[index] = {
                    ...floatData[index],
                    visible: siblings.filter(item => item.visible).length > 0,
                    expanded: siblings.filter(item => item.visible).length > 0 ? true : floatData[index].expanded
                };

                if (floatData[index].parentKey && parentKeys.indexOf(floatData[index].parentKey) === -1) {
                    parentKeys.push(floatData[index].parentKey);
                }
            }

            if (parentKeys.length > 0) {
                _updateParent(floatData, parentKeys);
            }
        };

        this.setState(prevState => {
            const floatData = [...prevState.floatData],
                totalItems = floatData.length,
                parentKeys = [];
            for (let i = 0; i < totalItems; i++) {
                if (floatData[i].isLeaf) {
                    floatData[i] = { ...floatData[i], visible: floatData[i].text.toLowerCase().indexOf(value.toLowerCase()) !== -1 };

                    if (floatData[i].parentKey && parentKeys.indexOf(floatData[i].parentKey) === -1) {
                        parentKeys.push(floatData[i].parentKey);
                    }
                }
            }

            if (parentKeys.length > 0) {
                _updateParent(floatData, parentKeys);
            }

            return {
                searchTerm: value,
                floatData
            };
        });
    }

    handleClearSearch = () => {
        this.handleSearch('');
    }

    renderSearchBox() {
        if (!this.props.hasSearchBox) {
            return null;
        }

        return (
            <div className="multiselect-treeview-search-box">
                <input autoFocus onChange={event => this.handleSearch(event.target.value)}
                    ref={element => this.searchInputBox = element}
                    value={this.state.searchTerm} type="text" placeholder="Search data" />
                <span className="clear-search-box" onClick={this.handleClearSearch}>x</span>
            </div>
        );
    }

    render() {
        const { floatData, data } = this.state;
        const selected = floatData.filter(item => item.checked);
        const selectedText = getSelectedText(this.props, floatData);

        return (
            <div id={this.id} className="multiselect-treeview" ref={element => this.wrapper = element}
                style={{ width: this.props.width ? this.props.width : '100%' }}>
                <div className="multiselect-treeview-dropdown-button" onClick={this.onToggle}>
                    <span className="selected-text">{selectedText}</span>
                    <span className="up-down-icon"><span className={this.state.opened ? 'up' : 'down'} /></span>
                </div>
                <div className={`multiselect-treeview-dropdown-list ${this.state.opened ? '' : 'hidden'}`}>
                    {this.renderSearchBox()}
                    {
                        floatData && floatData.length > 0 &&
                        <div className="multiselect-treeview-select-all">
                            <div className="checkbox-wrapper">
                                <div>
                                    <input type="checkbox" id={`${this.id}-selectAll`}
                                        checked={floatData.length === selected.length}
                                        onChange={this.handleSelectAll}
                                    />
                                    <label htmlFor={`${this.id}-selectAll`}> All</label>
                                </div>
                            </div>
                        </div>
                    }
                    <div className="multiselect-treeview-items">
                        {
                            data.map(item => {
                                const itemIndex = floatData.findIndex(child => child.key === item.key);
                                return floatData[itemIndex].visible ?
                                    <ul key={item.key}>
                                        <li className="multiselect-treeview-item">
                                            <TreeNode
                                                id={this.id}
                                                data={item}
                                                floatData={floatData}
                                                onCollaseExpand={this.handleCollaseExpand}
                                                onSelect={this.handleSelect}
                                            />
                                        </li>
                                    </ul> : null;
                            })
                        }
                    </div>
                </div>
            </div>
        );
    }
}