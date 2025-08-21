<template>
  <div class="relative min-h-screen overflow-hidden">
    <!-- Hero Background with Gradient -->
    <div class="absolute inset-0 bg-gradient-to-br from-blue-50 via-indigo-50/50 to-purple-50/30"></div>
    <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-blue-100/40 via-transparent to-transparent"></div>
    
    <!-- Hero Content -->
    <div class="relative px-6 pt-20 pb-16 mx-auto max-w-7xl lg:px-8 lg:pt-32">
      <div class="max-w-4xl mx-auto text-center">
        <!-- Main Heading -->
        <div class="mb-8 animate-fade-in">
          <h1 class="text-5xl font-bold tracking-tight text-gray-900 lg:text-7xl">
            <span class="bg-gradient-to-r from-blue-600 via-purple-600 to-indigo-600 bg-clip-text text-transparent">
              Forex Rate
            </span>
            <br>
            <span class="text-gray-900">Alerter</span>
          </h1>
        </div>
        
        <!-- Subtitle -->
        <div class="mb-12 animate-slide-up">
          <p class="text-xl leading-8 text-gray-600 lg:text-2xl max-w-3xl mx-auto">
            Stay ahead of the market with intelligent, real-time forex rate alerts. 
            Never miss an opportunity again.
          </p>
        </div>
        
        <!-- CTA Buttons -->
        <div v-if="!authStore.token" class="flex flex-col items-center justify-center gap-4 sm:flex-row sm:gap-6 animate-scale-in">
          <RouterLink
            to="/login"
            class="group relative inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-white transition-all duration-200 ease-out bg-gradient-to-r from-blue-600 to-indigo-600 rounded-2xl shadow-apple-lg hover:shadow-apple-xl hover:scale-105 active:scale-95 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          >
            <span class="relative z-10">Get Started</span>
            <div class="absolute inset-0 rounded-2xl bg-gradient-to-r from-blue-700 to-indigo-700 opacity-0 transition-opacity duration-200 group-hover:opacity-100"></div>
          </RouterLink>
          
          <RouterLink
            to="/register"
            class="group inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-gray-900 transition-all duration-200 ease-out bg-white/80 backdrop-blur-sm rounded-2xl shadow-apple border border-gray-200/50 hover:bg-white hover:shadow-apple-lg hover:scale-105 active:scale-95 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
          >
            Create Account
            <svg class="w-5 h-5 ml-2 transition-transform duration-200 group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 8l4 4m0 0l-4 4m4 4H3"/>
            </svg>
          </RouterLink>
        </div>
        
        <!-- Authenticated User CTA -->
        <div v-else class="animate-scale-in">
          <RouterLink
            :to="authStore.user?.role === 'Admin' ? '/admin' : '/dashboard'"
            class="group relative inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-white transition-all duration-200 ease-out bg-gradient-to-r from-blue-600 to-indigo-600 rounded-2xl shadow-apple-lg hover:shadow-apple-xl hover:scale-105 active:scale-95 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
          >
            <span class="relative z-10">Open Dashboard</span>
            <svg class="w-5 h-5 ml-2 transition-transform duration-200 group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 8l4 4m0 0l-4 4m4 4H3"/>
            </svg>
            <div class="absolute inset-0 rounded-2xl bg-gradient-to-r from-blue-700 to-indigo-700 opacity-0 transition-opacity duration-200 group-hover:opacity-100"></div>
          </RouterLink>
        </div>
      </div>
      
      <!-- Feature Cards -->
      <div class="mt-24 animate-fade-in">
        <div class="grid max-w-6xl grid-cols-1 gap-8 mx-auto md:grid-cols-3">
          <!-- Real-time Alerts -->
          <div class="group p-8 bg-white/60 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out">
            <div class="flex items-center justify-center w-16 h-16 mb-6 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 shadow-lg shadow-blue-500/25">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4m-6 2a9 9 0 110-18 9 9 0 010 18z"/>
              </svg>
            </div>
            <h3 class="mb-4 text-xl font-semibold text-gray-900">Real-time Alerts</h3>
            <p class="text-gray-600 leading-relaxed">Get instant notifications when exchange rates hit your target levels. Never miss a trading opportunity.</p>
          </div>
          
          <!-- Smart Monitoring -->
          <div class="group p-8 bg-white/60 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out">
            <div class="flex items-center justify-center w-16 h-16 mb-6 rounded-2xl bg-gradient-to-br from-purple-500 to-pink-600 shadow-lg shadow-purple-500/25">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
              </svg>
            </div>
            <h3 class="mb-4 text-xl font-semibold text-gray-900">Smart Monitoring</h3>
            <p class="text-gray-600 leading-relaxed">Advanced algorithms track multiple currency pairs simultaneously with intelligent threshold detection.</p>
          </div>
          
          <!-- Secure & Reliable -->
          <div class="group p-8 bg-white/60 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 hover:shadow-glass-lg hover:scale-105 transition-all duration-300 ease-out">
            <div class="flex items-center justify-center w-16 h-16 mb-6 rounded-2xl bg-gradient-to-br from-green-500 to-emerald-600 shadow-lg shadow-green-500/25">
              <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"/>
              </svg>
            </div>
            <h3 class="mb-4 text-xl font-semibold text-gray-900">Secure & Reliable</h3>
            <p class="text-gray-600 leading-relaxed">Enterprise-grade security with 99.9% uptime. Your data is protected with bank-level encryption.</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
</script>
