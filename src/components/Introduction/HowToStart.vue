<script lang="ts" setup>
import { onMounted, ref } from 'vue';
import DownloadBtn from 'src/components/HOME/DownloadBtn.vue';
import SsImg from '../utils/SsImg.vue';

const step_intro = ref(1)
const step_run = ref(1)
const step_participant = ref(1)
const tab_index = ref('win')

const versionName = ref('')

onMounted(async () => {
  // getting latest version name
  const jsonObj = await fetch('https://api.github.com/repos/CivilTT/ServerStarter2/releases/latest')
  versionName.value = (await jsonObj.json()).name
})
</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ServerStarterへようこそ！</h1>
      <p>ServerStarterはMinecraftのマルチサーバーをボタンクリックによって簡単に立てられるようにするソフトウェアです</p>
      <p>ワンクリックでサーバーを起動し，マルチプレイの世界にダイブしましょう！</p>

      <h1>導入方法</h1>
      <q-stepper v-model="step_intro" flat animated>
        <q-step :name="1" title="1. 導入" :done="step_intro > 1">
          <p>
            ServerStarter2のインストーラーをダウンロードしましょう<br>
            インストーラーを起動してServerStarterをPCにインストールしてください
          </p>

          <div class="q-py-md q-gutter-md">
            <q-expansion-item label="ダウンロード時に警告が出た場合" header-class="bg-orange-4">
              <q-card flat class="bg-orange-2">
                <q-card-section>
                  <p>
                    ServerStarterは個人開発のため，ダウンロード数が十分な数になるまで相当の時間がかかります<br />
                    一部のブラウザでは十分なダウンロード数のないソフトに対して警告を出すため，ソフトをダウンロードできないことがあります
                  </p>
                  <p>Edgeでは画像のように右側の「・・・」から保存を押し，次の画面で「保持する」をクリックすることで，インストーラーのダウンロードが始まります</p>
                  <p>
                    なお，ServerStarterのソースコードは
                    <a href="https://github.com/CivilTT/ServerStarter" target="_blank" class="a">GitHub</a>
                    にて公開しておりますので，そちらもご確認いただけますと幸いです
                  </p>
                  <div class="row q-gutter-md items-center">
                    <q-img src="~assets/Introduction/Edge_Save1.png" style="min-width: 15rem;" class="col fit" />
                    <q-img src="~assets/Introduction/Edge_Save2.png" style="min-width: 15rem;" class="col" />
                  </div>
                </q-card-section>
              </q-card>
            </q-expansion-item>

            <q-expansion-item label="インストール時に警告が出た場合" header-class="bg-orange-4">
              <q-card flat class="bg-orange-2">
                <q-card-section>
                  <p>
                    ServerStarter2では現在，アプリに対する署名を付けることができていません<br>
                    このため，以下のような警告がインストール時に表示されることがありますので，下記の手順に従って操作をお願いします
                  </p>

                  <q-card class="fit">
                    <q-tabs
                      v-model="tab_index"
                      dense
                      active-color="primary"
                      indicator-color="primary"
                      align="justify"
                    >
                      <q-tab no-caps name="win" label="Windows" />
                      <q-tab no-caps name="mac" label="Mac OS" />
                      <q-tab disable no-caps name="linux" label="Linux" />
                    </q-tabs>

                    <q-separator />

                    <q-tab-panels v-model="tab_index" animated>
                      <q-tab-panel name="win">
                        <ol>
                          <li>表示された画面中部にある「詳細設定」をクリック</li>
                          <ss-img path="assets/Introduction/defender1.png" style="max-width: 15rem;" />
                          <li>「実行」をクリックするとインストールが開始されます</li>
                          <ss-img path="assets/Introduction/defender2.png" style="max-width: 15rem;" />
                        </ol>
                      </q-tab-panel>

                      <q-tab-panel name="mac">
                        <ol>
                          <li>この画面はOKを押して閉じる</li>
                          <ss-img path="assets/Introduction/unopen.png" style="max-width: 15rem;" />
                          <li>「システム環境設定」＞「セキュリティとプライバシー」＞「一般」の順に開き，「このまま開く」を押すとインストールが開始されます</li>
                          <ss-img path="assets/Introduction/privacy.png" style="max-width: 15rem;" />
                        </ol>
                      </q-tab-panel>

                      <q-tab-panel name="linux">
                      </q-tab-panel>
                    </q-tab-panels>
                  </q-card>
                </q-card-section>
              </q-card>
            </q-expansion-item>
          </div>

          <div class="row q-gutter-md">
            <download-btn :version="versionName" outline :os-name="'windows'" />
            <download-btn :version="versionName" outline :os-name="'mac'" />
            <download-btn :version="versionName" outline disable :os-name="'linux'" />
          </div>

          <q-stepper-navigation>
            <q-btn @click="step_intro = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>

        <q-step :name="2" title="2. 起動" :done="step_intro > 2">
          <p>デスクトップにアイコンが作成されるため，これをダブルクリックして起動</p>
          <q-img src="~assets/Introduction/Icon.png" width="min(200px,100%)" />
          <q-stepper-navigation>
            <q-btn @click="step_intro = 3" color="secondary" label="next" />
            <q-btn flat @click="step_intro = 1" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>

        <q-step :name="3" title="3-1. 初期設定（利用規約）" :done="step_intro > 3">
          <p>言語設定を確認し、利用規約に同意すれば「スタート」！</p>
          <q-img src="~assets/Introduction/WelcomeWindow.png" width="min(500px,100%)" />
          <q-stepper-navigation>
            <q-btn @click="step_intro = 4" color="secondary" label="next" />
            <q-btn flat @click="step_intro = 2" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>

        <q-step :name="4" title="3-2. 初期設定（プレイヤー登録）" :done="step_intro > 3">
          <p>Minecraftのプレイヤーアカウントを持っている場合は、ゲーム内でのプレイヤー名を登録しましょう！</p>
          <p>
            自身のプレイヤー名を入力すると、画像のように候補が表示されます<br>
            「このプレイヤーを登録」ボタンを押し、「オーナーを登録」をクリックしましょう！
          </p>
          <div class="row q-gutter-md items-center">
            <q-img src="~assets/Introduction/OwnerPlayerSetting1.png" style="min-width: 15rem;" class="col fit" />
            <q-img src="~assets/Introduction/OwnerPlayerSetting2.png" style="min-width: 15rem;" class="col" />
          </div>
          <q-stepper-navigation>
            <q-btn flat @click="step_intro = 3" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
      </q-stepper>

      <h1>サーバーの起動</h1>
      <q-stepper v-model="step_run" flat animated>
        <q-step :name="1" title="1. 設定" :done="step_run > 1">
          <p>ワールド名を好きな名称に変更し、サーバーのバージョン指定、などができます</p>
          <p class="text-red text-bold ">「ポート開放不要化」ではご友人などがサーバーに入室するための設定を行いますのでお忘れなく！</p>
          <p>プロパティタグやプレイヤータグでも詳細な設定ができます！</p>
          <q-img src="~assets/Introduction/MainWindow.png" width="min(900px,100%)" />
          <q-stepper-navigation>
            <q-btn @click="step_run = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>

        <q-step :name="2" title="2. 起動" :done="step_run > 2">
          <p>準備が整ったら、サーバーを起動しましょう！</p>
          <p>
            サーバーが起動するとこのような画面が表示されます！<br>
            遊び終わった後には左下の「停止」ボタンを押してサーバーを閉じましょう！
          </p>
          <p>
            ※ 初回起動時は起動前にEula（利用規約）への同意を求められます
          </p>
          <q-img src="~assets/Introduction/Server2.png" width="min(700px,100%)" />
          <q-stepper-navigation>
            <q-btn flat @click="step_run = 1" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
      </q-stepper>

      <h1>サーバーに参加</h1>
      <q-stepper v-model="step_participant" flat animated>
        <q-step :name="1" title="1. Minecraftの起動" :done="step_participant > 1">
          <p>
            Minecraft Launcherを起動し，サーバーを起動したバージョンでゲームを開始してください
          </p>
          <p>
            バージョンがない時には「起動構成」から起動したいバージョンの追加が必要です！
          </p>
          <q-img src="~assets/Introduction/Launcher.png" width="min(500px,100%)" />
          <q-stepper-navigation>
            <q-btn @click="step_participant = 2" color="secondary" label="next" />
          </q-stepper-navigation>
        </q-step>
        <q-step :name="2" title="2. マルチプレイの設定" :done="step_participant > 2">
          <p>
            Minecraftが起動した後は以下の手順でご自身のサーバーを追加しましょう！
          </p>
          <ol>
            <li>画面中央の「マルチプレイ」を選択</li>
            <li>画面右下の「サーバーを追加」を選択</li>
            <li>開いた画面に対して画像のように各項目を設定</li>
          </ol>
          <p>
            「完了」を押して作成されたサーバーに入れば準備完了です！
          </p>
          <q-img src="~assets/Introduction/client.png" width="min(500px,100%)" />
          <q-stepper-navigation>
            <q-btn @click="step_participant = 3" color="secondary" label="next" />
            <q-btn flat @click="step_participant = 1" color="secondary" label="Back" class="q-ml-sm" />
          </q-stepper-navigation>
        </q-step>
        <q-step :name="3" title="3. ご友人などがサーバーに参加する" :done="step_participant > 3">
          <p>
            ご友人などがサーバーに参加する際には、
            サーバー起動時にServerStarterの画面右上に映っているIPアドレスを、Minecraftのサーバーアドレス欄に入力してください
          </p>
          <div class="row q-gutter-md">
            <q-img src="~assets/Introduction/ip.png" style="min-width: 15rem;" class="col" />
            <q-img src="~assets/Introduction/client2.png" style="min-width: 15rem;" class="col" />
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
