<template>
  <q-card flat style="max-width: 100%;">
    <q-card-section>
      <h1>自動ポート開放</h1>
      <p>
        <b>ルーターに対しては</b>自動でポートを解放する機能がServerStarterに付属しています<br />
        自動でポートの開放を行い、素早くサーバーを起動しましょう！
      </p>
      <ol>
        <li>ServerStarterを開き、システム設定を開く</li>
        <li>Networkタブを開き、「自動ポート開放を利用する」を「はい」に設定</li>
        <li>開放するポート番号（基本は25565）を指定し、「開放テスト」を押す</li>
        <li>ステータスがOpened!になれば，本機能を利用してルーターのポート開放を行うことが可能</li>
      </ol>
      <q-img src="~assets/AutoPortMapping/APM_Opened.png" width="min(500px,100%)" />

      <h3>ステータスがFailed…になった場合</h3>
      <p>
        ご利用者様の環境では本機能をご利用いただけませんので、ご自分でルーターにポート開放を行っていただく必要があります
      </p>
      <p>
        Failed...になる主な理由としましては
      </p>
      <ul>
        <li>すでに25565番のポート開放設定がされている</li>
        <li>ルーターにてUPnPを拒否する設定を行っている</li>
        <li>多重ルーターによるネットワークになっている</li>
      </ul>
      <p>
        などが考えられます
      </p>
    </q-card-section>
  </q-card>
</template>