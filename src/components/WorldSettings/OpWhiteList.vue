<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>Op権限・ホワイトリストへの登録</h1>
      <h2>登録方法</h2>
      <ol>
        <li>登録するプレイヤー・グループを<router-link to="/system" class="a">システム設定</router-link>で登録しておく</li>
        <li>登録したいプレイヤーやグループを選択</li>
        <li>Op権限を登録するときには，付与する権限レベルを選択</li>
        <li>プレイヤー追加ボタンを押す</li>
      </ol>

      <h2>プレイヤーの削除方法</h2>
      <ol>
        <li>削除したいプレイヤーを一覧から選択する</li>
        <li>ゴミ箱ボタンを押し，削除する</li>
      </ol>

      <q-img src="~assets/WorldSettings/Op.png" :width="`min(100%,500px)`" />
    </q-card-section>
  </q-card>
</template>