<template>
  <div class="min-h-screen bg-slate-50 font-sans">
    <!-- Authenticated Layout: Sidebar + Content -->
    <div v-if="authStore.token" class="flex h-screen">
      <!-- Desktop Sidebar -->
      <aside class="hidden md:flex md:w-64 bg-blueprint-surface border-r border-blueprint-border flex-col flex-shrink-0">
        <!-- User Section -->
        <div class="border-b border-blueprint-border p-6">
          <div class="flex items-center">
            <div class="w-10 h-10 border border-blueprint-border bg-blueprint-bg flex items-center justify-center">
              <span class="font-mono text-sm font-bold text-blueprint-text">{{ userInitials }}</span>
            </div>
            <div class="ml-3">
              <p class="font-sans text-xs font-bold text-blueprint-text">{{ authStore.user?.email?.split('@')[0] || 'USER' }}</p>
              <p class="font-sans text-xs text-blueprint-text-secondary">Active Session</p>
            </div>
          </div>
        </div>

        <!-- Navigation -->
        <nav class="flex-1 p-4">
          <div class="space-y-1">
            <RouterLink
              v-for="item in navigation"
              :key="item.href"
              :to="item.href"
              :class="[
                item.current
                  ? 'bg-blueprint-primary text-white'
                  : 'text-blueprint-text hover:bg-blueprint-text hover:text-white',
                'block w-full text-left px-4 py-3 border border-blueprint-border font-sans text-xs font-bold uppercase transition-colors'
              ]"
            >
              {{ item.name }}
            </RouterLink>
          </div>
        </nav>

        <!-- Action Buttons -->
        <div class="p-4 border-t border-blueprint-border space-y-2">
          <button 
            @click="handleLogout"
            class="w-full px-4 py-3 border border-blueprint-border bg-blueprint-surface text-blueprint-text font-sans text-xs font-bold uppercase hover:bg-blueprint-error hover:text-white hover:border-blueprint-error transition-colors"
          >
            LOGOUT
          </button>
        </div>
      </aside>

      <!-- Mobile Sidebar Toggle -->
      <div class="md:hidden fixed top-4 left-4 z-50">
        <button
          @click="mobileSidebarOpen = !mobileSidebarOpen"
          class="p-2 border border-blueprint-border bg-white hover:bg-blueprint-bg transition-colors"
          title="Toggle menu"
        >
          <svg class="w-5 h-5 text-blueprint-text" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>
      </div>

      <!-- Mobile Sidebar Overlay -->
      <Transition name="sidebar">
        <div v-if="mobileSidebarOpen" class="md:hidden fixed inset-0 bg-black bg-opacity-50 z-40" @click="mobileSidebarOpen = false"></div>
      </Transition>

      <!-- Mobile Sidebar -->
      <Transition name="sidebar">
        <aside v-if="mobileSidebarOpen" class="md:hidden fixed left-0 top-0 h-full w-64 bg-blueprint-surface border-r border-blueprint-border flex flex-col z-40 overflow-y-auto">
          <!-- User Section -->
          <div class="border-b border-blueprint-border p-6">
            <div class="flex items-center">
              <div class="w-10 h-10 border border-blueprint-border bg-blueprint-bg flex items-center justify-center">
                <span class="font-mono text-sm font-bold text-blueprint-text">{{ userInitials }}</span>
              </div>
              <div class="ml-3">
                <p class="font-sans text-xs font-bold text-blueprint-text">{{ authStore.user?.email?.split('@')[0] || 'USER' }}</p>
                <p class="font-sans text-xs text-blueprint-text-secondary">Active Session</p>
              </div>
            </div>
          </div>

          <!-- Navigation -->
          <nav class="flex-1 p-4">
            <div class="space-y-1">
              <RouterLink
                v-for="item in navigation"
                :key="item.href"
                :to="item.href"
                @click="mobileSidebarOpen = false"
                :class="[
                  item.current
                    ? 'bg-blueprint-primary text-white'
                    : 'text-blueprint-text hover:bg-blueprint-text hover:text-white',
                  'block w-full text-left px-4 py-3 border border-blueprint-border font-sans text-xs font-bold uppercase transition-colors'
                ]"
              >
                {{ item.name }}
              </RouterLink>
            </div>
          </nav>

          <!-- Action Buttons -->
          <div class="p-4 border-t border-blueprint-border space-y-2">
            <button 
              @click="handleLogout"
              class="w-full px-4 py-3 border border-blueprint-border bg-blueprint-surface text-blueprint-text font-sans text-xs font-bold uppercase hover:bg-blueprint-error hover:text-white hover:border-blueprint-error transition-colors"
            >
              LOGOUT
            </button>
          </div>
        </aside>
      </Transition>

      <!-- Main Content Area -->
      <main class="flex-1 overflow-auto">
        <RouterView />
      </main>
    </div>

    <!-- Unauthenticated Layout -->
    <main v-else>
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
const mobileSidebarOpen = ref(false);

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

const userInitials = computed(() => {
  const email = authStore.user?.email || 'U';
  return email.substring(0, 2).toUpperCase();
});

const handleLogout = () => {
  authStore.logout();
  mobileSidebarOpen.value = false;
  router.push('/login');
};
</script>

<style scoped>
.sidebar-enter-active,
.sidebar-leave-active {
  transition: all 0.3s ease;
}

.sidebar-enter-from {
  transform: translateX(-100%);
  opacity: 0;
}

.sidebar-leave-to {
  transform: translateX(-100%);
  opacity: 0;
}
</style>
