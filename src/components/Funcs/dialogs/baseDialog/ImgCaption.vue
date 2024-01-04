<script setup lang="ts">
import { onBeforeMount, ref } from 'vue';

interface Prop {
  img: string
  caption: string
}
const prop = defineProps<Prop>()

const gotImg = ref('')

async function loadImg() {
  const baseURL = window.location.origin + import.meta.env.BASE_URL
  gotImg.value = (new URL(prop.img, baseURL)).href
}

onBeforeMount(loadImg)
</script>

<template>
  <q-img :src="gotImg" />
  <p class="text-caption q-mt-sm">{{ caption }}</p>
</template>
