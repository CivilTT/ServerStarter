<script setup lang="ts">
import SsImg from 'src/components/utils/SsImg.vue';

interface Prop {
  img: string;
  caption: string;
}
defineProps<Prop>();
</script>

<template>
  <SsImg :path="img" />
  <p class="text-caption q-mt-sm">{{ caption }}</p>
</template>
