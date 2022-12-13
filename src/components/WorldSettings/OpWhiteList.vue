<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>Op権限・ホワイトリストへの登録</h1>
      <h2>登録方法</h2>
      <ol>
        <li>登録するプレイヤー・グループを<router-link to="/system" class="a">システム設定</router-link>で登録しておく</li>
        <li>登録したいプレイヤーやグループを選択</li>
        <li>Op権限を登録するときには，付与する権限レベルを選択</li>
        <li>プレイヤーを追加ボタンを押す</li>
      </ol>

      <h2>プレイヤーの削除方法</h2>
      <ol>
        <li>削除したいプレイヤーをリストから左クリックで選択する</li>
        <li>ゴミ箱ボタンを押し，削除を許可する</li>
      </ol>
    </q-card-section>
  </q-card>
</template>