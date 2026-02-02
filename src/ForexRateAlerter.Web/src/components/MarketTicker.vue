<template>
  <div class="border-b border-blueprint-border bg-blueprint-surface overflow-hidden">
    <div class="ticker-wrapper">
      <div v-if="isLoading" class="flex items-center justify-center h-full">
        <span class="font-sans text-xs text-blueprint-text-secondary">Loading rates...</span>
      </div>
      <div v-else-if="tickerRates.length === 0" class="flex items-center justify-center h-full">
        <span class="font-sans text-xs text-blueprint-text-secondary">No rates available</span>
      </div>
      <div v-else class="ticker-content">
        <div 
          v-for="(rate, index) in tickerRates" 
          :key="`${rate.pair}-${index}`"
          class="ticker-item"
        >
          <span class="font-sans text-xs font-semibold text-blueprint-text uppercase">{{ rate.pair }}</span>
          <span 
            class="font-mono text-xs font-bold ml-2"
            :class="rate.change >= 0 ? 'text-blueprint-primary' : 'text-blueprint-error'"
          >
            {{ rate.rate }}
          </span>
          <span 
            class="font-mono text-xs ml-1"
            :class="rate.change >= 0 ? 'text-blueprint-primary' : 'text-blueprint-error'"
          >
            {{ rate.change >= 0 ? '▲' : '▼' }} {{ Math.abs(rate.change).toFixed(2) }}%
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { getLatestRates, type ExchangeRate } from '@/services/exchangeRateService';

interface TickerRate {
  pair: string;
  rate: string;
  change: number;
}

const tickerRates = ref<TickerRate[]>([]);
const isLoading = ref(true);
const previousRates = new Map<string, number>();

let updateInterval: number | null = null;

const fetchRates = async () => {
  try {
    const response = await getLatestRates();
    
    if (response?.rates && Array.isArray(response.rates)) {
      // Transform API data to ticker format
      tickerRates.value = response.rates.slice(0, 10).map((rate: ExchangeRate) => {
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
          change: parseFloat(change.toFixed(2))
        };
      });
    }
    
    isLoading.value = false;
  } catch (error) {
    console.error('Failed to fetch exchange rates:', error);
    isLoading.value = false;
    // Fallback to empty array on error - graceful degradation
    if (tickerRates.value.length === 0) {
      tickerRates.value = [];
    }
  }
};

onMounted(async () => {
  await fetchRates();
  // Refresh rates every 30 seconds
  updateInterval = window.setInterval(fetchRates, 30000);
});

onUnmounted(() => {
  if (updateInterval) {
    clearInterval(updateInterval);
  }
});
</script>

<style scoped>
.ticker-wrapper {
  position: relative;
  width: 100%;
  overflow: hidden;
  height: 32px;
}

.ticker-content {
  display: flex;
  animation: ticker 30s linear infinite;
  white-space: nowrap;
}

.ticker-item {
  display: inline-flex;
  align-items: center;
  padding: 0 24px;
  border-right: 1px solid #E2E8F0;
}

@keyframes ticker {
  0% {
    transform: translateX(0);
  }
  100% {
    transform: translateX(-50%);
  }
}

/* Duplicate content for seamless loop */
.ticker-content::after {
  content: '';
  display: flex;
}
</style>
