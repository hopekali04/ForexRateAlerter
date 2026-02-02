<template>
  <div class="relative min-h-screen flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
    <!-- Background -->
    <div class="absolute inset-0 bg-slate-50"></div>
    
    <!-- Register Card -->
    <div class="relative w-full max-w-md">
      <div class="bg-white border border-solid border-gray-300 p-8">
        <!-- Header -->
        <div class="text-center mb-8">
          <div class="mx-auto w-12 h-12 bg-green-500 border border-solid border-gray-300 flex items-center justify-center mb-6">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"/>
            </svg>
          </div>
          <h1 class="text-2xl font-bold text-slate-900 mb-2">Create your account</h1>
          <p class="text-gray-600">Join thousands of traders monitoring forex rates</p>
        </div>

        <!-- Registration Form -->
        <form @submit.prevent="handleRegister" class="space-y-6">
          <div class="space-y-5">
            <!-- Name Fields -->
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label for="firstName" class="block text-sm font-medium text-gray-700 mb-2">First Name</label>
                <input
                  type="text"
                  id="firstName"
                  v-model="firstName"
                  class="block w-full px-4 py-3 text-slate-900 placeholder-gray-500 bg-white border border-solid border-gray-300 focus:outline-none focus:border-green-500 transition-all duration-200"
                  placeholder="John"
                  required
                />
              </div>
              <div>
                <label for="lastName" class="block text-sm font-medium text-gray-700 mb-2">Last Name</label>
                <input
                  type="text"
                  id="lastName"
                  v-model="lastName"
                  class="block w-full px-4 py-3 text-slate-900 placeholder-gray-500 bg-white border border-solid border-gray-300 focus:outline-none focus:border-green-500 transition-all duration-200"
                  placeholder="Doe"
                  required
                />
              </div>
            </div>

            <!-- Email Field -->
            <div>
              <label for="email" class="block text-sm font-medium text-gray-700 mb-2">Email address</label>
              <div class="relative">
                <input
                  type="email"
                  id="email"
                  v-model="email"
                  class="block w-full px-4 py-3 text-slate-900 placeholder-gray-500 bg-white border border-solid border-gray-300 focus:outline-none focus:border-green-500 transition-all duration-200"
                  placeholder="john@example.com"
                  required
                />
                <div class="absolute inset-y-0 right-0 flex items-center pr-4">
                  <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M16 12a4 4 0 10-8 0 4 4 0 008 0zm0 0v1.5a2.5 2.5 0 005 0V12a9 9 0 10-9 9m4.5-1.206a8.959 8.959 0 01-4.5 1.207"/>
                  </svg>
                </div>
              </div>
            </div>

            <!-- Password Field -->
            <div>
              <label for="password" class="block text-sm font-medium text-gray-700 mb-2">Password</label>
              <div class="relative">
                <input
                  :type="showPassword ? 'text' : 'password'"
                  id="password"
                  v-model="password"
                  class="block w-full px-4 py-3 text-slate-900 placeholder-gray-500 bg-white border border-solid border-gray-300 focus:outline-none focus:border-green-500 transition-all duration-200"
                  placeholder="Create a strong password"
                  required
                />
                <button
                  type="button"
                  @click="showPassword = !showPassword"
                  class="absolute inset-y-0 right-0 flex items-center pr-4 text-gray-400 hover:text-gray-600 transition-colors duration-200"
                >
                  <svg v-if="showPassword" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L21 21"/>
                  </svg>
                  <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                  </svg>
                </button>
              </div>
            </div>
          </div>

          <!-- Create Account Button -->
          <button
            type="submit"
            :disabled="isLoading"
            class="group relative w-full flex justify-center py-4 px-4 text-base font-semibold text-white bg-green-500 border border-solid border-gray-300 hover:bg-green-600 focus:outline-none focus:bg-green-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
          >
            <span v-if="!isLoading" class="relative z-10">Create Account</span>
            <span v-else class="relative z-10 flex items-center">
              <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Creating account...
            </span>
          </button>

          <!-- Terms -->
          <p class="text-xs text-center text-gray-600">
            By creating an account, you agree to our
            <a href="#" class="text-green-600 hover:text-green-500 transition-colors duration-200">Terms of Service</a>
            and
            <a href="#" class="text-green-600 hover:text-green-500 transition-colors duration-200">Privacy Policy</a>
          </p>
        </form>

        <!-- Sign In Link -->
        <div class="mt-8 pt-6 border-t border-solid border-gray-300">
          <p class="text-center text-sm text-gray-600">
            Already have an account?
            <RouterLink to="/login" class="font-medium text-green-600 hover:text-green-500 transition-colors duration-200">
              Sign in instead
            </RouterLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { register } from '@/services/authService';

const firstName = ref('');
const lastName = ref('');
const email = ref('');
const password = ref('');
const showPassword = ref(false);
const isLoading = ref(false);
const router = useRouter();

const handleRegister = async () => {
  if (isLoading.value) return;
  
  try {
    isLoading.value = true;
    await register({
      firstName: firstName.value,
      lastName: lastName.value,
      email: email.value,
      password: password.value,
    });
    router.push('/login');
  } catch (error) {
    console.error('Registration failed:', error);
  } finally {
    isLoading.value = false;
  }
};
</script>
