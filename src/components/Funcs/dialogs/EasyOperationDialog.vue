<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import BaseDialogCard from './baseDialog/BaseDialogCard.vue';

defineEmits({...useDialogPluginComponent.emitsObject})
const { dialogRef, onDialogHide, onDialogOK } = useDialogPluginComponent()
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <BaseDialogCard
      title="直感的な分かりやすさ"
      @close="onDialogOK"
    >
      <p>複雑なサーバーの仕組みや設定の難しさは一切ありません</p>
      <p>サーバーを起動するには，起動ボタンをクリックするだけです</p>
      <!-- サーバータブの画像を入れる -->
      <p>
        慣れてきて設定をアレンジしたい時に，難しいことは何もありません<br>
        詳細な設定をサポートする見やすい画面をたくさんご用意しております
      </p>
      <!-- プロパティ設定の画像を入れる -->
      <!-- プレイヤー設定の画像を入れる -->
    </BaseDialogCard>
  </q-dialog>
</template>
