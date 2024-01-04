<script setup lang="ts">
interface baseDialogProp {
  title: string
  secTitle?: string
  noSec?: boolean
  color?: string
  onClose?: () => void
}
defineProps<baseDialogProp>()
</script>

<template>
  <q-card flat class="q-py-sm">
    <!-- TODO: 画像をタイトルにかぶせる形で表示する？ -->
    <q-card-section class="q-pt-xs">
      <div class="title" :class="color !== void 0 ? `text-${color}` : ''">{{ title }}</div>
    </q-card-section>

    <q-card-section class="q-pt-none">
      <slot />
    </q-card-section>
    <q-card-section v-if="!noSec" class="q-pt-none">
      <span>■{{ secTitle ? secTitle : 'やり方' }}</span>
      <slot name="install"/>
    </q-card-section>
  </q-card>
</template>

<style scoped lang="scss">
.title {
  font-size: 1.2rem;
  padding-right: 2.5rem;
}
</style>
