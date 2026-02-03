import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/HomeView.vue')
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import('../views/UserDashboardView.vue'),
      meta: { requiresAuth: true, roles: ['User'] }
    },
    {
      path: '/alerts',
      name: 'alerts',
      component: () => import('../views/AlertsView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/rates',
      name: 'rates',
      component: () => import('../views/RatesView.vue'),
      meta: { requiresAuth: true}
    },
    {
      path: '/admin',
      name: 'admin',
      component: () => import('../views/AdminDashboardView.vue'),
      meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
      path: '/admin/rates',
      name: 'admin.rates',
      component: () => import('../views/RatesView.vue'),
      meta: { requiresAuth: true}
    },
    {
      path: '/admin/alerts',
      name: 'admin.alerts',
      component: () => import('../views/AlertsView.vue'),
      meta: { requiresAuth: true, roles: ['Admin'] }
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('@/views/NotFoundView.vue'),  
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  // Only redirect authenticated users away from login/register IF coming from home
  if ((to.name === 'login' || to.name === 'register') && authStore.token) {
    next('/dashboard')
    return
  }
  
  // Redirect unauthenticated users away from protected routes
  if (to.meta.requiresAuth && !authStore.token) {
    next('/login')
    return
  }
  
  // For protected routes with role requirements, validate user role
  if (to.meta.requiresAuth && to.meta.roles && authStore.token && authStore.user) {
    const roles = to.meta.roles as string[];
    if (!roles.includes(authStore.user.role)) {
      if (authStore.user.role === 'Admin') {
        next('/admin')
      } else {
        next('/dashboard')
      }
      return
    }
  }
  
  // Allow navigation
  next()
})

export default router
