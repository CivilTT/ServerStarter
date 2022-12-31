<template>
  <div class="home">
    <HowToStart/>
    <Attention/>
  </div>
</template>

<script lang="ts" setup>
import { defineComponent } from 'vue';
import HowToStart from '@/components/Introduction/HowToStart.vue';
import Attention from '@/components/Introduction/Attention.vue';
</script>
