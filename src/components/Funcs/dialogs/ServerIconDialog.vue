<script setup lang="ts">
import { useDialogPluginComponent } from 'quasar';
import { funcDialogProp } from './baseDialog/iBaseDialog';
import BaseDialogCard from './baseDialog/BaseDialogCard.vue';

defineProps<funcDialogProp>()
defineEmits({ ...useDialogPluginComponent.emitsObject })
const { dialogRef, onDialogHide, onDialogOK } = useDialogPluginComponent()
</script>

<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide">
    <BaseDialogCard :title="title" @close="onDialogOK">
      <p>
        サーバーの参加者が複数のサーバーを見分ける際に重要視する要素の一つに「サーバーアイコン」があります<br>
        適切なサーバーアイコンの設定はサーバーの様子を一目で確認できる一方，適切な画像を自前で用意するのは面倒でした
      </p>
      <p>
        ServerStarterではサーバーをよりブラッシュアップするためのサポートを惜しみません！<br>
        自由な画像を適切な画像に自動で変換するだけでなく，簡単に設定するためのアイコン候補も完備！<br>
        自分のサーバーを明るく着飾りましょう！
      </p>

      <template #install>
        <ol>
          <li>ホームタブ右上の「アイコンを変更」ボタンをクリック</li>
          <li>お好きなブロックをクリックするか，左上の「＋」ボタンから好きな画像を選択するだけ！</li>
        </ol>
        <div class="column q-gutter-md q-pr-md">
          <q-img src="~assets/Funcs/IconSettings/defaultIcons.png" />
          <q-img src="~assets/Funcs/IconSettings/CustomImg.png" />
        </div>
      </template>
    </BaseDialogCard>
  </q-dialog>
</template>
