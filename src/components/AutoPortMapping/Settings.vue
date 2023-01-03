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

const rows = [
  {
    index: '外部からのアクセス',
    value: '（全てのPCに対して）許可'
  },
  {
    index: 'ポートの種類',
    value: 'TCP'
  },
  {
    index: 'LAN側IPアドレス',
    value: '(192.168.で始まるIPアドレス)'
  },
  {
    index: 'ポート番号',
    value: '25565 - (25565)'
  },
]

</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>設定項目</h1>
      <p>
        各ルーターやセキュリティソフトごとに細かい設定内容や説明文の表現方法が異なります<br/>
        ここでは一般的な設定項目について示しますが，詳細な設定方法については各メーカーの取扱説明書をご参照下さい
      </p>
      <ol>
        <li>ルーターやセキュリティソフトの設定画面を開く</li>
        <li>「ファイアウォール」や「セキュリティー」の設定画面を開く</li>
        <li>「ルールの追加」や「ポートの追加」を開く</li>
        <li>各設定項目を次のように設定する</li>
        <div style="width: min(600px, 100%);" class="q-pl-md">
          <q-table
            :rows="rows"
            :columns="columns"
            hide-bottom
          />
        </div>
        <li>設定を保存し、サーバーにほかの人がログインできれば設定終了</li>
      </ol>
      <p>
        ※一人でポート開放の確認を行う場合は、サーバー起動後に<a href="https://www.cman.jp/network/support/port.html" class="a">こちらのHP</a>にアクセスすることで確認できます
      </p>
    </q-card-section>
  </q-card>
</template>