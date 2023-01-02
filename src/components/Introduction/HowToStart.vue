<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ServerStarterへようこそ！</h1>
      <p>
        ServerStarterはMinecraftのマルチサーバーをボタンクリックによって簡単に立てられるようにするソフトウェアです
      </p>
      <p>
        ワンクリックでサーバーを起動し，マルチプレイの世界にダイブしましょう！
      </p>
  
      <h1>導入方法</h1>
      <q-stepper v-model="step_intro" flat vertical animated>
        <q-step :name="1" title="1. 導入" :done="step_intro > 1">
          <p>
            ServerStarterのインストーラーをダウンロード
          </p>
          <p>
            インストーラーを起動してServerStarterをPCにインストール
          </p>
          <q-btn size="20px" color="primary" label="インストーラーのダウンロード" href="https://github.com/CivilTT/ServerStarter/releases/latest/download/Setup_ServerStarter.msi"/>
          <q-stepper-navigation>
            <q-btn @click="step_intro = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>
        <q-step :name="2" title="2. 起動" :done="step_intro > 2">
          <p>
            デスクトップにアイコンが作成されるため，これをダブルクリックして起動
          </p>
          <q-img src="@/assets/Introduction/Icon.png" width="min(200px,100%)"/>
          <q-stepper-navigation>
            <q-btn @click="step_intro = 3" color="secondary" label="next" />
            <q-btn flat @click="step_intro = 1" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
        <q-step :name="3" title="3. 初期設定" :done="step_intro > 3">
          <p>
            Minecraft内でのプレイヤー名を入力し，「チェック」を押す
          </p>
          <p>
            UUIDが表示されたことを確認し，利用規約に同意すると「スタート」
          </p>
          <q-img src="@/assets/Introduction/WelcomeWindow.png" width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn flat @click="step_intro = 2" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
      </q-stepper>

      <h1>サーバーの起動</h1>
      <q-stepper v-model="step_run" flat vertical animated>
        <q-step :name="1" title="1. 設定" :done="step_run > 1">
          <p>
            起動したいサーバーのバージョンを選択し，ワールド名を設定する
          </p>
          <p>
            ワールドの細かい設定をする場合は<a class="a" href="world">ワールド設定</a>を開く
          </p>
          <p>
            準備が整ったら<b>実行</b>！
          </p>
          <q-img src="@/assets/Introduction/MainWindow.png" width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn @click="step_run = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>
        <q-step :name="2" title="2. 起動" :done="step_run > 2">
          <p>
            Javaのインストールを求められた場合は，ページ一番下の「注意点」を参照
          </p>
          <p>
            初回起動時はいくつかのウィンドウが出た後にEulaへの同意を求められる
          </p>
          <p>
            Eulaに同意すれば直ちにサーバーの黒い画面が起動する
          </p>
          <p>
            終わった際は<span class="warning">stop</span>を画面内へ入力し，<b>ウィンドウの×ボタンは使わない</b>
          </p>
          <q-img src="@/assets/Introduction/Server.png" width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn flat @click="step_run = 1" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
      </q-stepper>

      <h1>サーバーに参加</h1>
      <q-stepper v-model="step_participant" flat vertical animated>
        <q-step :name="1" title="1. Minecraftの起動" :done="step_participant > 1">
          <p>
            Minecraft Launcherを起動し，サーバーを起動したバージョンでゲームを開始する
          </p>
          <p>
            必要に応じて「起動構成」から起動したいバージョンを追加する
          </p>
          <q-img src="@/assets/Introduction/Launcher.png" width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn @click="step_participant = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>
        <q-step :name="2" title="2. マルチプレイの設定" :done="step_participant > 2">
          <p>
            起動するとシングルプレイの下にあるマルチプレイを選択し，右下の「サーバーを追加」を押す
          </p>
          <p>
            分かりやすいサーバー名を設定し，サーバーアドレスには「localhost」と記入
          </p>
          <p>
            「完了」を押せばワールドに入ることができる
          </p>
          <q-img src="@/assets/Introduction/client.png" width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn @click="step_participant = 3" color="secondary" label="next" />
            <q-btn flat @click="step_participant = 1" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
        <q-step :name="3" title="3. ほかの人がサーバーに参加する" :done="step_participant > 3">
          <p>
            他の人がサーバーに参加するためにはサーバーの起動者に<a class="a" href="AutoPortMapping">ポート開放</a>の設定が必要
          </p>
          <p>
            サーバーの起動者でない人がサーバーに参加する際にはアドレス欄にサーバー起動者のIPアドレスを入力
          </p>
          <p>
            ServerStarterではネットワークの設定に関連するものはシステム設定に
          </p>
          <q-img src="@/assets/Introduction/Network.png"  width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn flat @click="step_participant = 2" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
      </q-stepper>
    </q-card-section>
    
  </q-card>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

var step_intro = ref(1)
var step_run = ref(1)
var step_participant = ref(1)
</script>

<style scoped>
.warning {
  font-size: 1.2rem;
  font-weight: 600;
  color: red;
}
</style>