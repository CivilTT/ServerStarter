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
    ├─data
    ├─datapacks
    ├─DIM1
    ├─DIM-1
    ├─dimensions
    ├─entities
    ├─playerdata
    ├─poi
    ├─region
    ├─stats
    ├─level.dat
    ├─level.dat_old
    └─session.lock
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
    
# Functions
## Init Settings
~~~
This system does not support the port open (port mapping) function.
Open port 25565 as it is a necessary task for others to enter the server.
~~~
    
When you start it for the first time after installation, the following screen will be displayed.<br>
Enter your in-game name.<br>
For Git, create a repository called ShareWorld and type it if you want to use the [Server Synchronization Function]().<br>
If you do not use it, you can leave it as Example.<br>
<br>
![infobuilder](https://github.com/CivilTT/ServerStarter/blob/master/Images/infobuilder.jpg)

## Start the Server
The following screen is the main screen of this system.<br>
Select the version of the server to start with `Version`, and select the world to start with `World`.<br>
At first, neither Version nor Word is installed, so select the version you want to install from 【new Version (World)】 and decide the name of the World.<br>
<br>
![main1](https://github.com/CivilTT/ServerStarter/blob/master/Images/main1.png)
    
### Version-UP
You can upgrade the version of the world due to factors such as the latest version being released.<br>
Select the server version you want to start and the world you want to upgrade, as shown in the picture below.<br>
In this case, since the server will be started with **1.17.1**, the **World of 1.16.1** will be upgraded to 1.17.1.<br>
<br>
![main3](https://github.com/CivilTT/ServerStarter/blob/master/Images/main3.png)

### Import existing world
I can correspond the case you want to import the original world data, such as when this system is introduced.<br>
Set the name of the new world as 【new World】 and push the Custom Map button from More Settings.<br>
Select the folder containing the world data in the same process as when [Import Custom Map]().<br>
Also, make sure that the hierarchy of the selected folder is as follows.<br>
After that, you can start the server from this system by pushing OK and running.<br>
~~~
（Selected Folder）
    |
    ├─advancements
    ├─data
    ├─datapacks
    ├─DIM1
    ├─DIM-1
    ├─dimensions
    ├─entities
    ├─playerdata
    ├─poi
    ├─region
    ├─stats
    ├─level.dat
    ├─level.dat_old
    └─session.lock
~~~

### Spigot
When installing the Spigot server, change Import Spigot to Yes in 【new Version】.<br>
This will switch the list of versions to Spigot's.<br>
From the next time onwards, when Spigot is already installed, you can select from the version list in the same way as a vanila version.<br>
<br>
![Spigot1](https://github.com/CivilTT/ServerStarter/blob/master/Images/Spigot1.png)
    
### Spigot and Vanila
If you want to set up a World originally created as a Vanila server as a Spigot server, the automatic conversion function will work.<br>
Of cource, you can also convert from Spigot to Vanila.<br>
Just select the server Version you want to start and the original World, then the server data will be converted automatically.<br>
<br>
![Spigot2](https://github.com/CivilTT/ServerStarter/blob/master/Images/Spigot2.png)
    
## ShareWorld
If one person is always opening the server, multiplayer will not be possible without that one.<br>
However, by using this ShareWorld, even another person who set up the server last time can start the server everytime in the latest state.<br>
    
### Init Settings (Set the Git repository)
I wrote it in [this page](https://qiita.com/CivilTT/items/16d53b734ac9d75c2e79). Please set the repository yourself with it.

### How to use
As soon as you finish editing `info.txt`, start this system from the shortcut.<br>
Then, ShareWorld is added to the list of World as shown in the picture below.<br>
You can build Synchronized Server System by selecting ShareWorld and launching the World.<br>
<br>
![main2](https://github.com/CivilTT/ServerStarter/blob/master/Images/main2.png)
<br>
In addition, a person who will open the ShareWorld Server has to do same settings. Then, you and a person share World data.  

## More Settings
Select More Settings at the bottom of the main window.
    
### Server Properties
You can make settings on the screen below.<br>
Main items are displayed in **Main Settings**, and other items are displayed in **Other Settings** separately for those set with true / false and those specified with characters such as numbers.<br>
<br>
![moresettings1](https://github.com/CivilTT/ServerStarter/blob/master/Images/moresettings1.png)
    
### Custom Map & Datapacks & Plugins
These settings can be set by launching another window from the button at the top of More Settings.

#### Custom Map
The Custom Map is set to be open only when a new world is introduced.<br>
Click the Import button to select the zip file or extracted folder created by the creator of it.<br>
At this time, set the type of data to be selected in advance. In the image below, you can select the zip file by pressing Import.<br>
Then, press the OK button to save the settings.<br>
<br>    
![custom1](https://github.com/CivilTT/ServerStarter/blob/master/Images/custom1.png)
    
#### Datapacks
As with the Custom Map, Datapacks can also be set by selecting the files to be installed.<br>
You can also remove the Datapack by selecting it in the list and pressing the Remove button.<br>
<br>
![datapack1](https://github.com/CivilTT/ServerStarter/blob/master/Images/datapack1.png)
    
#### Plugins
The Plugin can only be available when useing the Spigot server.<br>
This can also be set by selecting the jar file in the same procedure as for the Custom Map.<br>
Also, deleting the Plugin will be reflected by pressing the Remove button after selecting it on the list.<br>
<br>
![plugin1](https://github.com/CivilTT/ServerStarter/blob/master/Images/plugins1.png)
    

## Others
### Reset the World
If you want to recreate a new world with the same name, you can reset the world.<br>
Check the `Recreate World` checkbox on the main screen as shown in the image below.<br>
Also, if you want to save the previous world, check the `Save World` checkbox below it.<br>
<br>
![reset](https://github.com/CivilTT/ServerStarter/blob/master/Images/Reset.png)
    
### Delete the data
You can also delete a World not to recreat it with the same name.<br>
There is a trash button where next to the Version and World list.<br>
You can delete the data by selecting the Version or World you want to delete and pressing this button.<br>
<br>
![delete1]()
![delete2]()
    
### Give OP rights
I think there are many situations where you want to give OP authority to yourself, such as when you want to use any commands.<br>
As shown in the image below, you can automatically get OP rights by checking the `~~ has op rights` check box on the main screen.<br>
At this time, the authority level to be granted will be the highest 4, and OP rights will not be automatically granted to other participants.<br>
<br>
![op](https://github.com/CivilTT/ServerStarter/blob/master/Images/op.png)
    
### Shutdown your PC after stopping the Server

    
### For Developpers (Beta Ver.)

# Terms of Service
    
# TroubleShooting

# More Information
It is [HERE](https://qiita.com/CivilTT/items/a59d9be7cea50d60a666)!!

  </div>
</details>
