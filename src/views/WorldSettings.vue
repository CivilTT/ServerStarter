<script lang="ts" setup>
import Top from '@/components/WorldSettings/Top.vue';
import Property from '@/components/WorldSettings/Property.vue';
import Additional from '@/components/WorldSettings/Additional.vue'; // @ is an alias to /src
import OpWhiteList from '@/components/WorldSettings/OpWhiteList.vue'; // @ is an alias to /src
</script>

<template>
  <div class="world">
    <Top/>
    <Property/>
    <Additional/>
    <OpWhiteList/>
  </div>
</template>


