<script setup lang="ts">
import FuncItem from 'src/components/Funcs/FuncItem.vue';
import EasyOperationDialog from 'src/components/Funcs/dialogs/EasyOperationDialog.vue';
import MultipleServersDialog from 'src/components/Funcs/dialogs/MultipleServersDialog.vue';
import PlayerDialog from 'src/components/Funcs/dialogs/PlayerDialog.vue';
import SinglePlayDialog from 'src/components/Funcs/dialogs/SinglePlayDialog.vue';
import WorldFolderDialog from 'src/components/Funcs/dialogs/WorldFolderDialog.vue';
import ShareWorldDialog from 'src/components/Funcs/dialogs/ShareWorldDialog.vue';
import ServerIconDialog from 'src/components/Funcs/dialogs/ServerIconDialog.vue';
import PerfectDialog from 'src/components/Funcs/dialogs/PerfectDialog.vue';

</script>

<template>
  <h1 class="q-pl-md">主要機能一覧</h1>

  <div class="row q-gutter-md q-pa-md justify-center">
    <FuncItem
      title="全ての準備が完了します"
      asset-path="assets/Funcs/Top/Perfect.png"
      :dialog-component="PerfectDialog"
    >
      サーバーの準備からポート解放まで，全ての準備はServerStarter2がサポートします
    </FuncItem>

    <FuncItem
      title="6種類のサーバーに対応"
      asset-path="assets/Funcs/Top/MultipleServers.png"
      :dialog-component="MultipleServersDialog"
    >
      厳選した6種類のサーバーが様々な遊び方をサポートします
    </FuncItem>

    <FuncItem
      title="個人ワールドにご招待"
      asset-path="assets/Funcs/Top/SinglePlay.png"
      :dialog-component="SinglePlayDialog"
    >
      シングルプレイワールドをマルチプレイ化することができます
    </FuncItem>

    <FuncItem
      title="直感的な分かりやすさ"
      asset-path="assets/Funcs/Top/EasyOperation.png"
      :dialog-component="EasyOperationDialog"
    >
      各種設定、データパックなどを視覚的にわかりやすい操作で導入できるようにこだわりました
    </FuncItem>

    <FuncItem
      title="保存場所は自由自在"
      asset-path="assets/Funcs/Top/bridge.jpg"
      :dialog-component="WorldFolderDialog"
    >
      保存場所の変更も「ワンクリック」ですぐに反映することができます
    </FuncItem>

    <FuncItem
      title="プレイヤー権限設定"
      asset-path="assets/Funcs/Top/Player.png"
      :dialog-component="PlayerDialog"
    >
      OP権限やホワイトリストを視覚的な操作で簡単に設定できます
    </FuncItem>

    <!-- <FuncItem
      title="ワールドを複数人で共有"
      asset-path="assets/Funcs/Top/ShareWorld.png"
      :dialog-component="ShareWorldDialog"
    >
      最新のプレイデータを誰でも起動可能に。面倒な共有作業は自動化しましょう。
    </FuncItem> -->

    <FuncItem
      title="サーバーの顔をセット"
      asset-path="assets/Funcs/Top/IconSettings.png"
      :dialog-component="ServerIconDialog"
    >
      参加者がワールドに入る際に表示されるアイコンを，自由にカスタマイズできます
    </FuncItem>
  </div>
</template>
