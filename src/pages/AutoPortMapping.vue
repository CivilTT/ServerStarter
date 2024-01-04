<script lang="ts" setup>
import { ref } from 'vue';
import What from 'src/components/AutoPortMapping/WhatView.vue';
import How from 'src/components/AutoPortMapping/HowView.vue';
import Settings from 'src/components/AutoPortMapping/SettingsView.vue';
import APM from 'src/components/AutoPortMapping/APMView.vue';

const tab_index = ref('apm')
</script>

<template>
  <What />
  <How />

  <div class="q-px-md">
    <h1>設定項目</h1>
    <p>
      ServerStarter2ではポート開放を行わずに様々な人を招待することのできる機能を搭載しています！<br>
      本機能を使う場合と，ご自身でポート開放を行う場合の両方のやり方を記載しておりますので，お好みの方法でご準備ください！
    </p>
  </div>
  <q-card class="q-ma-md">
    <q-card-section class="fit">
      <q-tabs
        v-model="tab_index"
        dense
        active-color="primary"
        indicator-color="primary"
        align="justify"
      >
        <q-tab no-caps name="apm" label="ポート開放不要化機能を使う" />
        <q-tab no-caps name="old" label="自力でポート開放を行う" />
      </q-tabs>

      <q-separator />

      <q-tab-panels v-model="tab_index" animated>
        <q-tab-panel name="apm">
          <APM/>
        </q-tab-panel>
        <q-tab-panel name="old">
          <Settings />
        </q-tab-panel>
      </q-tab-panels>
    </q-card-section>
  </q-card>
</template>
