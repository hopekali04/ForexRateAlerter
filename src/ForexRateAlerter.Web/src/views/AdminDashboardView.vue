<template>
  <div class="min-h-screen bg-slate-50">
    <div class="px-6 py-8 mx-auto max-w-7xl lg:px-8">
      <!-- Header Section -->
      <div class="mb-8">
        <div class="text-center md:text-left">
          <h1 class="text-3xl font-bold tracking-tight text-slate-900 mb-2">
            Admin Dashboard
          </h1>
          <p class="text-base text-gray-600 max-w-2xl">
            Monitor system performance, user activity, and forex alert analytics
          </p>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="grid grid-cols-1 gap-4 mb-8 md:grid-cols-2 lg:grid-cols-4">
        <!-- Total Users -->
        <div class="bg-white border border-solid border-gray-300 p-6">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-10 h-10 bg-green-500 border border-solid border-gray-300 mr-4">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Total Users</h2>
              <p class="text-2xl font-mono font-bold text-slate-900">{{ formatNumber(stats.totalUsers) }}</p>
            </div>
          </div>
        </div>

        <!-- Total Alerts -->
        <div class="bg-white border border-solid border-gray-300 p-6">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-10 h-10 bg-green-500 border border-solid border-gray-300 mr-4">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4m-6 2a9 9 0 110-18 9 9 0 010 18z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Total Alerts</h2>
              <p class="text-2xl font-mono font-bold text-slate-900">{{ formatNumber(stats.totalAlerts) }}</p>
            </div>
          </div>
        </div>

        <!-- Alerts Today -->
        <div class="bg-white border border-solid border-gray-300 p-6">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-10 h-10 bg-green-500 border border-solid border-gray-300 mr-4">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Alerts Today</h2>
              <p class="text-2xl font-mono font-bold text-slate-900">{{ formatNumber(stats.alertsTriggeredToday) }}</p>
            </div>
          </div>
        </div>

        <!-- Alerts This Week -->
        <div class="bg-white border border-solid border-gray-300 p-6">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-10 h-10 bg-green-500 border border-solid border-gray-300 mr-4">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Weekly Alerts</h2>
              <p class="text-2xl font-mono font-bold text-slate-900">{{ formatNumber(stats.alertsTriggeredThisWeek) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Popular Currency Pairs Section -->
      <div>
        <div class="flex items-center mb-6">
          <div class="flex items-center justify-center w-10 h-10 bg-green-500 border border-solid border-gray-300 mr-4">
            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
            </svg>
          </div>
          <div>
            <h2 class="text-xl font-bold text-slate-900">Popular Currency Pairs</h2>
            <p class="text-gray-600">Most monitored currency pairs across all alerts</p>
          </div>
        </div>

        <div class="bg-white border border-solid border-gray-300 p-6">
          <div v-if="stats.mostPopularCurrencyPairs.length > 0" class="space-y-2">
            <div 
              v-for="(pair, index) in stats.mostPopularCurrencyPairs" 
              :key="pair.currencyPair" 
              class="flex items-center justify-between p-3 bg-gray-50 border border-solid border-gray-200 hover:bg-gray-100 transition-colors duration-200"
            >
              <div class="flex items-center">
                <div class="flex items-center justify-center w-8 h-8 bg-gray-200 border border-solid border-gray-300 text-gray-600 text-sm font-bold mr-3">
                  {{ index + 1 }}
                </div>
                <div>
                  <h3 class="text-lg font-mono font-bold text-slate-900">{{ pair.currencyPair }}</h3>
                  <p class="text-sm text-gray-600">Currency Pair</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-xl font-mono font-bold text-green-600">{{ formatNumber(pair.count) }}</p>
                <p class="text-sm text-gray-600">{{ pair.count === 1 ? 'alert' : 'alerts' }}</p>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-else class="text-center py-8">
            <div class="mx-auto w-12 h-12 bg-gray-200 border border-solid border-gray-300 flex items-center justify-center mb-4">
              <svg class="w-6 h-6 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
              </svg>
            </div>
            <h3 class="text-lg font-semibold text-slate-900 mb-2">No data available</h3>
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
