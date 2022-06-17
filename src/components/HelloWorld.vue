<script setup lang="ts">
const a = 'gomi'

interface Props {
  value: string;
}
const props = defineProps<Props>();
</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <img alt="Quasar logo" src="../assets/logo.svg" style="width: 200px; height: 200px">
      <p>{{a}}</p>
      <p>{{value}}</p>
    </q-card-section>
  </q-card>
</template>

<style>
</style>

<!-- <script>
export default {
  name: 'HelloWorld'
}
</script> -->

