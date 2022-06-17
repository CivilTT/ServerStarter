<template>
  <div class="about">
    <h1>This is an about page</h1>
    <h2>This is an about page</h2>
    <h1>This is an about page</h1>
    <q-input filled v-model="text" label="Filled" />
  </div>
</template>

<script lang="ts">
import { ref } from 'vue'

export default {
  setup () {
    return {
      text: ref('')
    }
  }
}
</script>
