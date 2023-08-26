<script setup lang="ts">
import { baseDialogProp } from './iBaseDialog';

defineProps<baseDialogProp>()
</script>

<template>
  <q-card flat class="q-py-sm">
    <q-card-section class="q-pt-xs">
      <div class="title" :class="color !== void 0 ? `text-${color}` : ''">{{ title }}</div>
    </q-card-section>

    <q-card-section class="q-pt-none">
      <slot />
    </q-card-section>
  </q-card>
</template>