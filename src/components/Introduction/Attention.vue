<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>注意点</h1>
      <ul>
        <li>
          <p>
            ServerStarterではJavaの自動インストールはサポートされておりません．<br />
            Javaがインストールされていない場合は以下のような案内が出ますので，インストールをお願いします．<br />
            サーバーの起動には<a href="https://download.oracle.com/java/17/latest/jdk-17_windows-x64_bin.msi" class="a">JDK（Java
              Development Kit）</a>が必要です．<br />
            リンクを開くとインストーラーをダウンロードします．
          </p>
          <q-img src="~assets/Introduction/JavaInstall.png" width="min(500px,100%)" />
        </li>
        <li>
          <p>
            自動ポート開放は「ルーター」に対してのみ開放を行いますが，セキュリティソフトに対しては開放しません．<br />
            このため，セキュリティソフトについてはご自身でポート開放を行っていただく必要があります．<br />
            詳細は<a class="a" href="AutoPortMapping">自動ポート開放</a>を参照してください．
          </p>
        </li>
      </ul>
    </q-card-section>
  </q-card>
</template>