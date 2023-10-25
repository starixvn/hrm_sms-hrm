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
    const [jobCategory, setJobCategory] = useState([]);
    const [idJobTitleEdit, setIdJobTitleEdit] = useState(0);
    const [nameJobTitleEdit, setNameJobTitleEdit] = useState('');
    const [keyword, setKeyword] = useState('');

    const handleGetJobTitle = () => {
        axios.post(resolveClientUrl('JobTitle/GetJobTitles'), {
            jobTitle: keyword
        }).then(response => {
            setJobTitle(response.data)
        }).catch(axiosError);
    }

    const handleGetJobCategory = () => {
        axios.post(resolveClientUrl('JobTitle/GetJobCategory'), {
        }).then(response => {
            setJobCategory(response.data)
        }).catch(axiosError);
    }

    const reloadTable = () => {
        handleGetJobTitle();
    }

    useEffect(() => {
        handleGetJobTitle();
        handleGetJobCategory();
    }, []);

    const handleEditInfo = (id, name) => {
        setIdJobTitleEdit(id);
        setNameJobTitleEdit(name);
    };

    return (
        <Fragment>
            <Filter setKeyword={setKeyword} searchKeyword={reloadTable} setCreateModal={setCreateModal} />
            <Create createModal={createModal} setCreateModal={setCreateModal} reloadTable={reloadTable} jobCategory={jobCategory} />
            <Edit editModal={editModal} setEditModal={setEditModal} id={idJobTitleEdit} name={nameJobTitleEdit} reloadTable={reloadTable} jobCategory={jobCategory} />
            <Table data={jobTitle} setEditModal={setEditModal} editJobLevel={handleEditInfo} reloadTable={reloadTable} />
        </Fragment>
    )
};