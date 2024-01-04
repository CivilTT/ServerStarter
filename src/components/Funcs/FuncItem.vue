<script setup lang="ts">
import { Component } from 'vue';
import { useQuasar } from 'quasar';
import { funcDialogProp } from './dialogs/baseDialog/iBaseDialog';
import SsImg from '../utils/SsImg.vue';

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
    <ss-img :path="assetPath" style="aspect-ratio: 16/9;">
      <div class="absolute-bottom text-h6">
        {{ title }}
      </div>
    </ss-img>

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
