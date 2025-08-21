<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-indigo-50/50">
    <div class="px-6 py-16 mx-auto max-w-4xl lg:px-8">
      <!-- Header -->
      <div class="text-center mb-16 animate-fade-in">
        <h1 class="text-4xl font-bold tracking-tight text-gray-900 mb-6 lg:text-6xl">
          <span class="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
            About
          </span>
          <br>
          <span class="text-gray-900">Forex Rate Alerter</span>
        </h1>
        <p class="text-xl text-gray-600 max-w-3xl mx-auto leading-relaxed">
          Empowering traders worldwide with intelligent, real-time forex monitoring and alerting solutions.
        </p>
      </div>

      <!-- Content Sections -->
      <div class="space-y-16 animate-slide-up">
        <!-- Mission -->
        <div class="bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-8 lg:p-12">
          <div class="flex items-center mb-6">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 shadow-lg shadow-blue-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M13 10V3L4 14h7v7l9-11h-7z"/>
              </svg>
            </div>
            <h2 class="text-2xl font-bold text-gray-900">Our Mission</h2>
          </div>
          <p class="text-lg text-gray-600 leading-relaxed">
            We believe that every trader deserves access to professional-grade forex monitoring tools. Our mission is to democratize financial market intelligence by providing sophisticated yet intuitive alerting systems that help you make informed trading decisions.
          </p>
        </div>

        <!-- Features -->
        <div class="bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-8 lg:p-12">
          <div class="flex items-center mb-8">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-purple-500 to-pink-600 shadow-lg shadow-purple-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
            <h2 class="text-2xl font-bold text-gray-900">Key Features</h2>
          </div>
          <div class="grid grid-cols-1 gap-6 md:grid-cols-2">
            <div class="space-y-3">
              <h3 class="text-lg font-semibold text-gray-900">Real-time Monitoring</h3>
              <p class="text-gray-600">Track multiple currency pairs simultaneously with millisecond precision.</p>
            </div>
            <div class="space-y-3">
              <h3 class="text-lg font-semibold text-gray-900">Smart Alerts</h3>
              <p class="text-gray-600">Customizable conditions with instant notifications via multiple channels.</p>
            </div>
            <div class="space-y-3">
              <h3 class="text-lg font-semibold text-gray-900">Enterprise Security</h3>
              <p class="text-gray-600">Bank-level encryption and security protocols protect your data.</p>
            </div>
            <div class="space-y-3">
              <h3 class="text-lg font-semibold text-gray-900">Global Coverage</h3>
              <p class="text-gray-600">Monitor major and exotic currency pairs from markets worldwide.</p>
            </div>
          </div>
        </div>

        <!-- Team -->
        <div class="bg-white/80 backdrop-blur-xl rounded-3xl shadow-glass border border-white/20 p-8 lg:p-12">
          <div class="flex items-center mb-8">
            <div class="flex items-center justify-center w-12 h-12 rounded-2xl bg-gradient-to-br from-green-500 to-emerald-600 shadow-lg shadow-green-500/25 mr-4">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"/>
              </svg>
            </div>
            <h2 class="text-2xl font-bold text-gray-900">Our Team</h2>
          </div>
          <p class="text-lg text-gray-600 leading-relaxed">
            Built by a passionate team of financial technology experts, software engineers, and trading professionals. We combine deep market knowledge with cutting-edge technology to deliver tools that truly make a difference in your trading journey.
          </p>
        </div>

        <!-- Contact -->
        <div class="text-center">
          <div class="bg-gradient-to-r from-blue-600 to-indigo-600 rounded-3xl p-8 lg:p-12 text-white">
            <h2 class="text-2xl font-bold mb-4">Ready to Get Started?</h2>
            <p class="text-blue-100 mb-8 text-lg">
              Join thousands of traders who trust Forex Rate Alerter for their market monitoring needs.
            </p>
            <RouterLink
              to="/register"
              class="inline-flex items-center justify-center px-8 py-4 text-base font-semibold text-blue-600 bg-white rounded-2xl shadow-apple-lg hover:shadow-apple-xl hover:scale-105 active:scale-95 transition-all duration-200 ease-out"
            >
              Create Free Account
              <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 8l4 4m0 0l-4 4m4 4H3"/>
              </svg>
            </RouterLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Component logic if needed
</script>
