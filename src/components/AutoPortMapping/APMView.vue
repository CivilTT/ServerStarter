<script setup lang="ts">
import { ref } from 'vue';
import BaseCard from './base/BaseCard.vue'

const slide = ref(1)
</script>

<template>
  <p>
    ServerStarter2ではポート開放不要化を実現するために
    <a href="https://ngrok.com/" class="text-primary">Ngrok</a>を利用します<br>
    下記の手順に従って，アカウントの登録と設定を完了しましょう！
  </p>

  <q-carousel
    v-model="slide"
    animated
    transition-prev="slide-right"
    transition-next="slide-left"
    control-color="primary"
    navigation
    padding
    arrows
    class="shadow-1 rounded-borders"
  >
    <template v-slot:navigation-icon="{ active, btnProps, onClick }">
      <q-btn v-if="active" :icon="btnProps.icon" color="primary" flat round dense @click="onClick" />
      <q-btn v-else :icon="btnProps.icon" flat round dense @click="onClick" style="opacity: .2;" />
    </template>

    <q-carousel-slide :name="1">
      <BaseCard
        title="アカウント登録"
        img="assets/Ngrok/signUp.png"
        img-width="9rem"
      >
        <p>
          Ngrokのアカウント登録は
          <a target="_blank" href="https://dashboard.ngrok.com/signup" class="text-primary">
            こちらの登録画面で行います
          </a>
        </p>
        <p>
          任意のユーザー名，メールアドレス，パスワードを記入して
          Sign upをクリックしましょう！
        </p>
      </BaseCard>
    </q-carousel-slide>
    <q-carousel-slide :name="2">
      <BaseCard
        title="Eメールの認証"
        img="assets/Ngrok/verifyEmail.png"
        img-width="20rem"
      >
        <p>
          登録したE-mailにNgrokよりメールが届くため，
          メールに記載されたURLをクリックして認証を完了しましょう
        </p>
      </BaseCard>
    </q-carousel-slide>
    <q-carousel-slide :name="3">
      <BaseCard
        title="アンケートへの回答"
        img="assets/Ngrok/welcome.png"
        img-width="14rem"
      >
        <p>
          これで登録は完了ですが，最後にNgrokからのアンケートを画像に倣って回答をお願いします
        </p>
      </BaseCard>
    </q-carousel-slide>
    <q-carousel-slide :name="4">
      <BaseCard
        title="接続方法"
        img="assets/Ngrok/mcInput.png"
        img-width="14rem"
      >
        <p>
          ServerStarter2でサーバーを起動すると画面の右上に
          <b><u>起動ごとに値が変更される</u></b>IPアドレスが表示されます
        </p>
        <p>
          このアドレスをMinecraftのマルチプレイサーバーのアドレス欄に入力することで，
          サーバーに接続することができます！
        </p>
      </BaseCard>
    </q-carousel-slide>
  </q-carousel>
</template>
