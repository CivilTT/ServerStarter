<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>サーバープロパティ</h1>
      <p>
        ServerStarterではサーバープロパティを各ワールドごとに設定することができます．
        「適用」を押すことで<router-link to="/system">システム設定</router-link>にて設定したデフォルトプロパティが反映されます
        「リセット」を押すことで<router-link to="/system">システム設定</router-link>にて設定したプロパティに戻すことができます
        設定が終わったところで「保存」を押すと設定が反映されます
      </p>
    </q-card-section>
  </q-card>
</template>