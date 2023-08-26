<script setup lang="ts">
import FuncItem from '@/components/Funcs/FuncItem.vue';
import MultipleServersDialog from '@/components/Funcs/dialogs/MultipleServersDialog.vue';

</script>

<template>
  <h1 class="q-pl-md">主要機能一覧</h1>

  <div class="row q-gutter-md q-pa-md justify-center">
    <FuncItem
      title="様々なサーバー"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
      :dialog-component="MultipleServersDialog"
    >
      起動可能なサーバーは全部で６種類です
    </FuncItem>
    <!-- <FuncItem
      title="個人ワールドにご招待"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      シングルプレイワールドをマルチプレイ化することができます
    </FuncItem>
    
    <FuncItem
      title="分かりやすい設定画面"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      各種設定、データパックなどの視覚的な操作
    </FuncItem>

    <FuncItem
      title="保存場所は自由自在"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      
    </FuncItem>
    
    <FuncItem
      title="プレイヤー権限設定"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
    </FuncItem>

    <FuncItem
      title="ワールドを複数人で共有"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      ShareWorldタブに飛ぶ
    </FuncItem>

    <FuncItem
      title="サーバーの顔"
      asset-path="https://cdn.quasar.dev/img/parallax2.jpg"
    >
      アイコンを自由に設定可能
    </FuncItem> -->
  </div>
</template>