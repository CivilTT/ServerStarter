<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import BaseDialogCard from './BaseDialogCard.vue';

defineEmits({...useDialogPluginComponent.emitsObject})
const { dialogRef, onDialogHide, onDialogOK } = useDialogPluginComponent()
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <BaseDialogCard
      title="6種類のサーバーに対応"
      @close="onDialogOK"
    >
    </BaseDialogCard>
  </q-dialog>
</template>