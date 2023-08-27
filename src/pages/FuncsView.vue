<script setup lang="ts">
import FuncItem from 'src/components/Funcs/FuncItem.vue';
import EasyOperationDialog from 'src/components/Funcs/dialogs/EasyOperationDialog.vue';
import MultipleServersDialog from 'src/components/Funcs/dialogs/MultipleServersDialog.vue';
import SinglePlayDialog from 'src/components/Funcs/dialogs/SinglePlayDialog.vue';
import WorldFolderDialog from 'src/components/Funcs/dialogs/WorldFolderDialog.vue';

</script>

<template>
  <h1 class="q-pl-md">主要機能一覧</h1>

  <div class="row q-gutter-md q-pa-md justify-center">
    <!-- アイコンを6つ並べた画像を表示？ -->
    <FuncItem
      title="6種類のサーバーに対応"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
      :dialog-component="MultipleServersDialog"
    >
      厳選した6種類のサーバーが様々な遊び方をサポートします
    </FuncItem>

    <FuncItem
      title="個人ワールドにご招待"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
      :dialog-component="SinglePlayDialog"
    >
      シングルプレイワールドをマルチプレイ化することができます
    </FuncItem>

    <FuncItem
      title="直感的な分かりやすさ"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
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

    <!-- <FuncItem
      title="プレイヤー権限設定"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
    </FuncItem> -->

    <!-- <FuncItem
      title="ワールドを複数人で共有"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      ShareWorldタブに飛ぶ
    </FuncItem> -->

    <!-- <FuncItem
      title="サーバーの顔"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      アイコンを自由に設定可能
    </FuncItem> -->
  </div>
</template>
