<template>
  <div class="bg-blueprint-surface border border-blueprint-border">
    <!-- Header -->
    <div class="border-b border-blueprint-border px-6 py-4">
      <h3 class="font-sans text-sm font-bold text-blueprint-text uppercase tracking-wide">Market Pulse – Top Movers</h3>
      <p class="font-sans text-xs text-blueprint-text-secondary mt-1">Click any currency pair to set an alert</p>
    </div>

    <!-- Currency Strength Bars -->
    <div class="p-6 space-y-3">
      <!-- Loading State -->
      <div v-if="isLoading" class="text-center py-8">
        <span class="font-sans text-xs text-blueprint-text-secondary">Loading market data...</span>
      </div>
      
      <!-- Empty State -->
      <div v-else-if="topMovers.length === 0" class="text-center py-8">
        <span class="font-sans text-xs text-blueprint-text-secondary">No market data available</span>
      </div>
      
      <!-- Currency Bars -->
      <div 
        v-else
        v-for="mover in topMovers" 
        :key="mover.pair"
        @click="$emit('select-pair', mover.pair)"
        class="cursor-pointer group"
      >
        <!-- Currency Pair Label -->
        <div class="flex justify-between items-baseline mb-1">
          <div class="flex items-center">
            <span class="font-sans text-xs font-bold text-blueprint-text">{{ mover.pair }}</span>
            <span 
              class="font-mono text-xs font-semibold ml-2"
              :class="mover.change >= 0 ? 'text-blueprint-primary' : 'text-blueprint-error'"
            >
              {{ mover.rate }}
            </span>
          </div>
          <div class="flex items-center">
            <span 
              class="font-mono text-xs font-bold"
              :class="mover.change >= 0 ? 'text-blueprint-primary' : 'text-blueprint-error'"
            >
              {{ mover.change >= 0 ? '▲' : '▼' }} {{ Math.abs(mover.change).toFixed(2) }}%
            </span>
          </div>
        </div>

        <!-- Divergence Bar -->
        <div class="h-8 border border-blueprint-border bg-blueprint-bg relative overflow-hidden group-hover:border-2 group-hover:border-blueprint-primary transition-all">
          <div 
            class="h-full transition-all duration-500"
            :class="mover.change >= 0 ? 'bg-blueprint-primary' : 'bg-blueprint-error'"
            :style="{ width: `${Math.min(Math.abs(mover.change) * 20, 100)}%` }"
          >
          </div>
          <!-- Value Label Inside Bar -->
          <div class="absolute inset-0 flex items-center px-2">
            <span class="font-mono text-xs font-bold text-blueprint-text">
              {{ mover.change >= 0 ? 'GAINING' : 'LOSING' }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Volatility Indicator -->
    <div class="border-t border-blueprint-border px-6 py-3 bg-blueprint-bg">
      <div class="flex justify-between items-center">
        <span class="font-sans text-xs font-semibold text-blueprint-text-secondary">VOLATILITY INDEX</span>
        <span class="font-mono text-xs font-bold text-blueprint-text">{{ volatilityIndex.toFixed(1) }}</span>
      </div>
      <div class="mt-2 h-2 border border-blueprint-border bg-blueprint-surface">
        <div 
          class="h-full bg-blueprint-text"
          :style="{ width: `${Math.min(volatilityIndex, 100)}%` }"
        ></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { getLatestRates, type ExchangeRate } from '@/services/exchangeRateService';

interface CurrencyMover {
  pair: string;
  rate: string;
  change: number;
  volume: number;
}

defineEmits<{
  'select-pair': [pair: string]
}>();

const topMovers = ref<CurrencyMover[]>([]);
const isLoading = ref(true);
const previousRates = new Map<string, number>();

const volatilityIndex = computed(() => {
  if (topMovers.value.length === 0) return 0;
  const avgChange = topMovers.value.reduce((sum, m) => sum + Math.abs(m.change), 0) / topMovers.value.length;
  return avgChange * 25; // Scale to 0-100
});

let updateInterval: number | null = null;

const fetchTopMovers = async () => {
  try {
    const response = await getLatestRates();
    
    if (response?.rates && Array.isArray(response.rates)) {
      // Transform and calculate changes
      const moversWithChanges = response.rates.map((rate: ExchangeRate) => {
        const pair = `${rate.baseCurrency}/${rate.targetCurrency}`;
        const currentRate = rate.rate; // Already a number from API
        const previousRate = previousRates.get(pair) || currentRate;
        
        // Calculate percentage change
        const change = previousRate !== 0 
          ? ((currentRate - previousRate) / previousRate) * 100 
          : 0;
        
        // Store current rate for next comparison
        previousRates.set(pair, currentRate);
        
        return {
          pair,
          rate: currentRate.toFixed(4),
          change: parseFloat(change.toFixed(2)),
          volume: Math.floor(Math.random() * 100000) + 50000 // Simulated volume
        };
      });
      
      // Sort by absolute change and take top 5
      topMovers.value = moversWithChanges
        .sort((a, b) => Math.abs(b.change) - Math.abs(a.change))
        .slice(0, 5);
    }
    
    isLoading.value = false;
  } catch (error) {
    console.error('Failed to fetch top movers:', error);
    isLoading.value = false;
    // Graceful degradation - keep existing data
    if (topMovers.value.length === 0) {
      // Fallback to empty array
      topMovers.value = [];
    }
  }
};

onMounted(async () => {
  await fetchTopMovers();
  // Refresh every 30 seconds
  updateInterval = window.setInterval(fetchTopMovers, 30000);
});

onUnmounted(() => {
  if (updateInterval) {
    clearInterval(updateInterval);
  }
});
</script>
