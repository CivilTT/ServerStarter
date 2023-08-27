<script setup lang="ts">
import { onMounted, ref } from 'vue';

interface Props {
  value: string;
}
defineProps<Props>();

const versionName = ref('')

onMounted(async () => {
  // getting latest version name
  const jsonObj = await fetch('https://api.github.com/repos/CivilTT/ServerStarter/releases/latest')
  versionName.value = (await jsonObj.json()).name.split(' ')[1]
})
</script>

<template>
  <q-page style="background-color: black;">
    <div class="row justify-center" style="min-height: inherit;">
      <q-img src="~assets/titleImg.png" style="opacity: .6;" />
      <div class="title_box_full row justify-center items-center">
        <div class="title_box">
          <h1 class="title">
            <b>Server Starter</b> for <b>Minecraft</b>
          </h1>
          <h1 class="row justify-center">
            <span class="title_text">- start Minecraft server only <strong class="title_text_strong">ONE</strong> click !
              -</span>
          </h1>

          <!-- <p class="row justify-center download_button text-bold">お使いのプラットフォームに合わせてダウンロードしてください（バージョン：{{versionName}}）</p> -->
          <p class="row justify-center download_button text-bold text-yellow">2023年9月2日 15：00に公開予定！！</p>
          <div class="row q-gutter-md justify-center">
            <q-btn disable color="white" text-color="primary" padding="md"
              href="https://github.com/CivilTT/ServerStarter/releases/latest/download/Setup_ServerStarter.msi"
              style="width: 10rem;">
              <div class="row items-center">
                <q-avatar size="2rem" square class="q-mr-md">
                  <q-img src="~assets/OS/windows.svg" />
                </q-avatar>
                <div class="download">
                  Windows
                </div>
              </div>
            </q-btn>

            <q-btn disable color="white" text-color="primary" padding="md"
              href="https://github.com/CivilTT/ServerStarter/releases/latest/download/Setup_ServerStarter.msi"
              style="width: 10rem;">
              <div class="row items-center">
                <q-avatar size="2rem" square class="q-mr-md">
                  <q-img src="~assets/OS/mac.svg" />
                </q-avatar>
                <div class="download">
                  Mac OS
                </div>
              </div>
            </q-btn>

            <q-btn disable color="white" text-color="primary" padding="md"
              href="https://github.com/CivilTT/ServerStarter/releases/latest/download/Setup_ServerStarter.msi"
              style="width: 10rem;">
              <div class="row items-center">
                <q-avatar size="2rem" square class="q-mr-md">
                  <q-img src="~assets/OS/linux.svg" />
                </q-avatar>
                <div class="download">
                  Linux
                </div>
              </div>
            </q-btn>
          </div>
        </div>
      </div>
    </div>
    <!-- <div class="row justify-center desc">
      <p>ServerStarterはJava版マインクラフトのサーバーのインストールと起動をサポートするwindowsアプリケーションです。</p>
    </div> -->
  </q-page>
</template>

<style lang="scss" scoped>
.download_button {
  margin-top: 5rem;
}

.download {
  text-transform: none;
}

.title_box_full {
  height: 90vh;
  width: 100%;
  position: absolute;
}

.title_box {
  position: relative;
  top: 0;
  bottom: 0;
  padding: auto;
  margin: auto;
  color: white;
}

.title {
  font-size: 3rem;
  line-height: 3rem;
  font-family: 'Quicksand';
  font-weight: 400;

  b {
    font-size: 4rem;
    font-family: 'PT Sans', sans-serif;
    font-weight: 900;
  }
}

.title_text {
  font-size: 1.7rem;
  line-height: 2rem;
  font-family: 'Quicksand';
  font-weight: 400;
}

.title_text_strong {
  font-size: inherit;
}

.desc {
  padding-top: 10rem;
}
</style>