/* eslint-disable @typescript-eslint/no-explicit-any */
import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

const base_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
const API_URL = `${base_URL}/alert`;

const getAuthHeaders = () => {
  const authStore = useAuthStore();
  return {
    headers: {
      Authorization: `Bearer ${authStore.token}`,
    },
  };
};

export const createAlert = async (alertData: any) => {
  const response = await axios.post(API_URL, alertData, getAuthHeaders());
  return response.data;
};

export const getUserAlerts = async () => {
  const response = await axios.get(API_URL, getAuthHeaders());
  return response.data;
};

export const getAlertById = async (id: number) => {
  const response = await axios.get(`${API_URL}/${id}`, getAuthHeaders());
  return response.data;
};

export const updateAlert = async (id: number, alertData: any) => {
  const response = await axios.put(`${API_URL}/${id}`, alertData, getAuthHeaders());
  return response.data;
};

export const deleteAlert = async (id: number) => {
  const response = await axios.delete(`${API_URL}/${id}`, getAuthHeaders());
  return response.data;
};
