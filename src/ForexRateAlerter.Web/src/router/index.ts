import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import UserDashboardView from '../views/UserDashboardView.vue'
import AdminDashboardView from '../views/AdminDashboardView.vue'
import NotFoundView from '@/views/NotFoundView.vue'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: UserDashboardView,
      meta: { requiresAuth: true, roles: ['User'] }
    },
    {
      path: '/admin',
      name: 'admin',
      component: AdminDashboardView,
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
      component: NotFoundView,  
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  
  // Only redirect authenticated users away from login/register IF coming from home
  if ((to.name === 'login' || to.name === 'register') && authStore.token && from.name === 'home') {
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
    if (!to.meta.roles.includes(authStore.user.role)) {
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
