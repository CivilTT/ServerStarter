<script setup lang="ts">
import FuncItem from 'src/components/Funcs/FuncItem.vue';
import MultipleServersDialog from 'src/components/Funcs/dialogs/MultipleServersDialog.vue';
import SinglePlayDialog from 'src/components/Funcs/dialogs/SinglePlayDialog.vue';

</script>

<template>
  <h1 class="q-pl-md">主要機能一覧</h1>

  <div class="row q-gutter-md q-pa-md justify-center">
    <!-- アイコンを6つ並べた画像を表示？ -->
    <FuncItem
      title="様々なサーバー"
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

    <!-- <FuncItem
      title="分かりやすい設定画面"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      各種設定、データパックなどの視覚的な操作
    </FuncItem> -->

    <!-- <FuncItem
      title="保存場所は自由自在"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >

    </FuncItem> -->

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
