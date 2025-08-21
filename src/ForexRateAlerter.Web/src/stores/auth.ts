import { defineStore } from 'pinia';
import { ref } from 'vue';

interface User {
  id: string;
  email: string;
  role: 'User' | 'Admin';
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '');
  const userStr = localStorage.getItem('user');
  const user = ref<User | null>(userStr ? JSON.parse(userStr) : null);

  function setToken(newToken: string) {
    token.value = newToken;
    localStorage.setItem('token', newToken);
  }

  function setUser(newUser: User) {
    user.value = newUser;
    localStorage.setItem('user', JSON.stringify(newUser));
  }

  function logout() {
    token.value = '';
    user.value = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  return { token, user, setToken, setUser, logout };
});
