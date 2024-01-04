<script setup lang="ts">
import { Component, onBeforeMount, ref } from 'vue';
import { useQuasar } from 'quasar';
import { funcDialogProp } from './dialogs/baseDialog/iBaseDialog';

interface Prop {
  assetPath: string
  title: string
  dialogComponent: Component | string
}
const prop = defineProps<Prop>()

const $q = useQuasar()
const gotImg = ref('')

function openDialog() {
  $q.dialog({
    component: prop.dialogComponent,
    componentProps: {
      title: prop.title,
      assetPath: prop.assetPath
    } as funcDialogProp
  })
}

async function loadImg() {
  const baseURL = window.location.origin + import.meta.env.BASE_URL
  gotImg.value = (new URL(prop.assetPath, baseURL)).href
}

onBeforeMount(loadImg)
</script>

<template>
  <q-card class="card">
    <q-img :src="gotImg">
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
