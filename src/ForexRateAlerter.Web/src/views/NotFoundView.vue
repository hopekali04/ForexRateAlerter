<template>
  <div class="relative min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-50 via-blue-50/30 to-indigo-50/50">
    <!-- Background decoration -->
    <div class="absolute inset-0 bg-[radial-gradient(ellipse_at_center,_var(--tw-gradient-stops))] from-blue-100/40 via-transparent to-transparent"></div>
    
    <div class="relative text-center px-6 py-16 animate-fade-in">
      <!-- 404 Icon -->
      <div class="mx-auto w-32 h-32 rounded-4xl bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center mb-8 shadow-glass">
        <svg class="w-16 h-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9.172 16.172a4 4 0 015.656 0M9 12h6m-6-4h6m2 5.291A7.962 7.962 0 0112 20a7.962 7.962 0 01-5-1.709V16h10v2.291zM20 12a8 8 0 11-16 0 8 8 0 0116 0z"/>
        </svg>
      </div>

      <!-- Error Message -->
      <div class="mb-8">
        <h1 class="text-8xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent mb-4 tracking-tight">
          404
        </h1>
        <h2 class="text-3xl font-bold text-gray-900 mb-4">Page Not Found</h2>
        <p class="text-xl text-gray-600 max-w-md mx-auto leading-relaxed">
          The page you're looking for doesn't exist or has been moved to a different location.
        </p>
      </div>

      <!-- Actions -->
      <div class="space-y-4 sm:space-y-0 sm:space-x-4 sm:flex sm:justify-center animate-slide-up">
        <RouterLink
          to="/"
          class="group relative inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-white bg-gradient-to-r from-blue-600 to-indigo-600 rounded-2xl shadow-apple-lg hover:shadow-apple-xl hover:scale-105 active:scale-95 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all duration-200 ease-out"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
          </svg>
          <span class="relative z-10">Go Home</span>
          <div class="absolute inset-0 rounded-2xl bg-gradient-to-r from-blue-700 to-indigo-700 opacity-0 transition-opacity duration-200 group-hover:opacity-100"></div>
        </RouterLink>

        <button
          @click="goBack"
          class="group inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-gray-900 bg-white/80 backdrop-blur-sm rounded-2xl shadow-apple border border-gray-200/50 hover:bg-white hover:shadow-apple-lg hover:scale-105 active:scale-95 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 transition-all duration-200 ease-out"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
          </svg>
          Go Back
        </button>
      </div>

      <!-- Help Text -->
      <div class="mt-12 p-6 bg-white/60 backdrop-blur-xl rounded-2xl shadow-glass border border-white/20 max-w-lg mx-auto">
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Need Help?</h3>
        <p class="text-gray-600 text-sm">
          If you believe this is an error, please contact our support team or try refreshing the page.
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router';

const router = useRouter();

const goBack = () => {
  if (window.history.length > 1) {
    router.go(-1);
  } else {
    router.push('/');
  }
};
</script>
