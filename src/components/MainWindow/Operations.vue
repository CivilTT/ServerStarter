<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>ワールドの操作</h1>
      <h2>バージョンアップ</h2>
      最新のバージョンがリリースされたなどの要因でワールドのバージョンを上げることができます。
      以下の写真のように、起動したいサーバーバージョンと、バージョンアップしたいワールドを選択してください。
      この場合、1.17.1でサーバーを起動するため、1.16.1のworldを1.17.1にバージョンアップします。<br/>
      <img alt="version up img" src="../../assets/MainWindow/main2.png" style="height: 2cm;"/>
      
      <h2>既存ワールドの導入</h2>
      本システムを導入した際など、もともとのワールドデータを移行したい場面に対応します。
      【new World】として新規ワールドの名称を設定し、「ワールド設定」より「追加要素」タグを開きます。
      <!-- TODO:「配布ワールド」にリンクを付与 -->
      配布ワールドを導入する際と同じ手順でワールドデータの入ったフォルダを選択してください。
      なお、選択するフォルダの階層が以下のようになっていることも確認してください。
      そのあとは、OKを押し、RUNすることでサーバーを本システムより起動することができるようになります。

      <h2>Spigot</h2>
      Spigotサーバーを導入する場合は【new Version】にてImport SpigotをYesに変更してください。
      これにより、バージョンの一覧がSpigotのものに切り替わります。
      次回以降Spigotがすでに導入されている状態では、普通のバージョンと同じようにバージョン一覧から選択できるようになっています。<br/>
      <img alt="spigot img" src="../../assets/MainWindow/Spigot1.png" style="height: 5cm;"/>

      <h2>SpigotとVanila</h2>
      もともとVanilaサーバーとして作成したワールドをSpigotサーバーとして立てたい場合、自動的に変換する機能が作動します。
      また、逆にSpigotからVanilaに変換することもできます。
      バージョンアップの際と同様に起動したいサーバーともともとのワールドを選択するだけで自動的にサーバーデータの変換が行われます。<br/>
      <img alt="spigot and vanila" src="../../assets/MainWindow/Spigot2.png" style="height: 3cm;"/>

    </q-card-section>
  </q-card>
</template>