import React, { useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import UnitList from './UnitList';
import FlowSetting from './FlowSetting';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

const AppContainer = () => {
    const [units, setUnits] = useState([]);
    const [selectedUnit, setSelectedUnit] = useState(null);
    const [flows, setFlows] = useState([]);

    useEffect(() => {
        const getUnits = async () => {
            Site.blockUI();
            const url = Site.resolveClientUrl('Manpower/Flow/GetUnits');
            await DataService.getDataAsync(url).then(response => {
                setUnits(response.data);
            }).then(() => Site.unblockUI());
        };

        getUnits();
    }, []);

    const getFlowByUnit = async unitName => {
        Site.blockUI();
        const url = Site.resolveClientUrl('Manpower/Flow/GetFlowByUnit');
        await DataService.getDataAsync(url, { unitName }).then(response => {
            setFlows(response.data);
        }).then(() => Site.unblockUI());
    };

    const handleSelectUnit = async unit => {
        setSelectedUnit(unit);

        await getFlowByUnit(unit.name);
    };

    const handleAddFlowItem = (processId, item) => {
        const index = flows.findIndex(x => x.processId === processId && x.siteId === item.siteId && x.empId === item.empId);
        if (index < 0) {
            const flowItems = flows;
            flowItems.push({
                processId,
                siteId: item.siteId,
                siteName: item.siteName,
                empId: item.empId,
                code: item.code,
                nickname: item.nickname,
                fullName: item.fullName
            });
            setFlows(flowItems);
        }
    };

    const handleRemoveFlowItem = (processId, item) => {
        const flowItems = flows;
        const index = flowItems.findIndex(x => x.processId === processId && x.siteId === item.siteId && x.empId === item.empId);
        flowItems.splice(index, 1);
        setFlows(flowItems);
    };

    const handleUpdateFlow = async () => {
        Site.blockUI();
        const params = {
            unitName: selectedUnit.name,
            flows
        };
        const url = Site.resolveClientUrl('Manpower/Flow/UpdateFlow');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Update successfully!');
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    return (
        <div className="manpower-flow">
            <UnitList units={units}
                onClick={handleSelectUnit}
            />
            <FlowSetting selectedUnit={selectedUnit}
                flows={flows}
                onAddFlowItem={handleAddFlowItem}
                onRemoveFlowItem={handleRemoveFlowItem}
                onSubmit={handleUpdateFlow}
            />
        </div>
    );
};

export default AppContainer;