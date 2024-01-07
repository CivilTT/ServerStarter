<script setup lang="ts">
interface Prop {
  osName: 'windows' | 'mac' | 'linux';
  version: string
  disable?: boolean
  outline?: boolean
}
const prop = defineProps<Prop>();

function getFileName() {
  switch (prop.osName) {
    case 'windows':
      return `ServerStarter2.${prop.version}.msi`
    case 'mac':
      return `ServerStarter2-${prop.version}.pkg`
    case 'linux':
      return ''
    default:
      break;
  }
}

function getOSName() {
  switch (prop.osName) {
    case 'windows':
      return 'Windows'
    case 'mac':
      return 'Mac OS'
    case 'linux':
      return 'Linux'
    default:
      break;
  }
}
</script>

<template>
  <q-btn
    :disable="disable"
    :outline="outline"
    color="white"
    text-color="primary"
    padding="md"
    :href="`https://github.com/CivilTT/ServerStarter2/releases/latest/download/${getFileName()}`"
    style="width: 10rem"
  >
    <div class="row items-center">
      <q-avatar size="2rem" square class="q-mr-md">
        <q-img :src="`assets/OS/${osName}.svg`" />
      </q-avatar>
      <div class="download">
        {{ getOSName() }}
      </div>
    </div>
  </q-btn>
</template>

<style scoped lang="scss">
.download {
  text-transform: none;
}
</style>
