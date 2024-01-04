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
    index: '名称',
    value: '(任意．一例は「Minecraft」)'
  },
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
      <p>
        各ルーターやセキュリティソフトごとに細かい設定内容や説明文の表現方法が異なります<br />
        ここではWindowsDefenderを一例に一般的な設定項目を説明しますが，詳細な設定方法については各メーカーの取扱説明書をご参照下さい
      </p>
      <ol>
        <li>ルーターやセキュリティソフトの設定画面を開く</li>
        <li>「ファイアウォール」や「セキュリティー」の設定画面を開く</li>
        <q-img src="~assets/AutoPortMapping/FireWallList.png" width="min(500px,100%)" />
        <li>「ルールの追加」や「ポートの追加」を開く</li>
        <q-img src="~assets/AutoPortMapping/AddPortMapping.png" width="min(500px,100%)" />
        <li>各設定項目を次のように設定する</li>
        <div style="width: min(600px, 100%);" class="q-pa-md">
          <q-table :rows="rows" :columns="columns" hide-bottom />
        </div>
        <q-img src="~assets/AutoPortMapping/PortNum.png" width="min(500px,100%)" />
        <q-img src="~assets/AutoPortMapping/PortRange.png" width="min(500px,100%)" />
        <q-img src="~assets/AutoPortMapping/PortTarget.png" width="min(500px,100%)" />
        <q-img src="~assets/AutoPortMapping/PortName.png" width="min(500px,100%)" />
        <li>設定を保存し、サーバーにほかの人がログインできれば設定終了</li>
      </ol>
      <p>
        ※一人でポート開放の確認を行う場合は、サーバー起動後に<a href="https://www.cman.jp/network/support/port.html"
          class="a">こちらのHP</a>にアクセスすることで確認できます
      </p>
</template>
