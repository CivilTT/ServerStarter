<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>何をすればよいの？</h1>
      <p>
        ポート開放するものは主に「ルーター」と「セキュリティソフト」になります
      </p>
      <p>
        ポートはアプリケーションごとに開放すべきポートがある程度決まっており、Minecraftにおいては「25565」番を解放します
      </p>
      <p>
        ポート開放を行うことでLAN内のサーバーと通信することができます
      </p>
      <q-img src="~assets/AutoPortMapping/Abstruct2.png" width="min(800px,100%)" class="img" />
    </q-card-section>
  </q-card>
</template>

<style scoped>
.img {
  display: block;
  margin: 0 auto;
}
</style>