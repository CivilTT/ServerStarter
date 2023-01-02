<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>サーバープロパティ</h1>
      <p>
        各設定項目をプルダウンや表形式で設定することができます
      </p>
      <p>
        プロパティの設定では次の操作をサポートしています
      </p>
      <ul>
        <li>「適用」を押すことでこのワールド設定が今後の新規ワールドにおけるデフォルト設定になります</li>
        <li>「リセット」を押すことで<router-link to="/system" class="a">システム設定</router-link>にて設定したデフォルトに戻すことができます</li>
      </ul>
    </q-card-section>
  </q-card>
</template>