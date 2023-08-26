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
    index: 'アカウント',
    value: 'ワールドの保存先アカウントの指定'
  },
  {
    index: 'リモートワールド',
    value: '読み込む（or 保存する）リモートの指定'
  },
]
</script>

<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>個別のワールドをShareWorldにする</h1>
      <ol>
        <li>「ワールド名」にてShareWorldにしたいワールドを選択（new Worldも可）</li>
        <li>「ワールド設定」を押し，ShareWorldのタブを開く</li>
        <li>ShareWorldを利用する，を「はい」に設定し，以下のように設定する</li>
        <div style="width: min(600px, 100%);" class="q-pl-md">
          <q-table
            :rows="rows"
            :columns="columns"
            hide-bottom
          />
        </div>
        <li>「保存」を押し，「実行」することでワールドがShareWorldになった状態で起動します</li>
      </ol>
    </q-card-section>
  </q-card>
</template>