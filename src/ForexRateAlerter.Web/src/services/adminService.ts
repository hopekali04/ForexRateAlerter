import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

const API_URL = 'http://localhost:5000/api/admin';

const getAuthHeaders = () => {
  const authStore = useAuthStore();
  return {
    headers: {
      Authorization: `Bearer ${authStore.token}`,
    },
  };
};

export const getAllUsers = async () => {
  const response = await axios.get(`${API_URL}/users`, getAuthHeaders());
  return response.data;
};

export const getAllAlerts = async () => {
  const response = await axios.get(`${API_URL}/alerts`, getAuthHeaders());
  return response.data;
};

export const getAlertLogs = async (page: number, pageSize: number) => {
  const response = await axios.get(`${API_URL}/alert-logs?page=${page}&pageSize=${pageSize}`, getAuthHeaders());
  return response.data;
};

export const getStatistics = async () => {
  const response = await axios.get(`${API_URL}/statistics`, getAuthHeaders());
  return response.data;
};
