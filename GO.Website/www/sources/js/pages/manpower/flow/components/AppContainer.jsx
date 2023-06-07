import React, { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';
import { toast } from 'react-toastify';

import UnitList from './UnitList';
import FlowSetting from './FlowSetting';
import SweetModalDialog from '@jsroot/components/SweetModalDialog';
import Site from '@jsroot/common/site';
import LangHelper from '@jsroot/common/langHelper';
import DataService from '@jsroot/common/dataService';
import { ManpowerFlow } from '@jsroot/common/constants';

const AppContainer = () => {
    const [units, setUnits] = useState([]);
    const [selectedUnit, setSelectedUnit] = useState(null);
    const [flows, setFlows] = useState([]);
    const [showDeleteModalDialog, setShowDeleteModalDialog] = useState(false);

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
        const url = Site.resolveClientUrl('Manpower/Flow/GetByUnit');
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
        const flowItems = [...flows];
        const index = flowItems.findIndex(x => x.processId === processId && x.siteId === item.siteId && x.empId === item.empId);
        flowItems.splice(index, 1);
        setFlows(flowItems);
    };

    const validateFlow = () => {
        let isValid = true;
        if (flows.filter(x => x.processId === ManpowerFlow.UnitHead).length === 0
            || flows.filter(x => x.processId === ManpowerFlow.Verifier).length === 0
            || flows.filter(x => x.processId === ManpowerFlow.Approver).length === 0
            || flows.filter(x => x.processId === ManpowerFlow.HR).length === 0) {
            isValid = false;
        }

        return isValid;
    };

    const updateFlow = async () => {
        Site.blockUI();
        const params = {
            unitName: selectedUnit.name,
            flows
        };
        const url = Site.resolveClientUrl('Manpower/Flow/Update');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Update successfully!');

                if (!selectedUnit.hasFlow) {
                    const tempUnits = [...units];
                    const index = tempUnits.findIndex(x => x.name === selectedUnit.name);
                    tempUnits[index] = { ...tempUnits[index], hasFlow: true };

                    setSelectedUnit({ ...selectedUnit, hasFlow: true });
                    setUnits(tempUnits);
                }
            } else {
                toast.error(response.data.message);
            }
        }).then(() => Site.unblockUI());
    };

    const handleUpdateFlow = async () => {
        const isValid = validateFlow();
        if (!isValid) {
            toast.error('All steps is required.');
        } else {
            await updateFlow();
        }
    };

    const handleDeleteFlow = () => {
        setShowDeleteModalDialog(true);
    };

    const handleCloseDeleteModalDialog = () => {
        setShowDeleteModalDialog(false);
    };

    const deleteFlow = async () => {
        Site.blockUI();
        const params = { unitName: selectedUnit.name };
        const url = Site.resolveClientUrl('Manpower/Flow/Delete');
        await DataService.postDataAsync(url, params).then(response => {
            if (response.data.isSuccessful) {
                toast.success('Delete successfully!');

                const tempUnits = [...units];
                const index = tempUnits.findIndex(x => x.name === selectedUnit.name);
                tempUnits[index] = { ...tempUnits[index], hasFlow: false };

                setSelectedUnit({ ...selectedUnit, hasFlow: false });
                setUnits(tempUnits);
                setFlows([]);
                handleCloseDeleteModalDialog();
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
                onDelete={handleDeleteFlow}
            />
            {
                showDeleteModalDialog &&
                <SweetModalDialog
                    title={LangHelper.getResource('Confirmation')}
                    contents={<div>Are you sure you want to delete flow of <strong>{selectedUnit.name}</strong>?</div>}
                    buttons={[
                        <Button variant="primary" size="sm" key="ok"
                            onClick={deleteFlow}
                        >
                            {LangHelper.getResource('OK')}
                        </Button>,
                        <Button variant="secondary" size="sm" key="cancel"
                            onClick={handleCloseDeleteModalDialog}
                        >
                            {LangHelper.getResource('Cancel')}
                        </Button>
                    ]}
                    onClose={handleCloseDeleteModalDialog}
                />
            }
        </div>
    );
};

export default AppContainer;