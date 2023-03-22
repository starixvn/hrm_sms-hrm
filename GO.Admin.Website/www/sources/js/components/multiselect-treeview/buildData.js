const getChilds = (floatData, data, parentKey, isChecked = false) => {
    const totalItems = data.length;
    for (let i = 0; i < totalItems; i++) {
        const key = parentKey + '-' + i;

        floatData.push({
            ...data[i],
            checked: isChecked,
            key: key,
            visible: true,
            expanded: true,
            parentKey: parentKey
        });
        data[i] = {
            ...data[i],
            checked: isChecked,
            key: key,
            visible: true,
            expanded: true,
            items: data[i].items ? [...data[i].items] : []
        };
        if (data[i].items && data[i].items.length > 0) {
            getChilds(floatData, data[i].items, key, isChecked);
        }
    }
};

export default (data, isChecked = false) => {
    const tempData = [],
        floatData = [],
        totalItems = data.length;
    for (let i = 0; i < totalItems; i++) {
        const key = 'item-' + i;

        floatData.push({
            ...data[i],
            checked: isChecked,
            key: key,
            visible: true,
            expanded: true,
            parentKey: ''
        });
        tempData.push({
            ...data[i],
            checked: isChecked,
            key: key,
            visible: true,
            expanded: true,
            items: data[i].items ? [...data[i].items] : []
        });
        if (tempData[i].items && tempData[i].items.length > 0) {
            getChilds(floatData, tempData[i].items, key, isChecked);
        }
    }

    return { data: tempData, floatData };
};