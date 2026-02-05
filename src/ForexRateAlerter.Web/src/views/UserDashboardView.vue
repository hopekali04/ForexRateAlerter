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
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
        <!-- Currency Strength (Takes 2 column) -->
        <div class="lg:col-span-2">
          <CurrencyStrength 
            @select-pair="handleSelectPair"
            @view-details="handleViewDetails"
          />
        </div>

        <!-- Active Alerts List (Takes 1 columns) -->
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

    <!-- Details Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div 
          v-if="isDetailsModalOpen"
          class="fixed inset-0 z-50 flex items-center justify-center bg-blueprint-text bg-opacity-80"
          @click.self="closeDetailsModal"
        >
          <!-- Modal Container -->
          <div class="bg-white border-2 border-blueprint-border w-full max-w-lg mx-4">
            <!-- Header -->
            <div class="border-b border-blueprint-border px-6 py-4 flex justify-between items-center bg-white">
              <div>
                <h2 class="font-sans text-lg font-bold text-blueprint-text uppercase">
                  Rate Information
                </h2>
                <p class="font-sans text-xs text-blueprint-text-secondary mt-1">
                  DETAILED EXCHANGE RATE DATA
                </p>
              </div>
              <button 
                @click="closeDetailsModal"
                class="text-blueprint-text hover:text-blueprint-error transition-colors"
              >
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>

            <!-- Content -->
            <div class="p-6 bg-white" v-if="selectedDetails">
              <div class="flex justify-between items-start mb-6">
                <div>
                  <div class="text-4xl font-mono font-bold text-blueprint-text">{{ selectedDetails.rate.toFixed(4) }}</div>
                  <div class="text-sm font-sans font-bold text-blueprint-text-secondary mt-1">1 {{ selectedDetails.baseCurrency }} = {{ selectedDetails.targetCurrency }}</div>
                </div>
                <div 
                  class="px-3 py-1 text-sm font-mono font-bold border"
                  :class="selectedDetails.change24h >= 0 
                    ? 'border-blueprint-primary text-blueprint-primary bg-green-50' 
                    : 'border-blueprint-error text-blueprint-error bg-red-50'"
                >
                  {{ selectedDetails.change24h >= 0 ? '+' : '' }}{{ selectedDetails.change24h.toFixed(2) }}%
                </div>
              </div>

              <div class="grid grid-cols-2 gap-4 mb-6">
                <!-- 24h High -->
                <div class="p-3 border border-blueprint-border bg-blueprint-surface">
                  <div class="text-xs font-sans font-bold text-blueprint-text-secondary uppercase mb-1">24h High</div>
                  <div class="font-mono font-bold text-blueprint-text">{{ selectedDetails.high24h.toFixed(4) }}</div>
                </div>
                <!-- 24h Low -->
                <div class="p-3 border border-blueprint-border bg-blueprint-surface">
                  <div class="text-xs font-sans font-bold text-blueprint-text-secondary uppercase mb-1">24h Low</div>
                  <div class="font-mono font-bold text-blueprint-text">{{ selectedDetails.low24h.toFixed(4) }}</div>
                </div>
                <!-- Open -->
                <div class="p-3 border border-blueprint-border bg-blueprint-surface">
                  <div class="text-xs font-sans font-bold text-blueprint-text-secondary uppercase mb-1">Open</div>
                  <div class="font-mono font-bold text-blueprint-text">{{ selectedDetails.open24h.toFixed(4) }}</div>
                </div>
                <!-- Source -->
                <div class="p-3 border border-blueprint-border bg-blueprint-surface">
                  <div class="text-xs font-sans font-bold text-blueprint-text-secondary uppercase mb-1">Source</div>
                  <div class="font-mono font-bold text-blueprint-text truncate">{{ selectedDetails.source || 'Aggregated' }}</div>
                </div>
              </div>

              <!-- Action Button -->
              <button 
                @click="openAlertFromDetails"
                class="w-full py-3 bg-blueprint-text text-white font-sans text-sm font-bold uppercase hover:bg-blueprint-primary transition-colors flex items-center justify-center gap-2"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/>
                </svg>
                Set Alert for this Pair
              </button>
            </div>
            
            <!-- Loading State -->
            <div v-else class="p-12 text-center">
              <span class="font-sans text-xs text-blueprint-text-secondary">Loading details...</span>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

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
import { ref, onMounted, onUnmounted, computed, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { createAlert, getUserAlerts, deleteAlert } from '@/services/alertService';
import { getEnrichedRates, type EnrichedExchangeRate } from '@/services/exchangeRateService';
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
const isDetailsModalOpen = ref(false);
const selectedDetails = ref<EnrichedExchangeRate | null>(null);
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
  timeInterval = window.setInterval(updateTime, 10000);
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
  // Normalize pair string to handle "EUR/USD" or "EURUSD"
  const cleanPair = pair.replace('/', '');
  // Format for the form which might expect "EUR/USD" depending on how it's built
  // But AlertForm takes prefill-pair.
  selectedPair.value = pair.includes('/') ? pair : cleanPair.slice(0, 3) + '/' + cleanPair.slice(3);
  isModalOpen.value = true;
};

const handleViewDetails = async (pair: string) => {
  try {
    // 1. Show loading state or modal immediately
    isDetailsModalOpen.value = true;
    selectedDetails.value = null; // Clear previous

    // 2. Fetch enriched rates (assuming we don't have a reliable single-endpoint yet)
    // Optimization: In a real app, we'd cache this or use the store. 
    // For now, fetching fresh ensures accuracy.
    const response = await getEnrichedRates();
    
    // 3. Find the pair
    const cleanPair = pair.replace('/', '');
    const base = cleanPair.substring(0, 3);
    const target = cleanPair.substring(base.length); // Handle 3-char codes usually
    
    // Attempt to find match
    const found = response.rates.find((r: EnrichedExchangeRate) => 
      (r.baseCurrency === base && r.targetCurrency === target) ||
      (`${r.baseCurrency}/${r.targetCurrency}` === pair)
    );

    if (found) {
      selectedDetails.value = found;
    } else {
      showToast('Details not available for this pair.', 'error');
      closeDetailsModal();
    }
  } catch (error) {
    console.error('Failed to get details:', error);
    showToast('Failed to load rate details.', 'error');
    closeDetailsModal();
  }
};

const closeDetailsModal = () => {
  isDetailsModalOpen.value = false;
  selectedDetails.value = null;
};

const openAlertFromDetails = () => {
  if (selectedDetails.value) {
    const pair = `${selectedDetails.value.baseCurrency}/${selectedDetails.value.targetCurrency}`;
    closeDetailsModal();
    handleSelectPair(pair);
  }
};

const openAlertModal = () => {
  selectedPair.value = '';
  isModalOpen.value = true;
};

const closeAlertModal = () => {
  isModalOpen.value = false;
  selectedPair.value = '';
};

const handleCreateAlert = async (formData: any) => {
  try {
    await createAlert({
      baseCurrency: formData.baseCurrency,
      targetCurrency: formData.targetCurrency,
      condition: Number(formData.condition),
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
