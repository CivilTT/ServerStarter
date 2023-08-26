<script setup lang="ts">
import { Component } from 'vue';
import { useQuasar } from 'quasar';

interface Prop {
  assetPath: string
  title: string
  dialogComponent: Component | string
}
const prop = defineProps<Prop>()

const $q = useQuasar()

function openDialog() {
  $q.dialog({
    component: prop.dialogComponent
  })
}
</script>

<template>
  <q-card class="card">
    <q-img src="https://cdn.quasar.dev/img/parallax2.jpg">
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