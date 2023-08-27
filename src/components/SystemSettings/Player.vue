<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>プレイヤー</h1>
      <p>
        Minecraftにおけるプレイヤーのユーザー名を登録しておけます<br />
        登録済みプレイヤーは、各ワールド設定にてOp権限やホワイトリストの設定を行うことができます<br />
        さらに、一括してプレイヤーをOpやホワイトリストに登録可能な「グループ」機能を利用できます
      </p>
      <q-img src="~assets/SystemSettings/PlayerList.png" width="min(500px, 100%)" />

      <h2>グループの登録方法</h2>
      <ol>
        <li>わかりやすいグループ名を「新規グループ名」に記入</li>
        <li>「プレイヤー」の中からグループに登録したいプレイヤーを選択し、「追加」を押す</li>
        <li>登録したいプレイヤーがすべて「グループメンバー」に表示されたら「登録」を押す</li>
        <li>「登録済みグループ一覧」に登録内容が表示されればOK</li>
      </ol>
      <q-img src="~assets/SystemSettings/GroupSetting.png" width="min(500px,100%)" />

      <h2>既存グループのメンバー編集方法</h2>
      <ol>
        <li>「登録済みグループ一覧」より編集したいグループを選択</li>
        <li>
          編集ボタン
          <q-img src="~assets/SystemSettings/EditButton.png" width="1.4em" />
          を押し、メンバーの登録、削除を行う
        </li>
        <li>編集が終わり次第、「登録」を押して編集結果を登録</li>
      </ol>
      <q-img src="~assets/SystemSettings/GroupList.png" width="min(500px,100%)" />

      <h2>利用方法</h2>
      <p>
        ワールド設定にてOp権限やホワイトリストを登録する際のプルダウンに登録されているため，
        必要なプレイヤーやグループを選択して追加する
      </p>
      <q-img src="~assets/SystemSettings/OpGroup.png" width="min(500px,100%)" />
    </q-card-section>
  </q-card>
</template>