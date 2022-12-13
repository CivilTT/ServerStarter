<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>サーバーの起動</h1>
      <p>
        以下の画面が本システムのメイン画面です。
        Versionにて起動するサーバーのバージョンを指定し、Worldにて起動するワールドを選択します。
        最初はVersionもWordも何もインストールされていないため、【new Version(World)】より、インストールしたいバージョンを選択し、起動するワールドの名前を決めてください。
  
        ワールドの諸設定は「ワールド設定」から行ってください．
        準備ができたら「実行」を押してサーバーを起動しましょう！
      </p>
      <AdjustImage src="/images/MainWindow/main1.png" max_width="800px"/>
    </q-card-section>
  </q-card>
</template>

<script lang="ts" setup>
import AdjustImage from '@/components/Util/AdjustImage.vue';
</script>
