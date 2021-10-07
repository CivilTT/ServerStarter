# 言語を選択(Select language)
<details>
  <summary>
    日本語
  </summary>
  <div>

# Server Starter
ボタンクリックによって簡単にサーバーを立てることができるようにするソフトウェアです。

# 使い方
1. `Setup_ServerStarter.msi`を[こちら](https://github.com/CivilTT/ServerStarter/releases/download/v1.1.0/Setup_ServerStarter.msi)からダウンロードしてください。
2. ダウンロードしたファイルを起動し、デスクトップに`Server Starter`というショートカットが作成されたことを確認してください。
3. `Server Starter`を起動してください。
4. プレイヤー名などを入力する画面にて情報を入力し、次の画面で起動したいサーバーのバージョンとワールド名を設定してください。
5. "RUN"を押してください。
6. 少しするとサーバーの起動が開始されます。

# 搭載機能
## 初期設定
~~~
本システムはポート開放（ポートマッピング）機能をサポートしていません。
サーバーにほかの人が入るためには必要な作業なので、25565番のポートを解放してください。
~~~
インストール後に最初に起動すると以下のような画面が表示されます。<br>
自身のゲーム内での名前を入力してください。<br>
GitについてはShareWorldというレポジトリを作成し、[サーバー同期機能](https://github.com/CivilTT/ServerStarter#shareworld)を使用する場合は入力してください。<br>
使用しない場合はExampleのままで問題ありません。<br>
<br>
![infobuilder](https://github.com/CivilTT/ServerStarter/blob/master/Images/infobuilder.jpg)

## サーバーの起動
以下の画面が本システムのメイン画面です。<br>
`Version`にて起動するサーバーのバージョンを指定し、`World`にて起動するワールドを選択します。<br>
最初はVersionもWordも何もインストールされていないため、【new Version(World)】より、インストールしたいバージョンを選択し、起動するワールドの名前を決めてください。<br>
<br>
![main1](https://github.com/CivilTT/ServerStarter/blob/master/Images/main1.png)

### バージョンアップ
最新のバージョンがリリースされたなどの要因でワールドのバージョンを上げることができます。<br>
以下の写真のように、起動したいサーバーバージョンと、バージョンアップしたいワールドを選択してください。<br>
この場合、**1.17.1**でサーバーを起動するため、**1.16.1のworld**を1.17.1にバージョンアップします。<br>
<br>
![main3](https://github.com/CivilTT/ServerStarter/blob/master/Images/main3.png)

### 既存ワールドの導入
本システムを導入した際など、もともとのワールドデータを移行したい場面に対応します。<br>
【new World】として新規ワールドの名称を設定し、More SettingsよりCustom Mapボタンを押します。<br>
[配布ワールドを導入する](https://github.com/CivilTT/ServerStarter#%E9%85%8D%E5%B8%83%E3%83%AF%E3%83%BC%E3%83%AB%E3%83%89custom-map)際と同じ手順でワールドデータの入ったフォルダを選択してください。<br>
なお、選択するフォルダの階層が以下のようになっていることも確認してください。<br>
そのあとは、OKを押し、RUNすることでサーバーを本システムより起動することができるようになります。<br>
~~~
（選択するフォルダ）
    |
    ├─advancements
    ├─datapacks
    :
    (現在編集中です)
~~~

### Spigot
Spigotサーバーを導入する場合は【new Version】にて`Import Spigot`をYesに変更してください。<br>
これにより、バージョンの一覧がSpigotのものに切り替わります。<br>
次回以降Spigotがすでに導入されている状態では、普通のバージョンと同じようにバージョン一覧から選択できるようになっています。<br>
<br>
![Spigot1](https://github.com/CivilTT/ServerStarter/blob/master/Images/Spigot1.png)
    
### SpigotとVanila
もともとVanilaサーバーとして作成したワールドをSpigotサーバーとして立てたい場合、自動的に変換する機能が作動します。<br>
また、逆にSpigotからVanilaに変換することもできます。<br>
バージョンアップの際と同様に起動したいサーバーともともとのワールドを選択するだけで自動的にサーバーデータの変換が行われます。<br>
<br>
![Spigot2](https://github.com/CivilTT/ServerStarter/blob/master/Images/Spigot2.png)

## ShareWorld
サーバーの起動を常に1人が行う場合、その人がいないときはマルチプレイができません。<br>
しかし、このShareWorldを用いることで、前回サーバーを立てた人とは別の人でもサーバーを最新の状態で起動することができます。<br>
### 事前準備（Gitレポジトリの設定）
[こちらの記事](https://qiita.com/CivilTT/items/16d53b734ac9d75c2e79)にまとめてあるため、参照しながら作業を進めてください。

### 利用方法
`info.txt`の編集が終わり次第、保存したうえでこれを閉じ、本システムをショートカットより起動して下さい。<br>
すると、以下の写真のようにWorldの一覧に`ShareWorld`が追加されています。<br>
これを選択してワールドを起動することで、同期されたサーバーシステムを構築することができます。<br>
<br>
![main2](https://github.com/CivilTT/ServerStarter/blob/master/Images/main2.png)
<br>
なお、同様の設定をサーバーを共有で開く可能性のある人にも行うことで、サーバー起動時にその人にも最新のデータが同期されるようになります。
    
## More Settings
メイン画面の一番下にある`More Settings`を選択することにより、新しいウィンドウが開きます。
### Server Properties
以下のような画面にて設定を行うことができます。<br>
主要な項目を**Main Settings**にて表示し、そのほかの項目についてはtrue/falseで設定するもの、数字などの文字で指定するものに分けて**Other Settings**にて表示しています。<br>
<br>
![moresettings1](https://github.com/CivilTT/ServerStarter/blob/master/Images/moresettings1.png)
    
### 配布ワールド＆Datapacks＆Plugins 
これらの設定はMore Settingsの上部にあるボタンより、別のウィンドウを立ち上げることで、設定できるようになります。
#### 配布ワールド（Custom Map）
配布ワールドは新規ワールドの導入時のみ、開くことができる設定になっています。<br>
配布ワールドの製作者様が作成したzipファイルや展開済みのフォルダをImportボタンを押して、選択してください。<br>
この時、選択するデータの種類をあらかじめ設定しておいてください。以下の画像ではImportを押すとzipファイルを選択できるようになっています。<br>
設定が終わり次第、OKボタンを押し、設定を保存してください。<br>
<br>
![custom1](https://github.com/CivilTT/ServerStarter/blob/master/Images/custom1.png)
    

#### Datapacks
データパックについても配布ワールドと同様に、導入するファイルを選択することで設定ができます。<br>
また、リスト中のデータパックを選択し、Removeボタンを押すことで、データパックを削除することができます。<br>
<br>
![datapack1](https://github.com/CivilTT/ServerStarter/blob/master/Images/datapack1.png)
    
#### Plugins
PluginはSpigotサーバーを導入する際にのみ、設定を行うことができます。<br>
これも配布ワールドと同様の手順でjarファイルを選択することにより、設定ができます。<br>
また、Pluginの削除についても、リスト上で選択したのちにRemoveボタンを押すことで反映されます。<br>
<br>
![plugin1](https://github.com/CivilTT/ServerStarter/blob/master/Images/plugins1.png)

## Others
### ワールドデータのリセット
同じ名前のワールドを新しく作り直す場合、ワールドのリセットができます。<br>
以下の画像のように、メイン画面にある`Recreate World`のチェックボックスを入れてください。<br>
また、前のワールドを保存する場合はその下の`Save World`のチェックボックスも入れてください。<br>
<br>
![reset](https://github.com/CivilTT/ServerStarter/blob/master/Images/Reset.png)

### データの削除
同じ名前のワールドを再生成するのではなく、ワールドを削除してしまうこともできます。<br>
メイン画面にてバージョンやワールドの選択を行う欄の横にゴミ箱ボタンがあります。<br>
削除したいバージョンやワールドを選択したうえでこのボタンを押すことで、データを削除することができます。<br>
<br>
![delete1]()
![delete2]()

### OP権限の付与
サーバーを起動した際にコマンドを使いたい時など、自身にOP権限を付与したい場面は多いと思います。<br>
以下の画像のように、メイン画面の`~~ has op rights`のチェックボックスを入れることで、サーバーを起動した人には自動でOP権限を付与することができます。<br>
この際、付与する権限レベルは最高の4になり、ほかの参加者に自動的にOP権限を付与するわけではありません。<br>
<br>
![op](https://github.com/CivilTT/ServerStarter/blob/master/Images/op.png)

### サーバー終了後のPCのシャットダウン
サーバーを起動した人が先にゲームから抜けてしまい、参加者が全員抜けた後もPCがつけっぱなしになってしまうことがあると思います。<br>
以下の画像のように、メイン画面の`Shutdown this PC`のチェックボックスを入れておくことで、サーバーが停止した後にPCを自動でシャットダウンします。<br>
サーバーを停止する必要はあるため、最後に抜ける人にゲーム内で`/stop`のコマンドを打ってもらう必要はあります。<br>
なお、シャットダウンする前に確認のダイアログが表示されるため、サーバー終了後にシャットダウンしないことも選択できます。<br>
<br>
![shutdown](https://github.com/CivilTT/ServerStarter/blob/master/Images/shutdown.png)

### 開発者向け機能（ベータ版）
本システムは基本的な機能についてはコマンドラインより操作することができます。<br>
詳細な解説についてはカレントディレクトリをインストールフォルダへ移動させ、`/?`オプションよりご確認ください。<br>
なお、規定通りのインストールフォルダにインストールされている場合は、以下のコマンドで確認することができます。<br>
    
~~~
cd .\AppData\Roaming\.minecraft\Servers
Server_GUI2.exe /?
~~~

また、More Settingsの最下部にある`Get All-VerWor.json`のチェックボックスを適用することにより、本システムにインストールされているバージョンとワールドデータの一覧を表示することができます。<br>
<br>
![all-verwor](https://github.com/CivilTT/ServerStarter/blob/master/Images/All-VerWor.png)
    
# 利用規約
インストーラに同梱されており、利用開始時にこれに同意する必要があります。<br>
なお利用規約はバージョンの改定とともに、予告なく変更する可能性がありますこと、予めご了承ください。

# 問題が発生した場合
個別の環境における問題については、作者が回答することはありません。<br>
しかし、明らかなシステム側のバグである場合やバグであることが疑われる場合は、恐れ入りますが作者の[Twitter](https://twitter.com/CivilT_T)のDMにそっとご報告いただけますと幸いです。<br>
よろしくお願いいたします。

# 詳細な解説＆機能紹介
[こちら](https://qiita.com/CivilTT/items/a59d9be7cea50d60a666)のリンクよりご確認ください。


  </div>
</details>
<details>
  <summary>
    English
  </summary>
  <div>

# Server Starter
You can easily build the Minecraft Multiplay server

# How to use
1. Download `Setup_ServerStarter.msi` at [here](https://github.com/CivilTT/ServerStarter/releases/download/v1.0.0/Setup_ServerStarter.msi)
2. Start this file and check to create `Server Starter` at your Desktop
3. Start `Server Starter`
4. Set any information and Select Version and World
5. Push "RUN" button
6. You can build the Minecraft Server!!

# More Information
It is [HERE](https://qiita.com/CivilTT/items/a59d9be7cea50d60a666)!!

  </div>
</details>
