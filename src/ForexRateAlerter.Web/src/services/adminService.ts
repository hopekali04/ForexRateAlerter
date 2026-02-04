/* eslint-disable @typescript-eslint/no-explicit-any */
import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

const base_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
const API_URL = `${base_URL}/admin`;

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

export const getUser = async (id: number) => {
  const response = await axios.get(`${API_URL}/users/${id}`, getAuthHeaders());
  return response.data;
};

export const createUser = async (userData: {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
}) => {
  const response = await axios.post(`${API_URL}/users`, userData, getAuthHeaders());
  return response.data;
};

export const updateUser = async (id: number, userData: {
  email: string;
  password?: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
}) => {
  const response = await axios.put(`${API_URL}/users/${id}`, userData, getAuthHeaders());
  return response.data;
};

export const deleteUser = async (id: number) => {
  const response = await axios.delete(`${API_URL}/users/${id}`, getAuthHeaders());
  return response.data;
};

export const toggleUserStatus = async (id: number) => {
  const response = await axios.patch(`${API_URL}/users/${id}/toggle-status`, {}, getAuthHeaders());
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
