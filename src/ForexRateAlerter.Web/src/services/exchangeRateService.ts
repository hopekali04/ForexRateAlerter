import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

const API_URL = 'http://localhost:5000/api/exchangerate';

const getAuthHeaders = () => {
  const authStore = useAuthStore();
  return {
    headers: {
      Authorization: `Bearer ${authStore.token}`,
    },
  };
};

export const getLatestRates = async () => {
  const response = await axios.get(`${API_URL}/latest`, getAuthHeaders());
  return response.data;
};

export const getLatestRate = async (baseCurrency: string, targetCurrency: string) => {
  const response = await axios.get(`${API_URL}/latest/${baseCurrency}/${targetCurrency}`, getAuthHeaders());
  return response.data;
};

export const getRateHistory = async (baseCurrency: string, targetCurrency: string, days: number) => {
  const response = await axios.get(`${API_URL}/history/${baseCurrency}/${targetCurrency}?days=${days}`, getAuthHeaders());
  return response.data;
};

export const refreshRates = async () => {
  const response = await axios.post(`${API_URL}/refresh`, {}, getAuthHeaders());
  return response.data;
};
