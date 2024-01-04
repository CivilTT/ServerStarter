<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import { funcDialogProp } from './baseDialog/iBaseDialog';
import BaseDialogCard from './baseDialog/BaseDialogCard.vue';

defineProps<funcDialogProp>()
defineEmits({ ...useDialogPluginComponent.emitsObject })
const { dialogRef, onDialogHide, onDialogOK } = useDialogPluginComponent()
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <BaseDialogCard :title="title" no-sec @close="onDialogOK">
      <p>複雑なサーバーの仕組みや設定の難しさは一切ありません</p>
      <p>サーバーを起動するには，起動ボタンをクリックするだけです</p>
      <q-img src="~assets/Funcs/EasyOperation/ServerBootBtn.png" class="q-mb-md" />

      <p>
        慣れてきて設定をアレンジしたい時に，難しいことは何もありません<br>
        詳細な設定をサポートする見やすい画面をたくさんご用意しております
      </p>
      <div class="column q-gutter-md q-pr-md">
        <q-img src="~assets/Funcs/EasyOperation/Property.png" />
        <q-img src="~assets/Funcs/EasyOperation/Players.png" />
      </div>
    </BaseDialogCard>
  </q-dialog>
</template>
