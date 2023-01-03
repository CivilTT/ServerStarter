<script lang="ts" setup>
const columns = [
  {
    name: 'index',
    label: '設定項目名',
    field: 'index',
    style: 'font-size: 1rem'
  },
  {
    name: 'value',
    label: '設定値',
    field: 'value',
    style: 'font-size: 1rem'
  }
]

const tokenRows = [
  {
    index: 'Note',
    value: '適当に分かりやすい名称を付ける'
  },
  {
    index: 'Expiration',
    value: '発行するTokenの有効期限を決める'
  },
  {
    index: 'Select scopes',
    value: '「repo」にチェックを付ける'
  },
]

const credentialRows = [
  {
    index: 'アドレス',
    value: 'git:https//(アカウント名)@github.com'
  },
  {
    index: 'ユーザー名',
    value: '（GitHubのアカウント名）'
  },
  {
    index: 'パスワード',
    value: '（先ほど発行したToken）'
  },
]

</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>初期設定</h1>
      <ol>
        <li><a href="https://gitforwindows.org/" class="a">git bash</a>のインストールを行う</li>
        <li><a href="https://github.com/" class="a">GitHub</a>のアカウントを作成する</li>
        <li>ServerStarterを起動し，左下の設定ボタンから System Settings を開く</li>
        <li>ShareWorldタブをスクロールし，「Personal access tokens」を開く</li>
        <li>Generate new token ボタンを押し，次のように設定したうえで「Generate token」を押す</li>
        <div style="width: min(600px, 100%);" class="q-pl-md">
          <q-table
            :rows="tokenRows"
            :columns="columns"
            hide-bottom
          />
        </div>
        <li>生成されたトークンをコピーし，適当なテキストファイルに保存しておく</li>
        <li>ServerStarterのShareWorldタブから「Windows資格情報」を開く</li>
        <li>「汎用資格情報の追加」より次のように設定する</li>
        <div style="width: min(600px, 100%);" class="q-pl-md">
          <q-table
            :rows="credentialRows"
            :columns="columns"
            hide-bottom
          />
        </div>
      </ol>
    </q-card-section>
  </q-card>
</template>