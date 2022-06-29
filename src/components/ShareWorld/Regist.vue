<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ServerStarterに設定を登録する</h1>
      <ol>
        <li><a href="https://github.com/">GitHub</a>に新しく空のレポジトリを作成する</li>
        <ol>
          <li><a href="https://github.com/">GitHub</a>にアクセスし，緑色の「New」ボタンを押す</li>
          <li>各設定項目を次のように指定する</li>
          <ul>
            <li>Repository name  ShareWorld</li>
            <li>Description  (分かりやすい説明文　未記入でもOK)</li>
            <li>Public or Private  原則Private　ワールドデータを不特定多数に公開する場合はPublic</li>
            <li>その他の項目  初期設定の通り</li>
          </ul>
          <li>Create repositoryを押し，レポジトリの新規作成を行う</li>
        </ol>
        <li>ServerStarterを起動し，左下の設定ボタンから「システム設定」を開く</li>
        <li>ShareWorldタブを開き，次のように設定する</li>
        <ul>
          <li>アカウント名  (GitHubのアカウント名)</li>
          <li>メールアドレス  (GitHubのアカウント登録時に指定したメールアドレス)</li>
          <li>レポジトリ名  (作成したレポジトリ名)</li>
        </ul>
        <li>「リモート設定一覧」に登録内容が表示されたら登録作業は終わり</li>
      </ol>
    </q-card-section>
  </q-card>
</template>