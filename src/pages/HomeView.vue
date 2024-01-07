<script setup lang="ts">
import { onMounted, ref } from 'vue';
import DownloadBtn from 'src/components/HOME/DownloadBtn.vue';

interface Props {
  value: string;
}
defineProps<Props>();

const versionName = ref('')

onMounted(async () => {
  // getting latest version name
  const jsonObj = await fetch('https://api.github.com/repos/CivilTT/ServerStarter2/releases/latest')
  versionName.value = (await jsonObj.json()).name
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
            <span class="title_text">
              - Start Minecraft Java edition server only <strong class="title_text_strong">ONE</strong> click ! -
            </span>
          </h1>

          <p class="row justify-center download_button text-bold">
            お使いのプラットフォームに合わせてダウンロードしてください（バージョン：{{versionName}}）
          </p>
          <div class="row q-gutter-md justify-center">
            <download-btn :version="versionName" :os-name="'windows'"/>
            <download-btn :version="versionName" :os-name="'mac'"/>
            <download-btn :version="versionName" disable :os-name="'linux'"/>
          </div>
          <p class="row justify-center text-bold text-yellow">
            Linux版は年度末ごろの長期リリース版に合わせて公開します！
          </p>
        </div>
      </div>
    </div>
  </q-page>
</template>

<style lang="scss" scoped>
.download_button {
  margin-top: 5rem;
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
