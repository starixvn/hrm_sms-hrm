import React, { Fragment, useState, useEffect } from 'react';
import axios from 'axios';

import Table from './Table';
import Popup from './Popup';
import Filter from './Filter';

import { resolveClientUrl, axiosError, alert, confirm } from '../../../shared/common.functions';

const AppContainer = () => {
    const [createModal, setCreateModal] = useState(false);
    const [editModal, setEditModal] = useState(false);
    const [jobTitle, setJobTitle] = useState([]);
    const [jobCategory, setJobCategory] = useState([]);
    const [keyword, setKeyword] = useState('');
    const [jobTitleEdit, setJobTitleEdit] = useState([]);

    const handleGetJobTitle = () => {
        axios.post(resolveClientUrl('JobTitle/GetJobTitles'), {
            jobTitle: keyword
        }).then(response => {
            setJobTitle(response.data)
        }).catch(axiosError);
    };

    const handleGetJobCategory = () => {
        axios.post(resolveClientUrl('JobTitle/GetJobCategory'), {
        }).then(response => {
            setJobCategory(response.data)
        }).catch(axiosError);
    };

    useEffect(() => {
        handleGetJobTitle();
        handleGetJobCategory();
    }, []);

    const handleEditInfo = (id, name, categoryId) => {
        setEditModal(true);
        setJobTitleEdit({
            id: id,
            name: name,
            categoryId: categoryId
        });
    };

    const handleCreate = jobTitle => {
        axios.post(resolveClientUrl(`JobTitle/Insert`), {
            name: jobTitle.name.trim(),
            categoryId: jobTitle.categoryId ? jobTitle.categoryId : jobCategory[0].value
        })
            .then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    setCreateModal(false);
                    alert('Create successfully!');
                    handleGetJobTitle();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
    };

    const handleDelete = (id) => {
        confirm('Are you sure you want to delete?', () => {
            axios.post(resolveClientUrl('JobTitle/Delete'), {
                id: id
            }).then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    alert('Delete successfully!');
                    handleGetJobTitle();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
        });
    };

    const handleEdit = (values) => {
        axios.post(resolveClientUrl(`JobTitle/Update`), {
            id: jobTitleEdit.id,
            name: values.name.trim(),
            categoryId: values.categoryId
        })
            .then(response => {
                const { data } = response;
                if (data.IsSuccessful) {
                    setEditModal(false);
                    alert('Edit successfully!');
                    handleGetJobTitle();
                } else {
                    alert(data.Message);
                }
            }).catch(axiosError);
    };

    return (
        <Fragment>
            <Filter
                keyword={keyword}
                setKeyword={setKeyword}
                searchKeyword={handleGetJobTitle}
                setCreateModal={setCreateModal} />
            <Popup
                show={createModal}
                onHide={() => setCreateModal(false)}
                onSubmit={handleCreate}
                title='Add'
                jobCategory={jobCategory} />
            <Popup
                show={editModal}
                onHide={() => setEditModal(false)}
                onSubmit={handleEdit}
                title='Edit'
                jobCategory={jobCategory}
                data={jobTitleEdit} />
            <Table
                data={jobTitle}
                handleEditInfo={handleEditInfo}
                handleDelete={handleDelete} />
        </Fragment>
    )
};

export default AppContainer;