import React, { Fragment, useState, useEffect } from 'react';
import { Table } from './Table';
import { Create } from './Create';
import { Edit } from './Edit';
import { Filter } from './Filter';
import axios from 'axios';
import { resolveClientUrl, axiosError } from '../../../shared/common.functions';

export const AppContainer = () => {
    const [createModal, setCreateModal] = useState(false);
    const [editModal, setEditModal] = useState(false);
    const [jobTitle, setJobTitle] = useState([]);
    const [idJobLevelEdit, setIdJobLevelEdit] = useState(0);
    const [nameJobLevelEdit, setNameJobLevelEdit] = useState('');

    const handleGetJobTitle = () => {
        axios.post(resolveClientUrl('JobTitle/GetJobTitles'), {
        }).then(response => {
            setJobTitle(response.data)
        }).catch(axiosError);
    }

    const reloadTable = () => {
        handleGetJobTitle();
    }

    useEffect(() => {
        handleGetJobTitle();
    }, []);

    const handleEditInfo = (id, name) => {
        setIdJobLevelEdit(id);
        setNameJobLevelEdit(name);
    };

    return (
        <Fragment>
            <Filter />
            {/*<Create createModal={createModal} setCreateModal={setCreateModal} reloadTable={reloadTable} />*/}
            {/*<Edit editModal={editModal} setEditModal={setEditModal} id={idJobLevelEdit} name={nameJobLevelEdit} reloadTable={reloadTable} />*/}
            <Table data={jobTitle} setEditModal={setEditModal} editJobLevel={handleEditInfo} reloadTable={reloadTable} />
        </Fragment>
    )
};