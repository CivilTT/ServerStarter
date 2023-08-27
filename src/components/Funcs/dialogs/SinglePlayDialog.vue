<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import BaseDialogCard from './baseDialog/BaseDialogCard.vue';

defineEmits({...useDialogPluginComponent.emitsObject})
const { dialogRef, onDialogHide, onDialogOK } = useDialogPluginComponent()
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <BaseDialogCard
      title="個人ワールドをみんなで遊ぶ"
      @close="onDialogOK"
    >
      <ul>
        <li>一人で遊んでいたワールドをみんなとシェアしたい</li>
        <li>ワールド製作をみんなに協力してもらいたい</li>
      </ul>

      <p>
        ServerStarterでは個人ワールドを簡単にマルチプレイ化する機能を搭載！<br>
        ワールドの共有に難しい操作は必要ありません！
      </p>

      <template #install>
        <ol>
          <li>ホームタブにある「既存ワールドを選択」ボタンをクリック</li>
          <!-- 操作画面の画像を入れる -->
          <li>シングルプレイのワールド一覧から好きなワールドを選択するだけ！</li>
          <!-- 操作画面の画像を入れる -->
        </ol>
      </template>
    </BaseDialogCard>
  </q-dialog>
</template>
