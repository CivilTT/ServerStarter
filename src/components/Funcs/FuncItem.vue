<script setup lang="ts">
import { Component } from 'vue';
import { useQuasar } from 'quasar';
import { funcDialogProp } from './dialogs/baseDialog/iBaseDialog';

interface Prop {
  assetPath: string
  title: string
  dialogComponent: Component | string
}
const prop = defineProps<Prop>()

const $q = useQuasar()

function openDialog() {
  $q.dialog({
    component: prop.dialogComponent,
    componentProps: {
      title: prop.title,
      assetPath: prop.assetPath
    } as funcDialogProp
  })
}
</script>

<template>
  <q-card class="card">
    <q-img :src="assetPath">
      <div class="absolute-bottom text-h6">
        {{ title }}
      </div>
    </q-img>

    <q-card-section>
      <slot />
    </q-card-section>

    <div class="absolute-center fit">
      <q-btn color="transparent" class="fit" @click="openDialog" />
    </div>
  </q-card>
</template>

<style scoped lang="scss">
.card {
  width: 100%;
  max-width: 400px;
}
</style>
