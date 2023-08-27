<script setup lang="ts">
</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>追加要素</h1>
      <p>
        データパックやPlugin，配布ワールド（Custom Map）の導入を行います
      </p>
      <h2>導入方法</h2>
      <ol>
        <li>「データパックの種類」で導入するファイルの種類（フォルダ or Zip）を選択する</li>
        <li>インポートを押し，導入する ファイル or Zip を選択する</li>
        <li>導入済み一覧に追記されたことを確認する</li>
      </ol>

      <h2>注記</h2>
      <ul>
        <li>配布ワールドについては新規ワールド（【new World】）のみ導入可能になります</li>
        <li>プラグインについてはSpigotサーバーのみ導入可能になります</li>
      </ul>
      <q-img src="~assets/WorldSettings/Additional.png" :width="`min(100%,500px)`" />
    </q-card-section>
  </q-card>
</template>