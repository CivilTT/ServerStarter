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

<script setup lang="ts">
import { ref } from 'vue';
import SideMenuView from 'src/pages/SideMenuView.vue';

const leftDrawerOpen = ref(false)

</script>
