<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>システム設定</h1>
      <p>
        ServerStarter全体の設定や，ワールド起動時の共通設定，デフォルト設定を定義します<br />
        設定が終わったところで<b>「保存」</b>を押すと諸設定が反映されます
      </p>
      <p>
        ※ShareWorldについては左側Menuの「ShareWorld」タブをご覧ください<br />
        ※ネットワークについては左側Menuの「自動ポート開放」タブをご覧ください
      </p>
      <q-img src="~assets/SystemSettings/SystemSettings.png" width="min(800px,100%)" class="img" />
    </q-card-section>
  </q-card>
</template>

<style scoped>
.img {
  display: block;
  margin: 0 auto;
}
</style>