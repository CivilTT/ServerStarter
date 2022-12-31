import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import Introduction from '../views/Introduction.vue'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/intro',
    name: 'intro',
    component: Introduction
  },
  {
    path: '/about',
    name: 'about',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
  },
  {
    path: '/main',
    name: 'main',
    component: () => import(/* webpackChunkName: "about" */ '../views/MainWindow.vue')
  },
  {
    path: '/world',
    name: 'world',
    component: () => import(/* webpackChunkName: "about" */ '../views/WorldSettings.vue')
  },
  {
    path: '/system',
    name: 'system',
    component: () => import(/* webpackChunkName: "about" */ '../views/SystemSettings.vue')
  },
  {
    path: '/ShareWorld',
    name: 'sw',
    component: () => import(/* webpackChunkName: "about" */ '../views/ShareWorld.vue')
  },
  {
    path: '/AutoPortMapping',
    name: 'apm',
    component: () => import(/* webpackChunkName: "about" */ '../views/AutoPortMapping.vue')
  },
  {
    path: '/credit',
    name: 'credit',
    component: () => import(/* webpackChunkName: "about" */ '../views/Credit.vue')
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
