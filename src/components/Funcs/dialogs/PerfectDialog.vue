<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import { funcDialogProp } from './baseDialog/iBaseDialog';
import BaseDialogCard from './baseDialog/BaseDialogCard.vue';
import ImgCaption from './baseDialog/ImgCaption.vue';

defineProps<funcDialogProp>()
defineEmits({...useDialogPluginComponent.emitsObject})
const { dialogRef, onDialogHide, onDialogOK } = useDialogPluginComponent()
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <BaseDialogCard
      :title="title"
      sec-title="ギャラリー"
      @close="onDialogOK"
    >
      <p>ServerStarter2ではマルチプレイに対するあらゆるサポートを行います</p>
      <ul>
        <li>様々な種類のサーバーを起動</li>
        <li>ポート開放不要化</li>
        <li>プレイヤー管理</li>
        <li>データパックやMODの導入</li>
        <p class="text-right">このほかにも様々な機能を備えています！</p>
      </ul>
      <p>マルチプレイの必携ツールであるServerStarter2をぜひご利用ください！</p>

      <template #install>
        <div class="q-gutter-md q-pa-md">
          <ImgCaption
            img="assets/Funcs/Perfect/main.png"
            caption="メイン画面ではサーバーを開くために必要な最低限の設定項目を指定できます"
          />
          <ImgCaption
            img="assets/Funcs/Perfect/consoleRunning.png"
            caption="サーバーの起動中は見やすくなったコンソール画面の下にはサーバーの停止や再起動ができるボタンを配置しました"
          />
          <ImgCaption
            img="assets/Funcs/Perfect/property.png"
            caption="サーバーの細かい設定も充実した検索機能を使えば簡単に行うことができます"
          />
          <ImgCaption
            img="assets/Funcs/Perfect/player.png"
            caption="参加者の権限やホワイトリストの設定も直感的な操作を実現しました"
          />
          <ImgCaption
            img="assets/Funcs/Perfect/contents.png"
            caption="データパック・プラグイン・MODの全てを簡単に導入したり外したりすることができます"
          />
          <ImgCaption
            img="assets/Funcs/Perfect/others.png"
            caption="普段は操作しないような細かい設定も簡単にできるように配慮しました"
          />
        </div>
      </template>
    </BaseDialogCard>
  </q-dialog>
</template>
