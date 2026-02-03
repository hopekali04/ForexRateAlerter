<template>
  <div class="relative min-h-screen overflow-hidden">
    <!-- Hero Background -->
    <div class="absolute inset-0 bg-slate-50"></div>
    
    <!-- Hero Content -->
    <div class="relative px-6 pt-20 pb-16 mx-auto max-w-7xl lg:px-8 lg:pt-32">
      <div class="max-w-4xl mx-auto text-center">
        <!-- Main Heading -->
        <div class="mb-8">
          <h1 class="text-5xl font-bold tracking-tight text-slate-900 lg:text-6xl">
            Forex Rate Alerter
          </h1>
        </div>
        
        <!-- Subtitle -->
        <div class="mb-12">
          <p class="text-xl leading-8 text-gray-600 lg:text-xl max-w-3xl mx-auto">
            Stay ahead of the market with intelligent, real-time forex rate alerts. 
            Never miss an opportunity again.
          </p>
        </div>
        
        <!-- CTA Buttons -->
        <div v-if="!authStore.token" class="flex flex-col items-center justify-center gap-4 sm:flex-row sm:gap-6">
          <RouterLink
            to="/login"
            class="group relative inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-white transition-all duration-200 bg-green-500 border border-solid border-gray-300 hover:bg-green-600 focus:outline-none"
          >
            <span class="relative z-10">Get Started</span>
          </RouterLink>
          
          <RouterLink
            to="/register"
            class="group inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-slate-900 transition-all duration-200 bg-white border border-solid border-gray-300 hover:bg-gray-50 focus:outline-none"
          >
            Create Account
            <svg class="w-5 h-5 ml-2 transition-transform duration-200 group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 8l4 4m0 0l-4 4m4 4H3"/>
            </svg>
          </RouterLink>
        </div>
        
        <!-- Authenticated User CTA -->
        <div v-else>
          <RouterLink
            :to="authStore.user?.role === 'Admin' ? '/admin' : '/dashboard'"
            class="group relative inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-white transition-all duration-200 bg-green-500 border border-solid border-gray-300 hover:bg-green-600 focus:outline-none"
          >
            <span class="relative z-10">Open Dashboard</span>
            <svg class="w-5 h-5 ml-2 transition-transform duration-200 group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 8l4 4m0 0l-4 4m4 4H3"/>
            </svg>
          </RouterLink>
        </div>
      </div>
      
      <!-- Feature Cards -->
      <div class="mt-20">
        <div class="grid max-w-6xl grid-cols-1 gap-6 mx-auto md:grid-cols-3">
          <!-- Real-time Alerts -->
          <div class="group p-6 bg-white border border-solid border-gray-300 hover:bg-gray-50 transition-all duration-200">
            <div class="flex items-center justify-center w-12 h-12 mb-6 bg-green-500 border border-solid border-gray-300">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4m-6 2a9 9 0 110-18 9 9 0 010 18z"/>
              </svg>
            </div>
            <h3 class="mb-4 text-lg font-semibold text-slate-900">Real-time Alerts</h3>
            <p class="text-gray-600 leading-relaxed">Get instant notifications when exchange rates hit your target levels. Never miss a trading opportunity.</p>
          </div>
          
          <!-- Smart Monitoring -->
          <div class="group p-6 bg-white border border-solid border-gray-300 hover:bg-gray-50 transition-all duration-200">
            <div class="flex items-center justify-center w-12 h-12 mb-6 bg-green-500 border border-solid border-gray-300">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
              </svg>
            </div>
            <h3 class="mb-4 text-lg font-semibold text-slate-900">Smart Monitoring</h3>
            <p class="text-gray-600 leading-relaxed">Advanced algorithms track multiple currency pairs simultaneously with intelligent threshold detection.</p>
          </div>
          
          <!-- Secure & Reliable -->
          <div class="group p-6 bg-white border border-solid border-gray-300 hover:bg-gray-50 transition-all duration-200">
            <div class="flex items-center justify-center w-12 h-12 mb-6 bg-green-500 border border-solid border-gray-300">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z"/>
              </svg>
            </div>
            <h3 class="mb-4 text-lg font-semibold text-slate-900">Secure & Reliable</h3>
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
