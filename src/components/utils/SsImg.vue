<script setup lang="ts">
import { onBeforeMount, ref } from 'vue';

interface Prop {
  path: string;
  width?: string;
}
const prop = defineProps<Prop>();

const img = ref('');

async function loadImg() {
  const baseURL = window.location.origin + import.meta.env.BASE_URL;
  img.value = new URL(`${prop.path}`, baseURL).href;
}

onBeforeMount(loadImg);
</script>

<template>
  <q-img :src="path" :width="width">
    <slot />
  </q-img>
</template>
