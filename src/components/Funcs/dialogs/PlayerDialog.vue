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
        難しいコマンドを覚えたり，複雑なファイルを編集したりする作業に別れを告げましょう<br>
        ServerStarterでは，OP権限やホワイトリストと言った，プレイヤーの権限を簡単に設定することができます
      </p>
      <p>
        ユーザーの一覧と権限の様子は視覚的に分かりやすく表示されるようになります！
      </p>
      <q-img src="~assets/Funcs/Player/PlayerSettings.png" class="q-mb-md" />

      <p>
        複数のユーザーを登録するときには，グループ機能が便利です<br>
        一括でホワイトリストに登録したり，OP権限を設定したりすることができます！
      </p>
      <q-img src="~assets/Funcs/Player/Groups.png" />

      <template #install>
        <ol>
          <li>プレイヤータブを選択</li>
          <q-img src="~assets/Funcs/Player/tabs.png" class="q-mb-md" />

          <li>プレイヤー一覧を更新</li>
          <ul>
            <li>登録したいプレイヤー名を入力欄で検索</li>
            <li>グループから一括で追加</li>
            <li>個々のプレイヤーや，グループで選択したプレイヤーを削除</li>
          </ul>

          右側の操作盤から様々な設定をすることができます！
          <div class="row q-gutter-md q-pr-md q-pt-md justify-center">
            <q-img src="~assets/Funcs/Player/PlayerOperation.png" style="width: 45%;" />
            <q-img src="~assets/Funcs/Player/GroupOperation.png" style="width: 45%;" />
          </div>
        </ol>
      </template>
    </BaseDialogCard>
  </q-dialog>
</template>
