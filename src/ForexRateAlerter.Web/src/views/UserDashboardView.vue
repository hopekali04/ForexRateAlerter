<template>
  <div class="min-h-full bg-blueprint-bg">
    <!-- Market Ticker -->
    <MarketTicker />

    <!-- Main Content -->
    <div class="md:pt-0 pt-16 p-4 md:p-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="font-sans text-2xl font-bold text-blueprint-text uppercase tracking-wide mb-2">Forex Alert Dashboard</h1>
        <div class="flex items-center gap-4">
          <div class="flex items-center">
            <div class="w-2 h-2 bg-blueprint-primary mr-2"></div>
            <span class="font-sans text-xs text-blueprint-text-secondary">SYSTEM ONLINE</span>
          </div>
          <div class="border-l border-blueprint-border h-4"></div>
          <span class="font-mono text-xs text-blueprint-text-secondary">{{ currentTime }}</span>
        </div>
      </div>

      <!-- Dashboard Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Currency Strength (Takes 1 column) -->
        <div class="lg:col-span-1">
          <CurrencyStrength @select-pair="handleSelectPair" />
        </div>

        <!-- Active Alerts List (Takes 2 columns) -->
        <div class="lg:col-span-2">
          <div class="bg-blueprint-surface border border-blueprint-border">
            <!-- Header -->
            <div class="border-b border-blueprint-border px-6 py-4 flex justify-between items-center">
              <div>
                <h3 class="font-sans text-sm font-bold text-blueprint-text uppercase tracking-wide">Active Alerts</h3>
                <p class="font-sans text-xs text-blueprint-text-secondary mt-1">{{ alerts.length }} monitoring</p>
              </div>
              <button 
                @click="loadAlerts"
                class="px-3 py-2 border border-blueprint-border hover:bg-blueprint-text hover:text-white transition-colors"
                title="Refresh"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
                </svg>
              </button>
            </div>

            <!-- Alerts Table -->
            <div v-if="alerts.length > 0" class="overflow-x-auto">
              <table class="w-full">
                <thead class="bg-blueprint-bg border-b border-blueprint-border">
                  <tr>
                    <th class="px-6 py-3 text-left font-sans text-xs font-bold text-blueprint-text uppercase">Pair</th>
                    <th class="px-6 py-3 text-left font-sans text-xs font-bold text-blueprint-text uppercase">Condition</th>
                    <th class="px-6 py-3 text-left font-sans text-xs font-bold text-blueprint-text uppercase">Target Rate</th>
                    <th class="px-6 py-3 text-left font-sans text-xs font-bold text-blueprint-text uppercase">Status</th>
                    <th class="px-6 py-3 text-right font-sans text-xs font-bold text-blueprint-text uppercase">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr 
                    v-for="alert in alerts" 
                    :key="alert.id"
                    class="border-b border-blueprint-border hover:bg-blueprint-bg transition-colors"
                  >
                    <td class="px-6 py-4 font-mono text-sm font-bold text-blueprint-text">
                      {{ alert.baseCurrency }}/{{ alert.targetCurrency }}
                    </td>
                    <td class="px-6 py-4 font-sans text-sm text-blueprint-text">
                      {{ formatCondition(alert.condition, 'symbol') }}
                    </td>
                    <td class="px-6 py-4 font-mono text-sm font-bold text-blueprint-text">
                      {{ formatRate(alert.targetRate) }}
                    </td>
                    <td class="px-6 py-4">
                      <div class="flex items-center">
                        <div class="w-2 h-2 bg-blueprint-primary mr-2"></div>
                        <span class="font-sans text-xs text-blueprint-text-secondary uppercase">Active</span>
                      </div>
                    </td>
                    <td class="px-6 py-4 text-right">
                      <button 
                        @click="handleDeleteAlert(alert.id)"
                        class="px-3 py-2 border border-blueprint-border text-blueprint-text hover:bg-blueprint-error hover:text-white hover:border-blueprint-error transition-colors"
                        title="Delete"
                      >
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                        </svg>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Empty State -->
            <div v-else class="p-12 text-center">
              <div class="w-16 h-16 border border-blueprint-border bg-blueprint-bg mx-auto mb-4 flex items-center justify-center">
                <svg class="w-8 h-8 text-blueprint-text-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="1.5" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
                </svg>
              </div>
              <p class="font-sans text-sm font-bold text-blueprint-text mb-2">NO ALERTS CONFIGURED</p>
              <p class="font-sans text-xs text-blueprint-text-secondary">Click "NEW ALERT" to start monitoring</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Statistics Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mt-6">
        <div class="bg-blueprint-surface border border-blueprint-border p-6">
          <p class="font-sans text-xs font-bold text-blueprint-text-secondary uppercase mb-2">Total Alerts</p>
          <p class="font-mono text-3xl font-bold text-blueprint-text">{{ alerts.length }}</p>
        </div>
        <div class="bg-blueprint-surface border border-blueprint-border p-6">
          <p class="font-sans text-xs font-bold text-blueprint-text-secondary uppercase mb-2">Triggered Today</p>
          <p class="font-mono text-3xl font-bold text-blueprint-primary">0</p>
        </div>
        <div class="bg-blueprint-surface border border-blueprint-border p-6">
          <p class="font-sans text-xs font-bold text-blueprint-text-secondary uppercase mb-2">Monitoring Since</p>
          <p class="font-mono text-sm font-bold text-blueprint-text">{{ joinDate }}</p>
        </div>
      </div>

      <!-- Interactive Forex Chart -->
      <div class="mt-6">
        <ForexChart />
      </div>
    </div>

    <!-- Alert Form Modal -->
    <AlertForm 
      :is-open="isModalOpen"
      :prefill-pair="selectedPair"
      @close="closeAlertModal"
      @submit="handleCreateAlert"
    />

    <!-- Toast Notification -->
    <Toast 
      :show="toast.show"
      :message="toast.message"
      :type="toast.type"
      @close="toast.show = false"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { createAlert, getUserAlerts, deleteAlert } from '@/services/alertService';
import MarketTicker from '@/components/MarketTicker.vue';
import CurrencyStrength from '@/components/CurrencyStrength.vue';
import AlertForm from '@/components/AlertForm.vue';
import Toast from '@/components/Toast.vue';
import ForexChart from '@/components/ForexChart.vue';
import { formatRate, formatCondition } from '@/utils/formatters';

interface Alert {
  id: string | number;
  baseCurrency: string;
  targetCurrency: string;
  condition: string;
  targetRate: number;
}

interface ToastState {
  show: boolean;
  message: string;
  type: 'success' | 'error' | 'info';
}

const router = useRouter();
const authStore = useAuthStore();

const alerts = ref<Alert[]>([]);
const isModalOpen = ref(false);
const selectedPair = ref('');
const currentTime = ref('');
const toast = ref<ToastState>({
  show: false,
  message: '',
  type: 'info',
});

const joinDate = computed(() => {
  return new Date().toLocaleDateString('en-US', { year: 'numeric', month: 'short' });
});

const updateTime = () => {
  const now = new Date();
  currentTime.value = now.toLocaleTimeString('en-US', { 
    hour: '2-digit', 
    minute: '2-digit',
    second: '2-digit',
    hour12: false 
  }) + ' UTC';
};

let timeInterval: number | null = null;

onMounted(() => {
  loadAlerts();
  updateTime();
  timeInterval = window.setInterval(updateTime, 1000);
});

onUnmounted(() => {
  if (timeInterval) {
    clearInterval(timeInterval);
  }
});

const showToast = (message: string, type: 'success' | 'error' | 'info' = 'info') => {
  toast.value = { show: true, message, type };
};

const handleSelectPair = (pair: string) => {
  selectedPair.value = pair;
  isModalOpen.value = true;
};

const openAlertModal = () => {
  selectedPair.value = '';
  isModalOpen.value = true;
};

const closeAlertModal = () => {
  isModalOpen.value = false;
  selectedPair.value = '';
};

const mapCondition = (condition: string | number): 'GreaterThan' | 'LessThan' | 'EqualTo' => {
  const conditionStr = String(condition);
  const conditionMap: Record<string, 'GreaterThan' | 'LessThan' | 'EqualTo'> = {
    '1': 'GreaterThan',
    '2': 'LessThan',
    '3': 'EqualTo',
    'GreaterThan': 'GreaterThan',
    'LessThan': 'LessThan',
    'EqualTo': 'EqualTo',
  };
  return conditionMap[conditionStr] || 'GreaterThan';
};

const handleCreateAlert = async (formData: any) => {
  try {
    await createAlert({
      baseCurrency: formData.baseCurrency,
      targetCurrency: formData.targetCurrency,
      condition: mapCondition(formData.condition),
      targetRate: parseFloat(formData.targetRate),
    });
    
    showToast(
      `Alert set for ${formData.baseCurrency}/${formData.targetCurrency} ${formatCondition(formData.condition, 'symbol')} ${formData.targetRate}`,
      'success'
    );
    
    loadAlerts();
  } catch (error) {
    console.error('Failed to create alert:', error);
    showToast('Failed to create alert. Please try again.', 'error');
  }
};

const handleDeleteAlert = async (id: string | number) => {
  try {
    await deleteAlert(Number(id));
    showToast('Alert deleted successfully', 'success');
    loadAlerts();
  } catch (error) {
    console.error('Failed to delete alert:', error);
    showToast('Failed to delete alert. Please try again.', 'error');
  }
};

const loadAlerts = async () => {
  try {
    const data = await getUserAlerts();
    alerts.value = data.alerts;
  } catch (error) {
    console.error('Failed to load alerts:', error);
    showToast('Failed to load alerts', 'error');
  }
};
</script>
