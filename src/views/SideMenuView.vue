<script setup lang="ts">
const menuList = [
  {
    to:"/intro",
    caption:"Introduction",
    label:"はじめに"
  },
  {
    to:"/main",
    caption:"main",
    label:"メイン画面"
  },
  {
    to:"/world",
    caption:"world",
    label:"ワールド設定"
  },
  {
    to:"/system",
    caption:"system",
    label:"システム設定"
  },
  {
    to:"/ShareWorld",
    caption:"ShareWorld",
    label:"ShareWorld"
  },
  {
    to:"/PortMapping",
    caption:"PortMapping",
    label:"ポート開放"
  },
  {
    to:"/credit",
    caption:"credit",
    label:"利用規約"
  },
]
</script>

<template>
  <q-drawer show-if-above bordered class="bg-grey-2">
    <q-item-label header>Menu</q-item-label>
    <q-list>
      <template v-for="(menuItem, index) in menuList" :key="index">
        <q-item :to="menuItem.to">
          <q-item-section avatar>
            <img :src="$route.path == menuItem.to ? 'images/menuicon_open.svg' : 'images/menuicon.svg'" />
          </q-item-section>
          <q-item-section>
            <q-item-label>{{menuItem.label}}</q-item-label>
            <q-item-label caption>{{menuItem.caption}}</q-item-label>
          </q-item-section>
        </q-item>
      </template>
    </q-list>
  </q-drawer>
</template>