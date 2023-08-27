<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ワールドの操作</h1>
      <h2>バージョンアップ</h2>
      <p>
        最新のバージョンがリリースされたなどの要因でワールドのバージョンを上げることができます。<br />
        画像の例では、1.16.1のworldを1.17.1にバージョンアップします。
      </p>
      <q-img alt="version up img" src="~assets/MainWindow/main2.png" />

      <h2>既存ワールドの導入</h2>
      <p>
        本システムを導入した際など、もともとのワールドデータを移行したい場面に対応します。
      </p>
      <ol>
        <li>【new World】として新規ワールドを設定し、「ワールド設定」より「追加要素」タグを開く</li>
        <li>CustomMapタグを開き，「データパックの種類」をFolderにする</li>
        <li>インポートボタンからワールドのフォルダを選択</li>
        <li>左下の「保存」を押して設定画面を閉じてサーバーを「実行」！</li>
      </ol>
      <p>
        あとは既存のワールドデータを使い，ServerStarterがサーバーを起動します！
      </p>

      <h2>Spigot</h2>
      <p>
        Spigotサーバーを導入する場合は【new Version】にてImport SpigotをYesに変更してください。<br />
        バージョンの一覧がSpigotのものに切り替るため，新規でSpigotサーバーを起動することができます！<br />
      </p>
      <q-img alt="spigot img" src="~assets/MainWindow/Spigot1.png" width="min(500px,100%)" />

      <h2>SpigotとVanila</h2>
      <p>
        SpigotサーバーとVanilaサーバーのデータは相互に変換できます<br />
        画像のように起動したいサーバーバージョンとワールドを選択するだけでサーバーを起動できます！
      </p>
      <q-img alt="spigot and vanila" src="~assets/MainWindow/Spigot2.png" />
    </q-card-section>
  </q-card>
</template>