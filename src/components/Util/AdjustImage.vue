<script setup lang="ts">
interface Props {
  src: string;
  max_width:string;
}
const props = defineProps<Props>();
</script>

<template>
  <q-img src="@/assets/MainWindow/main1.png" :width="`min(100%,${props.max_width})`" class="img"/>
</template>

<style lang="scss" scoped>
.img {
  display: block;
  margin : 0 auto;
}
</style>
