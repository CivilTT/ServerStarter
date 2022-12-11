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
            <span>
              <b>Server Starter</b> for <b>Minecraft</b>
            </span>
          </h1>
          <div class="row justify-center">
            <span class="title_text">- start Minecraft server only <strong>ONE</strong> click ! -</span>
          </div>
        </div>
      </div>
    </div>
    <!-- <div class="row justify-center desc">
      <p>ServerStarterはJava版マインクラフトのサーバーのインストールと起動をサポートするwindowsアプリケーションです。</p>
    </div> -->
</q-page>
</template>

<style lang="scss">
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
.desc{
  padding-top: 10rem;
}
</style>

<!-- <script>
export default {
  name: 'HelloWorld'
}
</script> -->

