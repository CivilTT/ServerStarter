<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated class="glossy">
      <q-toolbar>
        <q-btn flat dense round @click="leftDrawerOpen = !leftDrawerOpen" aria-label="Menu" icon="menu" />

        <q-toolbar-title>
          Quasar App
        </q-toolbar-title>

        <div>Quasar v{{ $q.version }}</div>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered class="bg-grey-2">
      <q-list>
        <q-item-label header>Essential Links</q-item-label>

        <q-item to="/main">
          <q-item-section avatar>
            <q-icon name="school" />
          </q-item-section>
          <q-item-section>
            <q-item-label>main</q-item-label>
            <q-item-label caption>main</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/world">
          <q-item-section avatar>
            <q-icon name="school" />
          </q-item-section>
          <q-item-section>
            <q-item-label>world</q-item-label>
            <q-item-label caption>world</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/system">
          <q-item-section avatar>
            <q-icon name="school" />
          </q-item-section>
          <q-item-section>
            <q-item-label>system</q-item-label>
            <q-item-label caption>system</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/ShareWorld">
          <q-item-section avatar>
            <q-icon name="school" />
          </q-item-section>
          <q-item-section>
            <q-item-label>ShareWorld</q-item-label>
            <q-item-label caption>ShareWorld</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/AutoPortMapping">
          <q-item-section avatar>
            <q-icon name="school" />
          </q-item-section>
          <q-item-section>
            <q-item-label>AutoPortMapping</q-item-label>
            <q-item-label caption>AutoPortMapping</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/credit">
          <q-item-section avatar>
            <q-icon name="school" />
          </q-item-section>
          <q-item-section>
            <q-item-label>credit</q-item-label>
            <q-item-label caption>credit</q-item-label>
          </q-item-section>
        </q-item>

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
      leftDrawerOpen: ref(false)
    }
  }
}
</script>
