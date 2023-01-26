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

const repositoryRows = [
  {
    index: 'Repository name',
    value: 'ShareWorld'
  },
  {
    index: 'Description',
    value: '(分かりやすい説明文．未記入でもOK)'
  },
  {
    index: 'Public or Private',
    value: '（原則）Private'
  },
  {
    index: 'Add a README file',
    value: '(チェックを入れる)'
  },
  {
    index: 'その他の項目',
    value: '初期設定の通り'
  },
]

const ShareWorldRows = [
  {
    index: 'アカウント名',
    value: '(GitHubのアカウント名)'
  },
  {
    index: 'メールアドレス',
    value: '(GitHubアカウントのメールアドレス)'
  },
  {
    index: 'リポジトリ名',
    value: '(作成したリポジトリ名)'
  },
]

</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ServerStarterに設定を登録する</h1>
      <ol>
        <li><a href="https://github.com/" class="a">GitHub</a>に新しく空のレポジトリを作成する</li>
        <ol>
          <li><a href="https://github.com/" class="a">GitHub</a>にアクセスし，緑色の「New」ボタンを押す</li>
          <li>各設定項目を次のように指定する</li>
          <div style="width: min(600px, 100%);" class="q-pl-md">
            <q-table
              :rows="repositoryRows"
              :columns="columns"
              hide-bottom
            />
          </div>
          <p>
            ※ワールドデータを不特定多数に公開する場合は"Public or Private"をPublicに設定する
          </p>
          <li>Create repositoryを押し，レポジトリの新規作成を行う</li>
        </ol>
        <li>ServerStarterを起動し，左下の設定ボタンから「システム設定」を開く</li>
        <li>ShareWorldタブを開き，次のように設定する</li>
        <div style="width: min(600px, 100%);" class="q-pl-md">
          <q-table
            :rows="ShareWorldRows"
            :columns="columns"
            hide-bottom
          />
        </div>
        <li>「リモート設定一覧」に登録内容が表示されたら登録作業は終わり</li>
      </ol>
    </q-card-section>
  </q-card>
</template>