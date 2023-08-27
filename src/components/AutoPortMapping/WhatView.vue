<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ポート開放って何？</h1>
      <p>
        インターネットはセキュリティの関係で外部からPCへアクセスすることについていくつかの制限があり、「ポート」はその一つになります
      </p>
      <p>
        画像のように外部のプレイヤーと管理者ではサーバーに至るまでの経路が異なります
      </p>
      <p>
        友人やそのほかのユーザーがサーバーに接続するためにはそれらアクセスを許可する設定（ポート開放）を経路上の機器にする必要があります
      </p>
      <q-img src="~assets/AutoPortMapping/Abstruct.png" width="min(800px,100%)" class="img" />
    </q-card-section>
  </q-card>
</template>


<style scoped>
.img {
  display: block;
  margin: 0 auto;
}
</style>