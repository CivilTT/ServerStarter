<script setup lang="ts">
import FuncItem from 'src/components/Funcs/FuncItem.vue';
import EasyOperationDialog from 'src/components/Funcs/dialogs/EasyOperationDialog.vue';
import MultipleServersDialog from 'src/components/Funcs/dialogs/MultipleServersDialog.vue';
import PlayerDialog from 'src/components/Funcs/dialogs/PlayerDialog.vue';
import SinglePlayDialog from 'src/components/Funcs/dialogs/SinglePlayDialog.vue';
import WorldFolderDialog from 'src/components/Funcs/dialogs/WorldFolderDialog.vue';
import ShareWorldDialog from 'src/components/Funcs/dialogs/ShareWorldDialog.vue';
import ServerIconDialog from 'src/components/Funcs/dialogs/ServerIconDialog.vue';

</script>

<template>
  <h1 class="q-pl-md">主要機能一覧</h1>

  <div class="row q-gutter-md q-pa-md justify-center">
    <!-- アイコンを6つ並べた画像を表示？ -->
    <FuncItem
      title="6種類のサーバーに対応"
      asset-path="src/assets/Funcs/MultipleServers/Top.png"
      :dialog-component="MultipleServersDialog"
    >
      厳選した6種類のサーバーが様々な遊び方をサポートします
    </FuncItem>

    <FuncItem
      title="個人ワールドにご招待"
      asset-path="src/assets/Funcs/SinglePlay/Top.png"
      :dialog-component="SinglePlayDialog"
    >
      シングルプレイワールドをマルチプレイ化することができます
    </FuncItem>

    <FuncItem
      title="直感的な分かりやすさ"
      asset-path="src/assets/Funcs/EasyOperation/Top.png"
      :dialog-component="EasyOperationDialog"
    >
      各種設定、データパックなどを視覚的にわかりやすい操作で導入できるようにこだわりました
    </FuncItem>

    <FuncItem
      title="保存場所は自由自在"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
      :dialog-component="WorldFolderDialog"
    >
      保存場所の変更も「ワンクリック」ですぐに反映することができます
    </FuncItem>

    <FuncItem
      title="プレイヤー権限設定"
      asset-path="src/assets/Funcs/Player/Top.png"
      :dialog-component="PlayerDialog"
    >
      OP権限やホワイトリストを視覚的な操作で簡単に設定できます
    </FuncItem>

    <FuncItem
      title="ワールドを複数人で共有"
      asset-path="src/assets/Funcs/ShareWorld/Top.png"
      :dialog-component="ShareWorldDialog"
    >
      最新のプレイデータを誰でも起動可能に。面倒な共有作業は自動化しましょう。
    </FuncItem>

    <FuncItem
      title="サーバーの顔をセット"
      asset-path="src/assets/Funcs/IconSettings/Top.png"
      :dialog-component="ServerIconDialog"
    >
      参加者がワールドに入る際に表示されるアイコンを，自由にカスタマイズできます
    </FuncItem>
  </div>
</template>
