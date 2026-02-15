<template>
  <div class="border-b border-blueprint-border bg-blueprint-surface overflow-hidden mt-3">
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
        <!-- Duplicate content for seamless animation -->
        <div 
          v-for="(rate, index) in tickerRates" 
          :key="`${rate.pair}-${index}-clone`"
          class="ticker-item"
          aria-hidden="true"
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
import { getEnrichedRates, type EnrichedExchangeRate } from '@/services/exchangeRateService';

interface TickerRate {
  pair: string;
  rate: string;
  change: number;
}

const tickerRates = ref<TickerRate[]>([]);
const isLoading = ref(true);

let updateInterval: number | null = null;

const fetchRates = async () => {
  try {
    // Use ENRICHED endpoint to get server-side calculated 24h change
    const response = await getEnrichedRates();
    
    if (response?.rates && Array.isArray(response.rates)) {
      // Transform API data to ticker format
      tickerRates.value = response.rates.slice(0, 15).map((rate: EnrichedExchangeRate) => {
        const pair = `${rate.baseCurrency}/${rate.targetCurrency}`;
        
        return {
          pair,
          rate: rate.rate.toFixed(4),
          // Backend provides the true 24h change percentage
          change: parseFloat(rate.change24h.toFixed(2)) 
        };
      });
    }
    
    isLoading.value = false;
  } catch (error) {
    console.error('Failed to fetch enriched exchange rates:', error);
    isLoading.value = false;
    // Fallback to empty array on error - graceful degradation
    if (tickerRates.value.length === 0) {
      tickerRates.value = [];
    }
  }
};

onMounted(async () => {
  await fetchRates();
  // Refresh rates every 5 minutes
  updateInterval = window.setInterval(fetchRates, 300000);
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
