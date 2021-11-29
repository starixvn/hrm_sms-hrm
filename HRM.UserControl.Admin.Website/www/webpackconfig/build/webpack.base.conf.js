const alias = require('../config/alias');
const pathConfig = require('../config/path');
const pluginGenerator = require('../config/plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

function generateConfig(environment) {
    const jsPath = `${pathConfig.scriptPaths.build}/${environment}`;
    const cssPath = `${pathConfig.cssPaths.build}/${environment}`;
    return {
        resolve: {
            alias: alias,
            extensions: ['.js', '.jsx']
        },
        entry: {
            /*js*/
            [`${jsPath}/lib/lang-en-US`]: pathConfig.scriptPaths.language,
            [`${jsPath}/main`]: pathConfig.scriptPaths.main,
            [`${jsPath}/pages/auth/login`]: pathConfig.scriptPaths.auth_login,
            [`${jsPath}/pages/site/index`]: pathConfig.scriptPaths.site_index,
            [`${jsPath}/pages/user/index`]: pathConfig.scriptPaths.user_index,
            [`${jsPath}/pages/user/assign-site`]: pathConfig.scriptPaths.user_assign_site,
            [`${jsPath}/pages/user/permission`]: pathConfig.scriptPaths.user_permission,
            [`${jsPath}/pages/function/index`]: pathConfig.scriptPaths.function,
            [`${jsPath}/pages/menu/index`]: pathConfig.scriptPaths.menu,
            [`${jsPath}/pages/role/index`]: pathConfig.scriptPaths.role_index,

            /*css*/
            [`${cssPath}/main`]: pathConfig.cssPaths.main,
            [`${cssPath}/error`]: pathConfig.cssPaths.error,
            [`${cssPath}/pages/auth/login`]: pathConfig.cssPaths.auth_login,
            [`${cssPath}/pages/site/index`]: pathConfig.cssPaths.site_index,
            [`${cssPath}/pages/user/index`]: pathConfig.cssPaths.user_index,
            [`${cssPath}/pages/user/assign-site`]: pathConfig.cssPaths.user_assign_site,
            [`${cssPath}/pages/user/permission`]: pathConfig.cssPaths.user_permission,
            [`${cssPath}/pages/function/index`]: pathConfig.cssPaths.function,
            [`${cssPath}/pages/menu/index`]: pathConfig.cssPaths.menu,
            [`${cssPath}/pages/role/index`]: pathConfig.cssPaths.role_index
        },
        output: {
            path: pathConfig.projectRoot,
            filename: "[name].chunk.js",
            chunkFilename: `${jsPath}/lib/[name].chunk.js`
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    loader: ["babel-loader"],
                    exclude: /node_modules/
                }
            ]
        },
        plugins: pluginGenerator(environment),
        stats: {
            assets: false,
            colors: true,
            version: false,
            hash: false,
            timings: true,
            chunks: false,
            chunkModules: false,
            children: true
        },
        optimization: {
            minimizer: [
                new UglifyJsPlugin({
                    sourceMap: true,
                    uglifyOptions: {
                        output: {
                            comments: false
                        }
                    }
                }),
                new OptimizeCssAssetsPlugin()
            ],
            splitChunks: {
                chunks: 'all',
                minSize: 30000,
                maxSize: 0,
                minChunks: 2,
                maxAsyncRequests: 5,
                maxInitialRequests: 3,
                name: false,
                cacheGroups: {
                    // disable default cache group
                    default: false,
                    vendors: false,
                    reactVendor: {
                        name: 'react-vendor',
                        test: /[\\/]node_modules[\\/](react|react-dom)[\\/]/,
                        minChunks: 2,
                        priority: -10
                    },
                    reactBootstrapVendor: {
                        name: 'react-bootstrap-vendor',
                        test: /[\\/]node_modules[\\/](react-bootstrap)[\\/]/,
                        minChunks: 2,
                        priority: -10
                    }
                }
            }
        }
    };
}

module.exports = generateConfig;