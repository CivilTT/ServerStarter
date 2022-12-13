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
        <ul>
          <li>Note  適当に分かりやすい名称を付ける</li>
          <li>Expiration  適当に分かりやすい名称を付ける</li>
          <li>Select scopes  「repo」にチェックを付ける</li>
        </ul>
        <li>生成されたトークンをコピーし，適当なテキストファイルに保存しておく</li>
        <li>ServerStarterのShareWorldタブから「Windows資格情報」を開く</li>
        <li>「汎用資格情報の追加」より次のように設定する</li>
        <ul>
          <li>アドレス  git:https//(アカウント名)@github.com</li>
          <li>Expiration  (アカウント名)</li>
          <li>Select scopes  (保存したトークン)</li>
        </ul>
        <li>「ServerStarterに設定を登録する」に進む</li>
      </ol>
    </q-card-section>
  </q-card>
</template>