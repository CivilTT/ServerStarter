import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: () => import('pages/HomeView.vue')
  },
  {
    path: '/intro',
    name: 'intro',
    component: () => import('pages/IntroductionView.vue')
  },
  {
    path: '/about',
    name: 'about',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('pages/AboutView.vue')
  },
  // {
  //   path: '/main',
  //   name: 'main',
  //   component: () => import('pages/MainWindow.vue')
  // },
  // {
  //   path: '/world',
  //   name: 'world',
  //   component: () => import('pages/WorldSettings.vue')
  // },
  // {
  //   path: '/system',
  //   name: 'system',
  //   component: () => import('pages/SystemSettings.vue')
  // },
  {
    path: '/funcs',
    component: () => import('pages/FuncsView.vue')
  },
  {
    path: '/ShareWorld',
    name: 'sw',
    component: () => import('pages/ShareWorld.vue')
  },
  {
    path: '/PortMapping',
    name: 'apm',
    component: () => import('pages/AutoPortMapping.vue')
  },
  {
    path: '/credit',
    name: 'credit',
    component: () => import('pages/CreditPage.vue')
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
