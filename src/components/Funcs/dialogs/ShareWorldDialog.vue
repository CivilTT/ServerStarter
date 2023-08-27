<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import { funcDialogProp } from './baseDialog/iBaseDialog';
import BaseDialogCard from './baseDialog/BaseDialogCard.vue';

defineProps<funcDialogProp>()
defineEmits({ ...useDialogPluginComponent.emitsObject })
const { dialogRef, onDialogHide, onDialogOK } = useDialogPluginComponent()
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <BaseDialogCard :title="title" @close="onDialogOK">
      <p>
        個人でサーバーを立てて友人と遊ぶ際に，サーバーを立てた人がいないと友人だけではマルチプレイワールドに入れませんでした．<br>
        データを共有するにも，遊び終わった後に共有作業をするのは非常に面倒です
      </p>
      <p>
        ServerStarterのShareWorldを使えば，簡単にサーバーデータを複数人で共有し，
        共有相手はいつでも最新のワールドを立ち上げることができるようになります！
      </p>
      <p>
        プレイヤー間で自由にサーバーを立ち上げて，いつでも最新のワールドをプレイしましょう！
      </p>
      <p>
        事前準備や使い方の詳細は，
        <router-link class="a" to="/ShareWorld">ShareWorldの解説記事</router-link>
        をご覧ください！
      </p>
      <!-- 画面のスクショを入れる -->
      <div class="column q-gutter-md q-mr-md">
        <q-img src="~assets/Funcs/ShareWorld/BeforeSetting.png" />
        <q-img src="~assets/Funcs/ShareWorld/AfterSetting.png" />
      </div>
    </BaseDialogCard>
  </q-dialog>
</template>
