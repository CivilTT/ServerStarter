<script setup lang="ts">
const a = 'gomi'

interface Props {
  value: string;
}
const props = defineProps<Props>();
</script>

<template>
  <q-page>
    <div class="row justify-center" style="min-height: inherit;">
      <q-img src="images/main_image4.png" alt=""/>
      <div class="title_box_full row justify-center items-center">
        <div class="title_box">
          <h1 class="title">
              <b>Server Starter</b> for <b>Minecraft</b>
          </h1>
          <h1 class="row justify-center">
            <span class="title_text">- start Minecraft server only <strong class="title_text_strong">ONE</strong> click ! -</span>
          </h1>
          <div class="row justify-center download_button">
            <q-btn color="white" text-color="primary" size="lg" padding="md" href="https://github.com/CivilTT/ServerStarter/releases/latest/download/Setup_ServerStarter.msi">
              <span class="download">
                Download ver 1.2.0.0
              </span>
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
.download_button{
  margin-top: 5rem;
}
.download{
  text-transform: none;
}
.title_box_full{
  height: 90vh;
  width: 100%;
  position: absolute;
}

.title_box{
  position: relative;
  top: 0;
  bottom: 0;
  padding: auto;
  margin: auto;
  color: white;
}

.title{
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
.title_text{
  font-size: 1.7rem;
  line-height: 2rem;
  font-family: 'Quicksand';
  font-weight: 400;
}
.title_text_strong{
  font-size: inherit;
}
.desc{
  padding-top: 10rem;
}
</style>

<!-- <script>
export default {
  name: 'HelloWorld'
}
</script> -->

