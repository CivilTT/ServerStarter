<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>サーバーの起動</h1>
      <p>
        以下の画面が本システムのメイン画面です。<br />
        最初はVersionもWordも何もインストールされていないため、【new Version(World)】より、初めてのサーバーの設定をしましょう！
      </p>
      <p>
        ワールドの諸設定は「ワールド設定」からできます<br />
        準備ができたら「実行」を押してサーバーを起動しましょう！
      </p>
      <q-img src="~assets/MainWindow/main1.png" :width="`min(100%,800px)`" class="img" />
    </q-card-section>
  </q-card>
</template>

<script lang="ts" setup>
</script>

<style>
.img {
  display: block;
  margin: 0 auto;
}
</style>