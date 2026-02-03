<template>
  <div class="border border-blueprint-border bg-blueprint-surface">
    <!-- Control Bar -->
    <div class="border-b border-blueprint-border bg-blueprint-bg px-4 py-3">
      <div class="flex flex-wrap items-center gap-4">
        <!-- Currency Pair Selector -->
        <div class="flex items-center gap-2">
          <label class="text-xs font-medium text-blueprint-text-secondary uppercase tracking-wide">Pair:</label>
          <select 
            v-model="selectedPair"
            @change="handlePairChange"
            class="bg-blueprint-surface border border-blueprint-border text-blueprint-text font-mono text-sm px-3 py-1.5 focus:outline-none focus:border-blueprint-primary"
          >
            <option v-for="pair in popularPairs" :key="pair.value" :value="pair.value">
              {{ pair.label }}
            </option>
          </select>
        </div>

        <!-- Timeframe Selector -->
        <div class="flex items-center gap-2">
          <label class="text-xs font-medium text-blueprint-text-secondary uppercase tracking-wide">Timeframe:</label>
          <div class="flex border border-blueprint-border">
            <button
              v-for="tf in timeframes"
              :key="tf"
              @click="selectedTimeframe = tf"
              :class="[
                'px-3 py-1.5 text-xs font-medium transition-colors',
                selectedTimeframe === tf 
                  ? 'bg-blueprint-primary text-white' 
                  : 'bg-blueprint-surface text-blueprint-text hover:bg-blueprint-bg'
              ]"
            >
              {{ tf }}
            </button>
          </div>
        </div>

        <!-- Chart Type Selector -->
        <div class="flex items-center gap-2">
          <label class="text-xs font-medium text-blueprint-text-secondary uppercase tracking-wide">Type:</label>
          <div class="flex border border-blueprint-border">
            <button
              v-for="type in chartTypes"
              :key="type.value"
              @click="selectedChartType = type.value"
              :class="[
                'px-3 py-1.5 text-xs font-medium transition-colors',
                selectedChartType === type.value 
                  ? 'bg-blueprint-primary text-white' 
                  : 'bg-blueprint-surface text-blueprint-text hover:bg-blueprint-bg'
              ]"
            >
              {{ type.label }}
            </button>
          </div>
        </div>

        <!-- Indicator Selector -->
        <div class="flex items-center gap-2">
          <label class="text-xs font-medium text-blueprint-text-secondary uppercase tracking-wide">Indicator:</label>
          <select 
            v-model="selectedIndicator"
            class="bg-blueprint-surface border border-blueprint-border text-blueprint-text text-sm px-3 py-1.5 focus:outline-none focus:border-blueprint-primary"
          >
            <option value="none">None</option>
            <option value="sma20">SMA 20</option>
            <option value="ema50">EMA 50</option>
            <option value="bollinger">Bollinger Bands</option>
          </select>
        </div>

        <!-- Refresh Button -->
        <button
          @click="fetchChartData"
          :disabled="isLoading"
          class="ml-auto px-3 py-1.5 border border-blueprint-border bg-blueprint-surface text-blueprint-text text-xs font-medium hover:bg-blueprint-bg disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
        >
          {{ isLoading ? 'Loading...' : 'Refresh' }}
        </button>
      </div>
    </div>

    <!-- Chart Container -->
    <div class="relative" style="height: 500px;">
      <!-- Loading State -->
      <div v-if="isLoading" class="absolute inset-0 flex items-center justify-center bg-blueprint-surface bg-opacity-90 z-10">
        <div class="text-center">
          <div class="inline-block h-8 w-8 animate-spin border-2 border-blueprint-primary border-r-transparent"></div>
          <p class="mt-2 text-sm text-blueprint-text-secondary">Loading chart data...</p>
        </div>
      </div>

      <!-- Error State -->
      <div v-if="error && !isLoading" class="absolute inset-0 flex items-center justify-center bg-blueprint-surface z-10">
        <div class="text-center max-w-md px-4">
          <div class="text-blueprint-error text-4xl mb-2">⚠</div>
          <p class="text-sm text-blueprint-text mb-4">{{ error }}</p>
          <button
            @click="fetchChartData"
            class="px-4 py-2 border border-blueprint-border bg-blueprint-surface text-blueprint-text text-sm hover:bg-blueprint-bg transition-colors"
          >
            Retry
          </button>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="!isLoading && !error && candleCount === 0" class="absolute inset-0 flex items-center justify-center bg-blueprint-surface">
        <div class="text-center">
          <div class="text-blueprint-text-secondary text-4xl mb-2">📊</div>
          <p class="text-sm text-blueprint-text-secondary">No data available for this pair</p>
        </div>
      </div>

      <!-- Chart Canvas -->
      <div ref="chartContainer" class="w-full h-full"></div>
    </div>

    <!-- Chart Footer: Stats -->
    <div v-if="latestCandle" class="border-t border-blueprint-border bg-blueprint-bg px-4 py-2">
      <div class="flex items-center gap-6 text-xs">
        <div class="flex items-center gap-2">
          <span class="text-blueprint-text-secondary uppercase tracking-wide">O:</span>
          <span class="font-mono text-blueprint-text">{{ latestCandle.open.toFixed(4) }}</span>
        </div>
        <div class="flex items-center gap-2">
          <span class="text-blueprint-text-secondary uppercase tracking-wide">H:</span>
          <span class="font-mono text-blueprint-text">{{ latestCandle.high.toFixed(4) }}</span>
        </div>
        <div class="flex items-center gap-2">
          <span class="text-blueprint-text-secondary uppercase tracking-wide">L:</span>
          <span class="font-mono text-blueprint-text">{{ latestCandle.low.toFixed(4) }}</span>
        </div>
        <div class="flex items-center gap-2">
          <span class="text-blueprint-text-secondary uppercase tracking-wide">C:</span>
          <span 
            class="font-mono font-semibold"
            :class="latestCandle.close >= latestCandle.open ? 'text-blueprint-primary' : 'text-blueprint-error'"
          >
            {{ latestCandle.close.toFixed(4) }}
          </span>
        </div>
        <div class="ml-auto text-blueprint-text-secondary">
          {{ formatTimestamp(latestCandle.time) }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import { createChart, CandlestickSeries, LineSeries, AreaSeries } from 'lightweight-charts';
import type { IChartApi, ISeriesApi, CandlestickData, LineData } from 'lightweight-charts';
import { getOHLCData, type OHLCDataPoint } from '@/services/exchangeRateService';

// Types
interface OHLCCandle {
  time: string;
  open: number;
  high: number;
  low: number;
  close: number;
}

// Popular currency pairs
const popularPairs = [
  { label: 'EUR/USD', value: 'EUR/USD' },
  { label: 'GBP/USD', value: 'GBP/USD' },
  { label: 'USD/JPY', value: 'USD/JPY' },
  { label: 'USD/CAD', value: 'USD/CAD' },
  { label: 'AUD/USD', value: 'AUD/USD' },
  { label: 'USD/MWK', value: 'USD/MWK' },
  { label: 'USD/ZAR', value: 'USD/ZAR' },
  { label: 'EUR/GBP', value: 'EUR/GBP' },
];

// Timeframe options
const timeframes = ['1m', '5m', '15m', '1h', '1D'];

// Chart types
const chartTypes = [
  { label: 'Candles', value: 'candlestick' },
  { label: 'Line', value: 'line' },
  { label: 'Area', value: 'area' },
];

// Reactive state
const chartContainer = ref<HTMLElement | null>(null);
const selectedPair = ref('EUR/USD');
const selectedTimeframe = ref('1h');
const selectedChartType = ref('candlestick');
const selectedIndicator = ref('none');
const isLoading = ref(false);
const error = ref<string | null>(null);
const candleCount = ref(0);
const latestCandle = ref<OHLCCandle | null>(null);

// Chart instances
let chart: IChartApi | null = null;
let series: ISeriesApi<'Candlestick'> | ISeriesApi<'Line'> | ISeriesApi<'Area'> | null = null;
let refreshInterval: ReturnType<typeof setInterval> | null = null;

// Handle window resize
const handleResize = () => {
  if (chart && chartContainer.value) {
    chart.applyOptions({ width: chartContainer.value.clientWidth });
  }
};

// Initialize chart
const initChart = () => {
  if (!chartContainer.value) return;

  chart = createChart(chartContainer.value, {
    width: chartContainer.value.clientWidth,
    height: 500,
    layout: {
      background: { color: '#FFFFFF' },
      textColor: '#0F172A',
    },
    grid: {
      vertLines: { color: '#E2E8F0' },
      horzLines: { color: '#E2E8F0' },
    },
    crosshair: {
      mode: 1,
      vertLine: {
        color: '#0F172A',
        width: 1,
        style: 2,
      },
      horzLine: {
        color: '#0F172A',
        width: 1,
        style: 2,
      },
    },
    rightPriceScale: {
      borderColor: '#E2E8F0',
    },
    timeScale: {
      borderColor: '#E2E8F0',
      timeVisible: true,
      secondsVisible: false,
    },
  });

  createSeries();

  // Attach resize listener
  window.addEventListener('resize', handleResize);
};

// Create series based on chart type
const createSeries = () => {
  if (!chart) return;

  // Remove existing series
  if (series) {
    chart.removeSeries(series);
  }

  if (selectedChartType.value === 'candlestick') {
    series = chart.addSeries(CandlestickSeries, {
      upColor: '#22C55E',
      downColor: '#EF4444',
      borderUpColor: '#22C55E',
      borderDownColor: '#EF4444',
      wickUpColor: '#22C55E',
      wickDownColor: '#EF4444',
    });
  } else if (selectedChartType.value === 'line') {
    series = chart.addSeries(LineSeries, {
      color: '#22C55E',
      lineWidth: 2,
    });
  } else if (selectedChartType.value === 'area') {
    series = chart.addSeries(AreaSeries, {
      topColor: 'rgba(34, 197, 94, 0.4)',
      bottomColor: 'rgba(34, 197, 94, 0.0)',
      lineColor: '#22C55E',
      lineWidth: 2,
    });
  }
};

// Fetch chart data
const fetchChartData = async () => {
  if (!series) return;

  isLoading.value = true;
  error.value = null;

  try {
    const [base, target] = selectedPair.value.split('/');
    const response = await getOHLCData(base, target, selectedTimeframe.value, 100);

    if (!response.candles || response.candles.length === 0) {
      candleCount.value = 0;
      latestCandle.value = null;
      return;
    }

    candleCount.value = response.candles.length;

    // Convert data for lightweight-charts
    const chartData = response.candles.map((candle: OHLCDataPoint) => {
      const timestamp = new Date(candle.time).getTime() / 1000;
      
      if (selectedChartType.value === 'candlestick') {
        return {
          time: timestamp,
          open: candle.open,
          high: candle.high,
          low: candle.low,
          close: candle.close,
        } as CandlestickData;
      } else {
        // For line and area charts, use close price
        return {
          time: timestamp,
          value: candle.close,
        } as LineData;
      }
    });

    series.setData(chartData as any);

    // Store latest candle for footer display
    const lastCandle = response.candles[response.candles.length - 1];
    latestCandle.value = {
      time: lastCandle.time,
      open: lastCandle.open,
      high: lastCandle.high,
      low: lastCandle.low,
      close: lastCandle.close,
    };

    // Fit content
    if (chart) {
      chart.timeScale().fitContent();
    }
  } catch (err: any) {
    error.value = err.response?.data?.error || 'Failed to load chart data. Please try again.';
    console.error('Chart data fetch error:', err);
  } finally {
    isLoading.value = false;
  }
};

// Handle pair change
const handlePairChange = () => {
  fetchChartData();
};

// Format timestamp for display
const formatTimestamp = (timestamp: string) => {
  const date = new Date(timestamp);
  return date.toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

// Watch for changes
watch(selectedTimeframe, () => {
  fetchChartData();
});

watch(selectedChartType, () => {
  createSeries();
  fetchChartData();
});

// Lifecycle hooks
onMounted(() => {
  initChart();
  fetchChartData();

  // Auto-refresh every 60 seconds
  refreshInterval = window.setInterval(() => {
    fetchChartData();
  }, 60000);
});

onUnmounted(() => {
  if (refreshInterval) {
    clearInterval(refreshInterval);
  }
  if (chart) {
    chart.remove();
  }
  window.removeEventListener('resize', handleResize);
});
</script>
