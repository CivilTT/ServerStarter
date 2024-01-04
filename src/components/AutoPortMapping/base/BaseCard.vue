<script setup lang="ts">
import SsImg from 'src/components/utils/SsImg.vue';

interface Prop {
  title: string
  img: string
  imgWidth: string
}
defineProps<Prop>()
</script>

<template>
  <q-card-section horizontal class="full-height">
    <q-card-section>
      <div class="q-mt-sm q-mb-lg"><u style="font-size: 1.5rem;">{{ title }}</u></div>
      <div class="text-caption">
        <slot />
      </div>
    </q-card-section>

    <q-space />

    <div class="row full-height items-center" >
      <SsImg :path="img" :width="imgWidth" />
    </div>
  </q-card-section>
</template>
