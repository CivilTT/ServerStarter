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
    <BaseDialogCard :title="title" @close="onDialogOK">
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
          <li>画面左側のワールド一覧から「ワールドを追加」を選択し，「既存のワールドを導入」を選択</li>
          <q-img src="~assets/Funcs/SinglePlay/importCustomMapBtn.png" class="q-mb-md" />
          <li>シングルプレイのワールド一覧から好きなワールドを選択するだけ！</li>
          <q-img src="~assets/Funcs/SinglePlay/singleMaps.png" />
        </ol>
      </template>
    </BaseDialogCard>
  </q-dialog>
</template>
