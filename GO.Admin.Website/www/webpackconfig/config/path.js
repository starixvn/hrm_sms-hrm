const path = require('path');

const projectRoot = path.resolve(__dirname, '../../../');

const cssPaths = {
    build: 'wwwroot/css',

    main: ['./www/sources/css/main.scss'],
    error: ['./www/sources/css/pages/error.scss'],
    auth_login: ['./www/sources/css/pages/auth/login.scss'],
    site_index: ['./www/sources/css/pages/site/index.scss'],
    user_index: ['./www/sources/css/pages/user/index.scss'],
    user_assign_site: ['./www/sources/css/pages/user/assign-site.scss'],
    user_permission: ['./www/sources/css/pages/user/permission.scss'],
    user_mapping: ['./www/sources/css/pages/user/user-mapping.scss'],
    function: ['./www/sources/css/pages/function/index.scss'],
    menu: ['./www/sources/css/pages/menu/index.scss'],
    role_index: ['./www/sources/css/pages/role/index.scss'],
    role_permission: ['./www/sources/css/pages/role/permission.scss']
};

const scriptPaths = {
    build: 'wwwroot/js',

    language: ['./www/sources/js/common/languages/ViewResources.js'],
    main: ['./www/sources/js/main.js'],
    auth_login: ['./www/sources/js/pages/auth/login/index.js'],
    site_index: ['./www/sources/js/pages/site/index.js'],
    user_index: ['./www/sources/js/pages/user/index.js'],
    user_assign_site: ['./www/sources/js/pages/user/assign-site/index.js'],
    user_permission: ['./www/sources/js/pages/user/permission/index.js'],
    user_mapping: ['./www/sources/js/pages/user/user-mapping/index.js'],
    function: ['./www/sources/js/pages/function/index.js'],
    menu: ['./www/sources/js/pages/menu/index.js'],
    role_index: ['./www/sources/js/pages/role/index.js'],
    role_permission: ['./www/sources/js/pages/role/permission/index.js']
};

module.exports = {
    projectRoot: projectRoot,
    cssPaths: cssPaths,
    scriptPaths: scriptPaths
};