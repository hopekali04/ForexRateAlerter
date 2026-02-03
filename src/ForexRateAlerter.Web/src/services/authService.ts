import axios from 'axios';

const base_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';
const API_URL = `${base_URL}/auth`;

interface LoginCredentials {
  email: string;
  password: string;
}

interface RegisterData {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export const login = async (credentials: LoginCredentials) => {
  const response = await axios.post(`${API_URL}/login`, credentials);
  return response.data;
};

export const register = async (userData: RegisterData) => {
  const response = await axios.post(`${API_URL}/register`, userData);
  return response.data;
};
