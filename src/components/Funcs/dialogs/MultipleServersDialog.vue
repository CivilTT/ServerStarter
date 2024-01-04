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
      <p>ServerStarterでは6種類のサーバーを起動することができます</p>
      <ul>
        <li>バニラ（Minecraftの公式サーバー）</li>
        <li>Spigot</li>
        <li>PaperMC</li>
        <li>Fabric</li>
        <li>Forge</li>
        <li>MohistMC</li>
      </ul>

      <p>
        サーバーの種類を後から変更することも可能！<br>
        様々な遊び方に柔軟に対応できます
      </p>

      <!-- サーバー選択の画像を入れる？ -->
      <q-img src="~assets/Funcs/MultipleServers/servers.png" />
    </BaseDialogCard>
  </q-dialog>
</template>
