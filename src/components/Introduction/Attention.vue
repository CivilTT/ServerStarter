<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>注意点</h1>
      <ul>
        <li>
          <p>
            ServerStarterではJavaの自動インストールはサポートされておりません．
            Javaがインストールされていない場合は以下のような案内が出ますので，指示に沿ってインストールしてください．
            サーバーの起動にはJDK（Java Development Kit）が必要です
          </p>
          <q-img src="@/assets/Introduction/JavaInstall.png" width="min(500px,100%)"/>
        </li>
        <li>
          <p>
            自動ポート開放は「ルーター」に対してのみ開放を行いますが，セキュリティソフトに対しては開放しません．
            このため，セキュリティソフトについてはご自身でポート開放を行っていただく必要があります．
            詳細は<a class="a" href="AutoPortMapping">自動ポート開放</a>を参照してください．
          </p>
        </li>
      </ul>
    </q-card-section>
  </q-card>
</template>