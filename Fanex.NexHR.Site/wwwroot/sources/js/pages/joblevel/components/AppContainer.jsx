import React, { Fragment, useState, useEffect } from 'react';
import { Button } from 'react-bootstrap';
import { Table } from './Table';
import { Create } from './Create';
import { Edit } from './Edit';
import axios from 'axios';
import { resolveClientUrl, axiosError } from '../../../shared/common.functions';

export const AppContainer = () => {
    const [createModal, setCreateModal] = useState(false);
    const [editModal, setEditModal] = useState(false);
    const [jobLevel, setJobLevel] = useState([]);
    const [idJobLevelEdit, setIdJobLevelEdit] = useState(0);
    const [nameJobLevelEdit, setNameJobLevelEdit] = useState('');

    const handleCreateJobLevel = () => {
        setCreateModal(true);
    };

    const handleGetJobLevel = () => {
        axios.post(resolveClientUrl('JobLevel/GetJobLevel'), {
        }).then(response => {
            setJobLevel(response.data)
        }).catch(axiosError);
    }

    useEffect(() => {
        handleGetJobLevel();
    }, []);

    const handleEditInfo = (id, name) => {
        setIdJobLevelEdit(id);
        setNameJobLevelEdit(name);
    };

    return (
        <Fragment>
            {window.hasPermission &&
                <div className="filter">
                    <Button className="btn btn-request" type="button" onClick={handleCreateJobLevel}>
                        <span className="fa fa-plus" /> New Job Level
                    </Button>
                </div>
            }
            <Create createModal={createModal} setCreateModal={setCreateModal} />
            <Edit editModal={editModal} setEditModal={setEditModal} id={idJobLevelEdit} name={nameJobLevelEdit} />
            <Table data={jobLevel} setEditModal={setEditModal} editJobLevel={handleEditInfo} />
        </Fragment>
    )
};