﻿# 計画書

### classes

#### class﻿ Version

バージョンデータ



#### class﻿ World

ワールドデータ本体

###### class LocalWorld

ローカルに存在するワールド。存在しないときはこのクラスは使えない。

###### class RemoteWorld

リモートに存在するワールド。

.ToLocal()
#ローカルにPullする

.FromLocal()
#ローカルからPushする



#### class WorldWrapper

GUI上で選択するときのワールドはこっち。

.WrapRun()
#ワールドの新規作成とかVtoSとかPullとかぜんぶやってくれる
#引数に渡したサーバーを起動。



#### class WorldCollection

実際はWorldWrapperの集合

リンクされてなかったローカルワールドを新しくリンク
リンクされていたワールドの接続を解除



#### class Storage

Gitのリポジトリ的なやつ

各々がRemoteWorld(ブランチ)一覧を持つ



#### class ServerGuiPathPath

static .Instance #ServerSterterのカレントディレクトリ

.WorldData
#ワールドデータのディレクトリ

こんな感じでプロパティアクセスで下位ディレクトリにアクセスできる


