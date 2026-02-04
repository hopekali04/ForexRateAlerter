import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

const base_Url = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
const API_URL = `${base_Url}/exchangerate`;

export interface ExchangeRate {
  id: number;
  baseCurrency: string;
  targetCurrency: string;
  rate: number;
  timestamp: string;
  source: string;
}

export interface EnrichedExchangeRate extends ExchangeRate {
  high24h: number;
  low24h: number;
  open24h: number;
  change24h: number;
}

export interface LatestRatesResponse {
  rates: ExchangeRate[];
  timestamp: string;
}

export interface EnrichedRatesResponse {
  rates: EnrichedExchangeRate[];
  timestamp: string;
}

export interface RateHistoryResponse {
  history: ExchangeRate[];
  days: number;
}

export interface OHLCDataPoint {
  time: string;
  open: number;
  high: number;
  low: number;
  close: number;
  baseCurrency: string;
  targetCurrency: string;
}

export interface OHLCResponse {
  candles: OHLCDataPoint[];
  timeframe: string;
  count: number;
  baseCurrency: string;
  targetCurrency: string;
  timestamp: string;
}

const getAuthHeaders = () => {
  const authStore = useAuthStore();
  return {
    headers: {
      Authorization: `Bearer ${authStore.token}`,
    },
  };
};

export const getLatestRates = async (): Promise<LatestRatesResponse> => {
  const response = await axios.get<LatestRatesResponse>(`${API_URL}/latest`, getAuthHeaders());
  return response.data;
};

export const getEnrichedRates = async (): Promise<EnrichedRatesResponse> => {
  const response = await axios.get<EnrichedRatesResponse>(`${API_URL}/latest-enriched`, getAuthHeaders());
  return response.data;
};

export const getLatestRate = async (baseCurrency: string, targetCurrency: string): Promise<ExchangeRate> => {
  const response = await axios.get<ExchangeRate>(`${API_URL}/latest/${baseCurrency}/${targetCurrency}`, getAuthHeaders());
  return response.data;
};

export const getRateHistory = async (baseCurrency: string, targetCurrency: string, days: number): Promise<RateHistoryResponse> => {
  const response = await axios.get<RateHistoryResponse>(`${API_URL}/history/${baseCurrency}/${targetCurrency}?days=${days}`, getAuthHeaders());
  return response.data;
};

export const refreshRates = async (): Promise<boolean> => {
  const response = await axios.post<boolean>(`${API_URL}/refresh`, {}, getAuthHeaders());
  return response.data;
};

export const getOHLCData = async (
  baseCurrency: string, 
  targetCurrency: string, 
  timeframe: string = '1h', 
  limit: number = 100
): Promise<OHLCResponse> => {
  const response = await axios.get<OHLCResponse>(
    `${API_URL}/ohlc/${baseCurrency}/${targetCurrency}?timeframe=${timeframe}&limit=${limit}`, 
    getAuthHeaders()
  );
  return response.data;
};
