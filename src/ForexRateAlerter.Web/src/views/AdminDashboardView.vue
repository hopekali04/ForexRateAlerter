<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-indigo-50/50">
    <div class="px-6 py-8 mx-auto max-w-7xl lg:px-8">
      <!-- Header Section -->
      <div class="mb-12">
        <div class="text-center md:text-left">
          <h1 class="text-4xl font-bold tracking-tight text-gray-900 mb-3">
            <span class="bg-gradient-to-r from-purple-600 to-indigo-600 bg-clip-text text-transparent">
              Admin Dashboard
            </span>
          </h1>
          <p class="text-lg text-gray-600 max-w-2xl">
            Monitor system performance, user activity, and forex alert analytics
          </p>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="grid grid-cols-1 gap-6 mb-12 md:grid-cols-2 lg:grid-cols-4 animate-slide-up">
        <!-- Total Users -->
        <div class="group bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-6 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 shadow-lg shadow-blue-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Total Users</h2>
              <p class="text-3xl font-bold text-gray-900">{{ formatNumber(stats.totalUsers) }}</p>
            </div>
          </div>
        </div>

        <!-- Total Alerts -->
        <div class="group bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-6 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-green-500 to-emerald-600 shadow-lg shadow-green-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4m-6 2a9 9 0 110-18 9 9 0 010 18z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Total Alerts</h2>
              <p class="text-3xl font-bold text-gray-900">{{ formatNumber(stats.totalAlerts) }}</p>
            </div>
          </div>
        </div>

        <!-- Alerts Today -->
        <div class="group bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-6 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-yellow-500 to-orange-600 shadow-lg shadow-yellow-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Alerts Today</h2>
              <p class="text-3xl font-bold text-gray-900">{{ formatNumber(stats.alertsTriggeredToday) }}</p>
            </div>
          </div>
        </div>

        <!-- Alerts This Week -->
        <div class="group bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-6 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-purple-500 to-pink-600 shadow-lg shadow-purple-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Weekly Alerts</h2>
              <p class="text-3xl font-bold text-gray-900">{{ formatNumber(stats.alertsTriggeredThisWeek) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Popular Currency Pairs Section -->
      <div class="animate-fade-in">
        <div class="flex items-center mb-8">
          <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-indigo-500 to-purple-600 shadow-lg shadow-indigo-500/25 mr-4">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
            </svg>
          </div>
          <div>
            <h2 class="text-2xl font-bold text-gray-900">Popular Currency Pairs</h2>
            <p class="text-gray-600">Most monitored currency pairs across all alerts</p>
          </div>
        </div>

        <div class="bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-8">
          <div v-if="stats.mostPopularCurrencyPairs.length > 0" class="space-y-4">
            <div 
              v-for="(pair, index) in stats.mostPopularCurrencyPairs" 
              :key="pair.currencyPair" 
              class="flex items-center justify-between p-4 bg-gray-50/50 rounded-2xl hover:bg-gray-100/50 transition-all duration-200 ease-out"
            >
              <div class="flex items-center">
                <div class="flex items-center justify-center w-10 h-10 rounded-xl bg-gradient-to-br from-gray-100 to-gray-200 text-gray-600 font-bold mr-4">
                  {{ index + 1 }}
                </div>
                <div>
                  <h3 class="text-lg font-bold text-gray-900">{{ pair.currencyPair }}</h3>
                  <p class="text-sm text-gray-600">Currency Pair</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-2xl font-bold text-blue-600">{{ formatNumber(pair.count) }}</p>
                <p class="text-sm text-gray-600">{{ pair.count === 1 ? 'alert' : 'alerts' }}</p>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-else class="text-center py-12">
            <div class="mx-auto w-16 h-16 rounded-2xl bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center mb-4">
              <svg class="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
              </svg>
            </div>
            <h3 class="text-lg font-semibold text-gray-900 mb-2">No data available</h3>
            <p class="text-gray-600">Currency pair statistics will appear here once alerts are created.</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { getStatistics } from '@/services/adminService';

interface CurrencyPairStat {
  currencyPair: string;
  count: number;
}

const stats = ref({
  totalUsers: 0,
  totalAlerts: 0,
  alertsTriggeredToday: 0,
  alertsTriggeredThisWeek: 0,
  mostPopularCurrencyPairs: [] as CurrencyPairStat[],
});

const loadStats = async () => {
  try {
    const data = await getStatistics();
    stats.value = data;
  } catch (error) {
    console.error('Failed to load statistics:', error);
  }
};

const formatNumber = (num: number): string => {
  return new Intl.NumberFormat().format(num);
};

onMounted(() => {
  loadStats();
});
</script>
