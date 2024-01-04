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
        <li>ワールドの保存場所がわからない</li>
        <li>直接編集するために見やすい場所に保存しておきたい</li>
      </ul>

      <p>ServerStarterでは保存場所の変更も「ワンクリック」で行うことができます！</p>
      <p>
        保存場所ごとに好きな名前をつけたり，表示非表示を切り替えたりできます<br>
        ワールドの管理をより簡単にする機能です！
      </p>

      <template #install>
        <ol>
          <li>「その他の設定」タブの「ワールドフォルダ」を開く</li>
          <q-img src="~assets/Funcs/WorldFolder/BeforeAdding.png" class="q-mb-md" />

          <li>「ワールドフォルダを追加」ボタンをクリックし，好きな名前の設定と，設定するフォルダを選択</li>
          <q-img src="~assets/Funcs/WorldFolder/EditDialog.png" class="q-mb-md" />

          <li>追加されたアイテムを選択することで，保存場所を決めることができます</li>
          <q-img src="~assets/Funcs/WorldFolder/AfterAdding.png" />
        </ol>
      </template>
    </BaseDialogCard>
  </q-dialog>
</template>
