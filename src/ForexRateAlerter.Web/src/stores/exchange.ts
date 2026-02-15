import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { getLatestRates, type ExchangeRate } from '@/services/exchangeRateService';

export const useExchangeStore = defineStore('exchange', () => {
  const rates = ref<ExchangeRate[]>([]);
  const lastUpdated = ref<Date | null>(null);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  const fetchRates = async () => {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await getLatestRates();
      rates.value = response.rates;
      lastUpdated.value = new Date();
    } catch (err: any) {
      console.error('Failed to fetch rates', err);
      // If we fail, we keep old rates but set error
      error.value = 'Failed to update rates';
    } finally {
      isLoading.value = false;
    }
  };

  // Helper to find a specific rate
  const getRate = (base: string, target: string) => {
    return rates.value.find(
      r => r.baseCurrency === base && r.targetCurrency === target
    );
  };

  return {
    rates,
    lastUpdated,
    isLoading,
    error,
    fetchRates,
    getRate
  };
});
