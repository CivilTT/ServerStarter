<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn flat dense round @click="leftDrawerOpen = !leftDrawerOpen" aria-label="Menu" icon="menu" />

        <q-btn stretch flat href="..">
          <q-toolbar-title>
            <span class="title">
              <b>Server Starter</b> for <b>Minecraft</b>
            </span>
          </q-toolbar-title>
        </q-btn>

      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered class="bg-grey-2">
      <q-item-label header>Menu</q-item-label>
      <q-list>
        <template v-for="(menuItem, index) in menuList" :key="index">
          <q-item :to="menuItem.to"
            @click="select = index">
            <q-item-section avatar>
              <img :src="select == index ? 'images/menuicon_open.svg' : 'images/menuicon.svg'" />
            </q-item-section>
            <q-item-section>
            <q-item-label>{{menuItem.label}}</q-item-label>
            <q-item-label caption>{{menuItem.caption}}</q-item-label>
            </q-item-section>
          </q-item>
          <q-separator :key="'sep' + index"  v-if="menuItem.separator" />
        </template>
      </q-list>
    </q-drawer>
    <q-page-container>
      <router-view></router-view>
    </q-page-container>
  </q-layout>
</template>

<script>
import { ref } from 'vue'

export default {
  name: 'LayoutDefault',

  setup() {
    return {
      leftDrawerOpen: ref(false),
      select:ref(null),
      menuList:[
        {
          to:"/main",
          caption:"main",
          label:"main"
        },
        {
          to:"/world",
          caption:"world",
          label:"world"
        },
        {
          to:"/system",
          caption:"system",
          label:"system"
        },
        {
          to:"/ShareWorld",
          caption:"ShareWorld",
          label:"ShareWorld"
        },
        {
          to:"/AutoPortMapping",
          caption:"AutoPortMapping",
          label:"AutoPortMapping"
        },
        {
          to:"/credit",
          caption:"credit",
          label:"credit"
        },
      ]
    }
  }
}
</script>

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