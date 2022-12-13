const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  assetsDir: './',
  publicPath: '/ServerStarter',
  transpileDependencies: [
    'quasar'
  ],

  pluginOptions: {
    quasar: {
      importStrategy: 'kebab',
      rtlSupport: false
    }
  }
})
