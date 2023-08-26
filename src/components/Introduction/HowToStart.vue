<script lang="ts" setup>
import { ref } from 'vue';

var step_intro = ref(1)
var step_run = ref(1)
var step_participant = ref(1)
</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ServerStarterへようこそ！</h1>
      <p>ServerStarterはMinecraftのマルチサーバーをボタンクリックによって簡単に立てられるようにするソフトウェアです</p>
      <p>ワンクリックでサーバーを起動し，マルチプレイの世界にダイブしましょう！</p>
  
      <h1>導入方法</h1>
      <q-stepper v-model="step_intro" flat vertical animated>
        <q-step :name="1" title="1. 導入" :done="step_intro > 1">
          <p>ServerStarterのインストーラーをダウンロード</p>
          <p>インストーラーを起動してServerStarterをPCにインストール</p>

          <div class="q-pb-md">
            <q-expansion-item label="ダウンロード時に警告が出た場合" header-class="bg-orange-4">
              <q-card flat class="bg-orange-2">
                <q-card-section>
                  <p>
                    ServerStarterは個人開発のため，ダウンロード数が十分な数になるまで相当の時間がかかります<br/>
                    一部のブラウザでは十分なダウンロード数のないソフトに対して警告を出すため，ソフトをダウンロードできないことがあります
                  </p>
                  <p>Edgeでは画像のように右側の「・・・」から保存を押し，次の画面で「保持する」をクリックすることで，インストーラーのダウンロードが始まります</p>
                  <p>
                    なお，ServerStarterのソースコードは
                    <a href="https://github.com/CivilTT/ServerStarter" target="_blank" class="a">GitHub</a>
                    にて公開しておりますので，そちらもご確認いただけますと幸いです
                  </p>
                  <q-img src="@/assets/Introduction/Edge_Save1.png" width="min(400px,100%)"/>
                  <q-img src="@/assets/Introduction/Edge_Save2.png" width="min(400px,100%)"/>
                </q-card-section>
              </q-card>
            </q-expansion-item>
          </div>
          
          <!-- TODO: インストーラーのダウンロードパスを変更 -->
          <q-btn size="20px" color="primary" label="インストーラーのダウンロード" href="https://github.com/CivilTT/ServerStarter/releases/latest/download/Setup_ServerStarter.msi"/>
          
          <q-stepper-navigation>
            <q-btn @click="step_intro = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>

        <q-step :name="2" title="2. 起動" :done="step_intro > 2">
          <p>デスクトップにアイコンが作成されるため，これをダブルクリックして起動</p>
          <q-img src="@/assets/Introduction/Icon.png" width="min(200px,100%)"/>
          <q-stepper-navigation>
            <q-btn @click="step_intro = 3" color="secondary" label="next" />
            <q-btn flat @click="step_intro = 1" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
        
        <q-step :name="3" title="3-1. 初期設定（利用規約）" :done="step_intro > 3">
          <p>言語設定を確認し、利用規約に同意すれば「スタート」！</p>
          <q-img src="@/assets/Introduction/WelcomeWindow.png" width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn @click="step_intro = 4" color="secondary" label="next" />
            <q-btn flat @click="step_intro = 2" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
        
        <q-step :name="4" title="3-2. 初期設定（プレイヤー登録）" :done="step_intro > 3">
          <p>Minecraftのプレイヤーアカウントを持っている場合は、ゲーム内でのプレイヤー名を登録しましょう！</p>
          <q-img src="@/assets/Introduction/OwnerPlayerSetting1.png" width="min(500px,100%)"/>
          <p>
            自身のプレイヤー名を入力すると、以下の画像のように候補が表示されます<br>
            「このプレイヤーを登録」ボタンを押し、「オーナーを登録」できるようになります</p>
          <q-img src="@/assets/Introduction/OwnerPlayerSetting2.png" width="min(500px,100%)"/>
          <q-stepper-navigation>
            <q-btn flat @click="step_intro = 3" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
      </q-stepper>

      <h1>サーバーの起動</h1>
      <q-stepper v-model="step_run" flat vertical animated>
        <q-step :name="1" title="1. 設定" :done="step_run > 1">
          <p>ワールド名を好きな名称に変更し、サーバーのバージョン指定、シングルワールドの導入設定などができます</p>
          <p>プロパティタグやプレイヤータグでもサーバーの詳細な設定を行うことができます！</p>
          <q-img src="@/assets/Introduction/MainWindow.png" width="min(900px,100%)"/>
          <q-stepper-navigation>
            <q-btn @click="step_run = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>

        <q-step :name="2" title="2. 起動" :done="step_run > 2">
          <p>準備が整ったら、「サーバータグ」を開いて、サーバーを起動しましょう！</p>
          <q-img src="@/assets/Introduction/Server1.png" width="min(700px,100%)"/>

          <p class="q-pt-md">
            サーバーが起動するとこのような画面が表示されます<br>
            初回起動時は起動前にEulaへの同意を求められます<br>
            終わったときには左下の「停止」ボタンを押してサーバーを閉じましょう！
          </p>
          <q-img src="@/assets/Introduction/Server2.png" width="min(700px,100%)"/>
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
            他の人がサーバーに参加するためにはサーバーの起動者に<a class="a" href="PortMapping">ポート開放</a>の設定が必要です
          </p>
          <p>
            サーバーの起動者でない人がサーバーに参加する際には、ServerStarterの画面右上に映っているIPアドレスを、Minecraftのサーバーアドレス欄に入力してください
          </p>
          <div class="row q-gutter-md">
            <q-img src="@/assets/Introduction/ip.png"  width="min(500px,100%)"/>
            <q-img src="@/assets/Introduction/client2.png"  width="min(500px,100%)"/>
          </div>
          <q-stepper-navigation>
            <q-btn flat @click="step_participant = 2" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
      </q-stepper>
    </q-card-section>
  </q-card>
</template>

<style scoped>
.warning {
  font-size: 1.2rem;
  font-weight: 600;
  color: red;
}
</style>