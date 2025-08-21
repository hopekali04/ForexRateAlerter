<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-indigo-50/50">
    <div class="px-6 py-8 mx-auto max-w-7xl lg:px-8">
      <!-- Header Section -->
      <div class="mb-12">
        <div class="text-center md:text-left">
          <h1 class="text-4xl font-bold tracking-tight text-gray-900 mb-3">
            <span class="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
              Your Alerts
            </span>
          </h1>
          <p class="text-lg text-gray-600 max-w-2xl">
            Monitor forex rates and get notified when your target conditions are met
          </p>
        </div>
      </div>

      <!-- Create Alert Card -->
      <div class="mb-12 animate-slide-up">
        <div class="bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-8">
          <div class="flex items-center mb-6">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 shadow-lg shadow-blue-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
              </svg>
            </div>
            <div>
              <h2 class="text-2xl font-bold text-gray-900">Create New Alert</h2>
              <p class="text-gray-600">Set up a new forex rate alert to monitor your preferred currency pairs</p>
            </div>
          </div>
          
          <form @submit.prevent="handleCreateAlert" class="space-y-6">
            <div class="grid grid-cols-1 gap-6 lg:grid-cols-2 xl:grid-cols-4">
              <!-- Base Currency -->
              <div class="space-y-2">
                <label class="block text-sm font-semibold text-gray-700">Base Currency</label>
                <input 
                  v-model="newAlert.baseCurrency" 
                  placeholder="USD"
                  class="block w-full px-4 py-4 text-gray-900 placeholder-gray-500 bg-white/70 border border-gray-200/50 rounded-2xl backdrop-blur-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ease-out"
                  required 
                />
              </div>

              <!-- Target Currency -->
              <div class="space-y-2">
                <label class="block text-sm font-semibold text-gray-700">Target Currency</label>
                <input 
                  v-model="newAlert.targetCurrency" 
                  placeholder="EUR"
                  class="block w-full px-4 py-4 text-gray-900 placeholder-gray-500 bg-white/70 border border-gray-200/50 rounded-2xl backdrop-blur-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ease-out"
                  required 
                />
              </div>

              <!-- Condition -->
              <div class="space-y-2">
                <label class="block text-sm font-semibold text-gray-700">Condition</label>
                <select 
                  v-model="newAlert.condition" 
                  class="block w-full px-4 py-4 text-gray-900 bg-white/70 border border-gray-200/50 rounded-2xl backdrop-blur-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ease-out"
                  required
                >
                  <option value="1">Greater Than</option>
                  <option value="2">Less Than</option>
                  <option value="3">Equal To</option>
                </select>
              </div>

              <!-- Target Rate -->
              <div class="space-y-2">
                <label class="block text-sm font-semibold text-gray-700">Target Rate</label>
                <input 
                  v-model="newAlert.targetRate" 
                  type="number" 
                  step="0.0001" 
                  placeholder="1.0500"
                  class="block w-full px-4 py-4 text-gray-900 placeholder-gray-500 bg-white/70 border border-gray-200/50 rounded-2xl backdrop-blur-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all duration-200 ease-out"
                  required 
                />
              </div>
            </div>

            <!-- Submit Button -->
            <div class="flex justify-end">
              <button 
                type="submit"
                :disabled="isCreating"
                class="group relative inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-white bg-gradient-to-r from-blue-600 to-indigo-600 rounded-2xl shadow-apple-lg hover:shadow-apple-xl hover:scale-105 active:scale-95 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 ease-out"
              >
                <span v-if="!isCreating" class="relative z-10 flex items-center">
                  <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
                  </svg>
                  Create Alert
                </span>
                <span v-else class="relative z-10 flex items-center">
                  <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Creating...
                </span>
                <div class="absolute inset-0 rounded-2xl bg-gradient-to-r from-blue-700 to-indigo-700 opacity-0 transition-opacity duration-200 group-hover:opacity-100"></div>
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Active Alerts Section -->
      <div class="animate-fade-in">
        <div class="flex items-center justify-between mb-8">
          <div>
            <h2 class="text-2xl font-bold text-gray-900">Active Alerts</h2>
            <p class="text-gray-600">{{ alerts.length }} {{ alerts.length === 1 ? 'alert' : 'alerts' }} currently monitoring</p>
          </div>
        </div>

        <!-- Alerts Grid -->
        <div v-if="alerts.length > 0" class="grid grid-cols-1 gap-6 md:grid-cols-2 xl:grid-cols-3">
          <div 
            v-for="alert in alerts" 
            :key="alert.id" 
            class="group bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-6 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out"
          >
            <!-- Alert Header -->
            <div class="flex items-start justify-between mb-4">
              <div class="flex items-center">
                <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-green-500 to-emerald-600 shadow-lg shadow-green-500/25 mr-3">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
                  </svg>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-gray-900">{{ alert.baseCurrency }}/{{ alert.targetCurrency }}</h3>
                  <p class="text-sm text-gray-500">Currency Pair</p>
                </div>
              </div>
              
              <!-- Delete Button -->
              <button 
                @click="handleDeleteAlert(alert.id)"
                class="group/btn p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-xl transition-all duration-200 ease-out"
                title="Delete Alert"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                </svg>
              </button>
            </div>

            <!-- Alert Details -->
            <div class="space-y-3">
              <div class="flex items-center justify-between p-3 bg-gray-50/50 rounded-xl">
                <span class="text-sm font-medium text-gray-600">Condition</span>
                <span class="text-sm font-semibold text-gray-900">{{ getConditionText(alert.condition) }}</span>
              </div>
              
              <div class="flex items-center justify-between p-3 bg-gray-50/50 rounded-xl">
                <span class="text-sm font-medium text-gray-600">Target Rate</span>
                <span class="text-lg font-bold text-blue-600">{{ formatRate(alert.targetRate) }}</span>
              </div>
            </div>

            <!-- Status Indicator -->
            <div class="mt-4 flex items-center">
              <div class="w-2 h-2 bg-green-500 rounded-full mr-2 animate-pulse"></div>
              <span class="text-xs font-medium text-gray-600">Active & Monitoring</span>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="text-center py-16">
          <div class="mx-auto w-24 h-24 rounded-3xl bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center mb-6">
            <svg class="w-12 h-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4m-6 2a9 9 0 110-18 9 9 0 010 18z"/>
            </svg>
          </div>
          <h3 class="text-xl font-semibold text-gray-900 mb-2">No alerts yet</h3>
          <p class="text-gray-600 max-w-md mx-auto">
            Create your first alert above to start monitoring forex rates and get notified when your conditions are met.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { createAlert, getUserAlerts, deleteAlert } from '@/services/alertService';

interface Alert {
  id: string | number;
  baseCurrency: string;
  targetCurrency: string;
  condition: string;
  targetRate: number;
}

const newAlert = ref({
  baseCurrency: '',
  targetCurrency: '',
  condition: 1,
  targetRate: null,
});

const alerts = ref<Alert[]>([]);
const isCreating = ref(false);

const handleCreateAlert = async () => {
  if (isCreating.value) return;
  
  try {
    isCreating.value = true;
    await createAlert(newAlert.value);
    // Reset form
    newAlert.value = {
      baseCurrency: '',
      targetCurrency: '',
      condition: 1,
      targetRate: null,
    };
    loadAlerts();
  } catch (error) {
    console.error('Failed to create alert:', error);
  } finally {
    isCreating.value = false;
  }
};

const handleDeleteAlert = async (id: string | number) => {
  try {
    await deleteAlert(Number(id));
    loadAlerts();
  } catch (error) {
    console.error('Failed to delete alert:', error);
  }
};

const loadAlerts = async () => {
  try {
    const data = await getUserAlerts();
    alerts.value = data.alerts;
  } catch (error) {
    console.error('Failed to load alerts:', error);
  }
};

const getConditionText = (condition: string): string => {
  const conditionMap: Record<string, string> = {
    '1': 'Greater Than',
    '2': 'Less Than',
    '3': 'Equal To',
  };
  return conditionMap[condition] || 'Unknown';
};

const formatRate = (rate: number): string => {
  return rate?.toFixed(4) || '0.0000';
};

onMounted(() => {
  loadAlerts();
});
</script>
