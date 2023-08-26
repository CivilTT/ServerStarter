<script setup lang="ts">
import { ref } from 'vue';
import SideMenuView from './views/SideMenuView.vue'

const leftDrawerOpen = ref(false)
</script>

<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn flat dense round @click="leftDrawerOpen = !leftDrawerOpen" aria-label="Menu" icon="menu" />

        <q-btn stretch flat to="/">
          <q-toolbar-title>
            <span class="title">
              <b>Server Starter</b> for <b>Minecraft</b>
            </span>
          </q-toolbar-title>
        </q-btn>
      </q-toolbar>
    </q-header>

    <SideMenuView v-model="leftDrawerOpen" />

    <q-page-container>
      <router-view></router-view>
    </q-page-container>
  </q-layout>
</template>

<style scoped lang="scss">
.title{
  text-transform: none;
  font-size: 1.2rem;
  line-height: 1rem;
  font-family: 'Quicksand';
  font-weight: 400;
  
  b {
    font-size: 1.5rem;
    font-family: 'PT Sans', sans-serif;
    font-weight: 900;
  }
}
</style>