<template>
  <div class="min-h-screen bg-slate-50 font-sans">
    <!-- Navigation Bar with Glass Morphism -->
    <nav v-if="authStore.token" class="fixed top-0 left-0 right-0 z-50 bg-white border-b border-solid border-gray-300">
      <div class="px-6 mx-auto max-w-7xl lg:px-8">
        <div class="flex items-center justify-between h-16">
          <div class="flex items-center">
            <!-- Logo with rectangular design -->
            <div class="flex-shrink-0">
              <RouterLink to="/" class="relative group">
                <div class="w-9 h-9 bg-green-500 border border-solid border-gray-300 flex items-center justify-center group-hover:bg-green-600 transition-colors duration-200">
                  <div class="w-5 h-5 bg-white"></div>
                </div>
              </RouterLink>
            </div>
            
            <!-- Desktop Navigation Items -->
            <div class="hidden md:block">
              <div class="flex items-center ml-8 space-x-1">
                <RouterLink
                  v-for="item in navigation"
                  :key="item.name"
                  :to="item.href"
                  :class="[
                    item.current 
                      ? 'bg-gray-100 text-slate-900 border border-solid border-gray-300' 
                      : 'text-gray-700 hover:bg-gray-50 hover:text-slate-900',
                    'px-4 py-2.5 text-sm font-medium transition-all duration-200 ease-out'
                  ]"
                  :aria-current="item.current ? 'page' : undefined"
                >
                  {{ item.name }}
                </RouterLink>
              </div>
            </div>
          </div>
          
          <!-- Desktop User Actions -->
          <div class="hidden md:block">
            <button 
              @click="handleLogout" 
              class="inline-flex items-center px-4 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50 hover:text-slate-900 transition-all duration-200 ease-out border border-solid border-transparent hover:border-gray-300"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
              </svg>
              Sign out
            </button>
          </div>

          <!-- Mobile menu button -->
          <div class="md:hidden">
            <button
              @click="mobileMenuOpen = !mobileMenuOpen"
              class="inline-flex items-center justify-center p-2 text-gray-700 hover:text-slate-900 hover:bg-gray-50 focus:outline-none border border-solid border-gray-300 transition-all duration-200"
            >
              <span class="sr-only">Open main menu</span>
              <svg 
                v-if="!mobileMenuOpen" 
                class="block h-6 w-6" 
                fill="none" 
                viewBox="0 0 24 24" 
                stroke="currentColor"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
              <svg 
                v-else 
                class="block h-6 w-6" 
                fill="none" 
                viewBox="0 0 24 24" 
                stroke="currentColor"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Mobile menu -->
      <Transition
        enter-active-class="transition duration-200 ease-out"
        enter-from-class="transform scale-95 opacity-0"
        enter-to-class="transform scale-100 opacity-100"
        leave-active-class="transition duration-150 ease-in"
        leave-from-class="transform scale-100 opacity-100"
        leave-to-class="transform scale-95 opacity-0"
      >
        <div v-if="mobileMenuOpen" class="md:hidden">
          <div class="px-6 pt-2 pb-3 space-y-1 bg-white border-t border-solid border-gray-300">
            <RouterLink
              v-for="item in navigation"
              :key="item.name"
              :to="item.href"
              @click="mobileMenuOpen = false"
              :class="[
                item.current 
                  ? 'bg-gray-100 text-slate-900 border border-solid border-gray-300' 
                  : 'text-gray-700 hover:bg-gray-50 hover:text-slate-900',
                'block px-4 py-3 text-base font-medium transition-all duration-200 ease-out'
              ]"
            >
              {{ item.name }}
            </RouterLink>
            <button
              @click="handleLogout"
              class="w-full text-left block px-4 py-3 text-base font-medium text-gray-700 hover:bg-gray-50 hover:text-slate-900 transition-all duration-200 ease-out border border-solid border-transparent hover:border-gray-300"
            >
              Sign out
            </button>
          </div>
        </div>
      </Transition>
    </nav>

    <!-- Main Content Area -->
    <main :class="authStore.token ? 'pt-16' : ''">
      <RouterView />
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { RouterLink, RouterView, useRouter, useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
const router = useRouter();
const route = useRoute();
const mobileMenuOpen = ref(false);

const userNavigation = [
  { name: 'Dashboard', href: '/dashboard' },
  { name: 'Alerts', href: '/alerts' },
  { name: 'Rates', href: '/rates' },
];

const adminNavigation = [
  { name: 'Dashboard', href: '/admin' },
  { name: 'Users', href: '/admin/users' },
  { name: 'Alerts', href: '/admin/alerts' },
  { name: 'Rates', href: '/admin/rates' },
];

const navigation = computed(() => {
  const nav = authStore.user?.role === 'Admin' ? adminNavigation : userNavigation;
  return nav.map(item => ({
    ...item,
    current: route.path === item.href
  }));
});

const handleLogout = () => {
  authStore.logout();
  mobileMenuOpen.value = false;
  router.push('/login');
};
</script>
