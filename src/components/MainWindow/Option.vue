<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>オプション項目</h1>
      <!-- <h2>ワールドのリセット</h2>
      <p>
        同じ名前のワールドを新しく作り直す場合、ワールドのリセットができます。
        以下の画像のように、メイン画面にあるRecreate Worldのチェックボックスを入れてください。
        また、前のワールドを保存する場合はその下のSave Worldのチェックボックスも入れてください。
        同じシード値のワールドにはならないのでお気を付けください
      </p>
      <img alt="reset world" src="~assets/MainWindow/Reset.png" style="height: 4cm;"/> -->

      <h2>データの削除</h2>
      <p>
        メイン画面にてバージョンやワールドの横にあるゴミ箱ボタン
        <q-img alt="delete button" src="~assets/MainWindow/delete1.png" width="1.4em" />
        を押すと，データを削除できます<br />
        バージョンを削除すると，そのバージョンで起動したワールドも削除されます
      </p>
      <q-img alt="delete window" src="~assets/MainWindow/delete2.png" width="min(500px,100%)" />


      <h2>OP権限の付与</h2>
      <p>
        サーバーを起動した際にコマンドを使いたい時など、自身にOP権限を付与したい場面は多いと思います。<br />
        以下の画像のように、メイン画面の~~ has op rightsのチェックボックスを入れることで、サーバーを起動した人には自動でOP権限を付与することができます。<br />
        他の参加者にもOP権限を付与する場合は，「ワールド設定」の「Op権限」タブにて付与してください．
      </p>
      <q-img alt="op check" src="~assets/MainWindow/op.png" width="min(500px,100%)" />

      <h2>自動シャットダウン</h2>
      <p>
        サーバーを起動した人が先にゲームから抜けてしまい、参加者が全員抜けた後もPCがつけっぱなしになってしまうことがあると思います。<br />
        以下の画像のように、メイン画面のShutdown this PCのチェックボックスを入れておくことで、サーバーが停止した後にPCを自動でシャットダウンします。<br />
        最後に抜ける人（Op権限4を持つ人）がゲーム内で<b>/stopのコマンド</b>を打つことで自動シャットダウンが作動します<br />
        なお、サーバー終了後にシャットダウンしないことを終了後のダイアログで選択することもできます。
      </p>
      <q-img alt="shutdown" src="~assets/MainWindow/shutdown.png" width="min(500px,100%)" />

    </q-card-section>
  </q-card>
</template>