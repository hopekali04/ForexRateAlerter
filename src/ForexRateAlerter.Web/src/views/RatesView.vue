<template>
  <div class="min-h-screen bg-blueprint-bg">
    <div class="px-6 py-8 mx-auto max-w-7xl lg:px-8">
      <!-- Header Section -->
      <div class="mb-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between">
          <div>
            <h1 class="text-3xl font-bold tracking-tight text-blueprint-text mb-2">
              Exchange Rates
            </h1>
            <p class="text-base text-blueprint-text-secondary">
              Real-time currency exchange rates with comprehensive market data
            </p>
          </div>
          <div class="flex items-center gap-3 mt-4 md:mt-0">
            <span v-if="lastUpdateTime" class="text-xs font-mono text-blueprint-text-secondary">
              Last updated: {{ formatTime(lastUpdateTime) }}
            </span>
            <button
              @click="refreshAllRates"
              :disabled="isRefreshing"
              class="inline-flex items-center px-4 py-2 text-sm font-semibold text-blueprint-text bg-blueprint-surface border border-solid border-blueprint-border hover:bg-blueprint-bg transition-colors duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg 
                class="w-4 h-4 mr-2" 
                :class="{ 'animate-spin': isRefreshing }"
                fill="none" 
                stroke="currentColor" 
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
              </svg>
              {{ isRefreshing ? 'Refreshing...' : 'Refresh Rates' }}
            </button>
          </div>
        </div>
      </div>

      <!-- Market Overview Cards -->
      <div class="grid grid-cols-1 gap-4 mb-8 md:grid-cols-4">
        <div class="bg-blueprint-surface border border-solid border-blueprint-border p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-sans font-medium text-blueprint-text-secondary uppercase tracking-wide">Total Pairs</p>
              <p class="text-2xl font-mono font-bold text-blueprint-text mt-1">{{ marketStats.totalPairs }}</p>
            </div>
            <div class="w-10 h-10 bg-blueprint-primary border border-solid border-blueprint-border flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-blueprint-surface border border-solid border-blueprint-border p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-sans font-medium text-blueprint-text-secondary uppercase tracking-wide">Gainers (24h)</p>
              <p class="text-2xl font-mono font-bold text-blueprint-primary mt-1">{{ marketStats.gainers }}</p>
            </div>
            <div class="w-10 h-10 bg-blueprint-primary border border-solid border-blueprint-border flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6"/>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-blueprint-surface border border-solid border-blueprint-border p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-sans font-medium text-blueprint-text-secondary uppercase tracking-wide">Losers (24h)</p>
              <p class="text-2xl font-mono font-bold text-blueprint-error mt-1">{{ marketStats.losers }}</p>
            </div>
            <div class="w-10 h-10 bg-blueprint-error border border-solid border-blueprint-border flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M13 17h8m0 0V9m0 8l-8-8-4 4-6-6"/>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-blueprint-surface border border-solid border-blueprint-border p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs font-sans font-medium text-blueprint-text-secondary uppercase tracking-wide">Volatile Pairs</p>
              <p class="text-2xl font-mono font-bold text-blueprint-warning mt-1">{{ marketStats.volatile }}</p>
            </div>
            <div class="w-10 h-10 bg-blueprint-warning border border-solid border-blueprint-border flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-blueprint-surface border border-solid border-blueprint-border p-4 mb-6">
        <div class="flex flex-col lg:flex-row lg:items-center gap-4">
          <!-- Search Input -->
          <div class="flex-1">
            <div class="relative">
              <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none">
                <svg class="w-4 h-4 text-blueprint-text-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
                </svg>
              </div>
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search currency pairs (e.g., USD, EUR, GBP)"
                class="block w-full pl-10 pr-4 py-2 font-mono text-sm text-blueprint-text placeholder-blueprint-text-secondary bg-blueprint-surface border border-solid border-blueprint-border focus:outline-none focus:border-blueprint-primary transition-all duration-200"
              />
            </div>
          </div>

          <!-- Base Currency Filter -->
          <div class="flex items-center gap-2">
            <label class="text-xs font-sans font-medium text-blueprint-text-secondary uppercase tracking-wide whitespace-nowrap">Base:</label>
            <select 
              v-model="selectedBaseCurrency"
              class="bg-blueprint-surface border border-solid border-blueprint-border text-blueprint-text font-mono text-sm px-3 py-2 focus:outline-none focus:border-blueprint-primary"
            >
              <option value="">All Currencies</option>
              <option v-for="currency in popularCurrencies" :key="currency" :value="currency">
                {{ currency }}
              </option>
            </select>
          </div>

          <!-- View Mode Selector -->
          <div class="flex items-center gap-2">
            <label class="text-xs font-sans font-medium text-blueprint-text-secondary uppercase tracking-wide whitespace-nowrap">View:</label>
            <div class="flex border border-solid border-blueprint-border">
              <button
                @click="viewMode = 'grid'"
                :class="[
                  'px-3 py-2 text-xs font-medium transition-colors',
                  viewMode === 'grid' 
                    ? 'bg-blueprint-primary text-white' 
                    : 'bg-blueprint-surface text-blueprint-text hover:bg-blueprint-bg'
                ]"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z"/>
                </svg>
              </button>
              <button
                @click="viewMode = 'table'"
                :class="[
                  'px-3 py-2 text-xs font-medium transition-colors',
                  viewMode === 'table' 
                    ? 'bg-blueprint-primary text-white' 
                    : 'bg-blueprint-surface text-blueprint-text hover:bg-blueprint-bg'
                ]"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M4 6h16M4 10h16M4 14h16M4 18h16"/>
                </svg>
              </button>
            </div>
          </div>

          <!-- Sort Selector -->
          <div class="flex items-center gap-2">
            <label class="text-xs font-sans font-medium text-blueprint-text-secondary uppercase tracking-wide whitespace-nowrap">Sort:</label>
            <select 
              v-model="sortBy"
              class="bg-blueprint-surface border border-solid border-blueprint-border text-blueprint-text text-sm px-3 py-2 focus:outline-none focus:border-blueprint-primary"
            >
              <option value="pair">Currency Pair</option>
              <option value="rate">Exchange Rate</option>
              <option value="change">24h Change</option>
              <option value="volume">Volume</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="flex items-center justify-center py-20">
        <div class="text-center">
          <div class="inline-block h-12 w-12 animate-spin border-4 border-blueprint-primary border-r-transparent"></div>
          <p class="mt-4 text-sm font-mono text-blueprint-text-secondary">Loading exchange rates...</p>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="bg-blueprint-surface border-2 border-blueprint-error p-8 text-center">
        <div class="text-blueprint-error text-5xl mb-4">⚠</div>
        <h3 class="text-lg font-semibold text-blueprint-text mb-2">Failed to Load Exchange Rates</h3>
        <p class="text-sm text-blueprint-text-secondary mb-6">{{ error }}</p>
        <button
          @click="fetchRates"
          class="inline-flex items-center px-6 py-2 text-sm font-semibold text-white bg-blueprint-primary border border-solid border-blueprint-border hover:bg-opacity-90 transition-colors"
        >
          Try Again
        </button>
      </div>

      <!-- Grid View -->
      <div v-else-if="viewMode === 'grid'" class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
        <div
          v-for="rate in filteredRates"
          :key="`${rate.baseCurrency}-${rate.targetCurrency}`"
          @click="openRateDetails(rate)"
          class="bg-blueprint-surface border border-solid border-blueprint-border p-5 hover:border-blueprint-primary transition-all duration-200 cursor-pointer group"
        >
          <!-- Currency Pair Header -->
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center gap-2">
              <span class="text-sm font-mono font-bold text-blueprint-text">
                {{ rate.baseCurrency }}
              </span>
              <svg class="w-3 h-3 text-blueprint-text-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M9 5l7 7-7 7"/>
              </svg>
              <span class="text-sm font-mono font-bold text-blueprint-text">
                {{ rate.targetCurrency }}
              </span>
            </div>
            <div 
              class="text-xs font-mono px-2 py-1 border border-solid"
              :class="getRateChangeClass(rate.change24h)"
            >
              {{ formatChangePercent(rate.change24h) }}
            </div>
          </div>

          <!-- Exchange Rate -->
          <div class="mb-3">
            <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-1">Exchange Rate</p>
            <p class="text-2xl font-mono font-bold text-blueprint-text">
              {{ formatRate(rate.rate) }}
            </p>
          </div>

          <!-- Additional Info -->
          <div class="grid grid-cols-2 gap-3 pt-3 border-t border-solid border-blueprint-border">
            <div>
              <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-1">24h High</p>
              <p class="text-sm font-mono font-semibold text-blueprint-text">{{ formatRate(rate.high24h) }}</p>
            </div>
            <div>
              <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-1">24h Low</p>
              <p class="text-sm font-mono font-semibold text-blueprint-text">{{ formatRate(rate.low24h) }}</p>
            </div>
          </div>

          <!-- Action Button (shows on hover) -->
          <div class="mt-4 pt-3 border-t border-solid border-blueprint-border opacity-0 group-hover:opacity-100 transition-opacity">
            <button 
              class="w-full text-xs font-semibold text-blueprint-primary hover:text-blueprint-text transition-colors text-center"
            >
              View Details →
            </button>
          </div>
        </div>
      </div>

      <!-- Table View -->
      <div v-else-if="viewMode === 'table'" class="bg-blueprint-surface border border-solid border-blueprint-border overflow-hidden">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-blueprint-bg border-b border-solid border-blueprint-border">
              <tr>
                <th class="px-4 py-3 text-left text-xs font-sans font-semibold text-blueprint-text-secondary uppercase tracking-wide">
                  Currency Pair
                </th>
                <th class="px-4 py-3 text-right text-xs font-sans font-semibold text-blueprint-text-secondary uppercase tracking-wide">
                  Rate
                </th>
                <th class="px-4 py-3 text-right text-xs font-sans font-semibold text-blueprint-text-secondary uppercase tracking-wide">
                  24h Change
                </th>
                <th class="px-4 py-3 text-right text-xs font-sans font-semibold text-blueprint-text-secondary uppercase tracking-wide">
                  24h High
                </th>
                <th class="px-4 py-3 text-right text-xs font-sans font-semibold text-blueprint-text-secondary uppercase tracking-wide">
                  24h Low
                </th>
                <th class="px-4 py-3 text-right text-xs font-sans font-semibold text-blueprint-text-secondary uppercase tracking-wide">
                  Volume
                </th>
                <th class="px-4 py-3 text-center text-xs font-sans font-semibold text-blueprint-text-secondary uppercase tracking-wide">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="divide-y divide-blueprint-border">
              <tr 
                v-for="rate in filteredRates" 
                :key="`${rate.baseCurrency}-${rate.targetCurrency}`"
                class="hover:bg-blueprint-bg transition-colors cursor-pointer"
                @click="openRateDetails(rate)"
              >
                <td class="px-4 py-3">
                  <div class="flex items-center gap-2">
                    <span class="text-sm font-mono font-bold text-blueprint-text">
                      {{ rate.baseCurrency }}/{{ rate.targetCurrency }}
                    </span>
                  </div>
                </td>
                <td class="px-4 py-3 text-right">
                  <span class="text-sm font-mono font-semibold text-blueprint-text">
                    {{ formatRate(rate.rate) }}
                  </span>
                </td>
                <td class="px-4 py-3 text-right">
                  <div 
                    class="inline-flex items-center text-xs font-mono px-2 py-1 border border-solid"
                    :class="getRateChangeClass(rate.change24h)"
                  >
                    <svg 
                      v-if="rate.change24h > 0" 
                      class="w-3 h-3 mr-1" 
                      fill="none" 
                      stroke="currentColor" 
                      viewBox="0 0 24 24"
                    >
                      <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M5 10l7-7m0 0l7 7m-7-7v18"/>
                    </svg>
                    <svg 
                      v-else-if="rate.change24h < 0" 
                      class="w-3 h-3 mr-1" 
                      fill="none" 
                      stroke="currentColor" 
                      viewBox="0 0 24 24"
                    >
                      <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M19 14l-7 7m0 0l-7-7m7 7V3"/>
                    </svg>
                    {{ formatChangePercent(rate.change24h) }}
                  </div>
                </td>
                <td class="px-4 py-3 text-right">
                  <span class="text-sm font-mono text-blueprint-text">{{ formatRate(rate.high24h) }}</span>
                </td>
                <td class="px-4 py-3 text-right">
                  <span class="text-sm font-mono text-blueprint-text">{{ formatRate(rate.low24h) }}</span>
                </td>
                <td class="px-4 py-3 text-right">
                  <span class="text-sm font-mono text-blueprint-text-secondary">{{ formatVolume(rate.volume) }}</span>
                </td>
                <td class="px-4 py-3">
                  <div class="flex items-center justify-center gap-2">
                    <button
                      @click.stop="createAlertForPair(rate.baseCurrency, rate.targetCurrency)"
                      class="p-1 text-blueprint-text-secondary hover:text-blueprint-primary transition-colors"
                      title="Create Alert"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4"/>
                      </svg>
                    </button>
                    <button
                      @click.stop="openRateDetails(rate)"
                      class="p-1 text-blueprint-text-secondary hover:text-blueprint-primary transition-colors"
                      title="View Details"
                    >
                      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>
              <tr v-if="filteredRates.length === 0">
                <td colspan="7" class="px-4 py-8 text-center">
                  <p class="text-sm font-mono text-blueprint-text-secondary">No exchange rates match your search criteria</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Rate Details Modal -->
      <Teleport to="body">
        <Transition name="modal">
          <div 
            v-if="showDetailsModal && selectedRate"
            class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50"
            @click.self="closeDetailsModal"
          >
            <div class="bg-blueprint-surface border-2 border-blueprint-border max-w-4xl w-full max-h-[90vh] overflow-y-auto">
              <!-- Modal Header -->
              <div class="sticky top-0 bg-blueprint-surface border-b border-solid border-blueprint-border px-6 py-4 flex items-center justify-between">
                <div>
                  <h2 class="text-xl font-mono font-bold text-blueprint-text">
                    {{ selectedRate.baseCurrency }}/{{ selectedRate.targetCurrency }}
                  </h2>
                  <p class="text-xs font-sans text-blueprint-text-secondary mt-1">
                    Detailed Exchange Rate Information
                  </p>
                </div>
                <button
                  @click="closeDetailsModal"
                  class="text-blueprint-text-secondary hover:text-blueprint-text transition-colors"
                >
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                  </svg>
                </button>
              </div>

              <!-- Modal Content -->
              <div class="p-6">
                <!-- Current Rate Section -->
                <div class="bg-blueprint-bg border border-solid border-blueprint-border p-6 mb-6">
                  <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div>
                      <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-2">Current Rate</p>
                      <p class="text-3xl font-mono font-bold text-blueprint-text">
                        {{ formatRate(selectedRate.rate) }}
                      </p>
                    </div>
                    <div>
                      <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-2">24h Change</p>
                      <div 
                        class="inline-flex items-center text-lg font-mono font-bold px-3 py-1 border border-solid"
                        :class="getRateChangeClass(selectedRate.change24h)"
                      >
                        {{ formatChangePercent(selectedRate.change24h) }}
                      </div>
                    </div>
                    <div>
                      <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-2">Last Updated</p>
                      <p class="text-sm font-mono text-blueprint-text">
                        {{ formatTime(selectedRate.timestamp) }}
                      </p>
                    </div>
                  </div>
                </div>

                <!-- 24h Statistics -->
                <div class="mb-6">
                  <h3 class="text-sm font-sans font-semibold text-blueprint-text uppercase tracking-wide mb-4">24-Hour Statistics</h3>
                  <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div class="bg-blueprint-surface border border-solid border-blueprint-border p-4">
                      <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-1">High</p>
                      <p class="text-lg font-mono font-bold text-blueprint-primary">{{ formatRate(selectedRate.high24h) }}</p>
                    </div>
                    <div class="bg-blueprint-surface border border-solid border-blueprint-border p-4">
                      <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-1">Low</p>
                      <p class="text-lg font-mono font-bold text-blueprint-error">{{ formatRate(selectedRate.low24h) }}</p>
                    </div>
                    <div class="bg-blueprint-surface border border-solid border-blueprint-border p-4">
                      <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-1">Open</p>
                      <p class="text-lg font-mono font-bold text-blueprint-text">{{ formatRate(selectedRate.open24h) }}</p>
                    </div>
                    <div class="bg-blueprint-surface border border-solid border-blueprint-border p-4">
                      <p class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-1">Volume</p>
                      <p class="text-lg font-mono font-bold text-blueprint-text">{{ formatVolume(selectedRate.volume) }}</p>
                    </div>
                  </div>
                </div>

                <!-- Historical Chart -->
                <div class="mb-6">
                  <div class="flex items-center justify-between mb-4">
                    <h3 class="text-sm font-sans font-semibold text-blueprint-text uppercase tracking-wide">Price Chart</h3>
                    <div class="flex items-center gap-2">
                      <label class="text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide">Period:</label>
                      <select 
                        v-model="chartPeriod"
                        @change="loadChartData"
                        class="bg-blueprint-surface border border-solid border-blueprint-border text-blueprint-text text-xs px-2 py-1 focus:outline-none focus:border-blueprint-primary"
                      >
                        <option value="7">7 Days</option>
                        <option value="30">30 Days</option>
                        <option value="90">90 Days</option>
                      </select>
                    </div>
                  </div>
                  <div class="bg-blueprint-bg border border-solid border-blueprint-border p-4 relative" style="min-height: 348px;">
                    <!-- Loading Overlay -->
                    <div v-if="chartLoading" class="absolute inset-0 flex items-center justify-center bg-blueprint-bg z-10">
                      <div class="text-center">
                        <div class="inline-block h-8 w-8 animate-spin border-2 border-blueprint-primary border-r-transparent"></div>
                        <p class="mt-2 text-xs font-mono text-blueprint-text-secondary">Loading historical data...</p>
                      </div>
                    </div>
                    
                    <!-- Error Overlay -->
                    <div v-if="chartError && !chartLoading" class="absolute inset-0 flex items-center justify-center bg-blueprint-bg z-10">
                      <div class="text-center">
                        <p class="text-sm font-mono text-blueprint-error mb-4">{{ chartError }}</p>
                        <button
                          @click="loadChartData"
                          class="px-4 py-2 text-xs font-semibold text-blueprint-text bg-blueprint-surface border border-solid border-blueprint-border hover:bg-blueprint-bg transition-colors"
                        >
                          Retry
                        </button>
                      </div>
                    </div>
                    
                    <!-- Chart Canvas (always in DOM) -->
                    <div style="height: 300px; position: relative;">
                      <canvas ref="chartCanvas"></canvas>
                    </div>
                  </div>
                </div>

                <!-- Conversion Calculator -->
                <div class="mb-6">
                  <h3 class="text-sm font-sans font-semibold text-blueprint-text uppercase tracking-wide mb-4">Currency Converter</h3>
                  <div class="bg-blueprint-bg border border-solid border-blueprint-border p-6">
                    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 items-end">
                      <div>
                        <label class="block text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-2">
                          Amount
                        </label>
                        <input
                          v-model.number="conversionAmount"
                          type="number"
                          step="0.01"
                          class="w-full px-4 py-2 font-mono text-sm text-blueprint-text bg-blueprint-surface border border-solid border-blueprint-border focus:outline-none focus:border-blueprint-primary"
                          placeholder="1.00"
                        />
                      </div>
                      <div>
                        <label class="block text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-2">
                          {{ selectedRate.baseCurrency }}
                        </label>
                        <div class="px-4 py-2 font-mono text-sm text-blueprint-text bg-blueprint-surface border border-solid border-blueprint-border">
                          {{ formatRate(conversionAmount || 1) }}
                        </div>
                      </div>
                      <div>
                        <label class="block text-xs font-sans text-blueprint-text-secondary uppercase tracking-wide mb-2">
                          {{ selectedRate.targetCurrency }}
                        </label>
                        <div class="px-4 py-2 font-mono text-sm font-bold text-blueprint-primary bg-blueprint-surface border border-solid border-blueprint-border">
                          {{ formatRate((conversionAmount || 1) * selectedRate.rate) }}
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Action Buttons -->
                <div class="flex items-center justify-end gap-4 pt-4 border-t border-solid border-blueprint-border">
                  <button
                    @click="createAlertForPair(selectedRate.baseCurrency, selectedRate.targetCurrency)"
                    class="inline-flex items-center px-6 py-2 text-sm font-semibold text-white bg-blueprint-primary border border-solid border-blueprint-border hover:bg-opacity-90 transition-colors"
                  >
                    <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4"/>
                    </svg>
                    Create Alert
                  </button>
                  <button
                    @click="closeDetailsModal"
                    class="px-6 py-2 text-sm font-semibold text-blueprint-text bg-blueprint-surface border border-solid border-blueprint-border hover:bg-blueprint-bg transition-colors"
                  >
                    Close
                  </button>
                </div>
              </div>
            </div>
          </div>
        </Transition>
      </Teleport>

      <!-- Toast Notification -->
      <Toast
        :message="toastMessage"
        :type="toastType"
        :show="showToast"
        @close="showToast = false"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import { getLatestRates, refreshRates, getRateHistory, type ExchangeRate } from '@/services/exchangeRateService';
import Toast from '@/components/Toast.vue';
import { Chart, registerables, type ChartConfiguration } from 'chart.js';

// Register Chart.js components
Chart.register(...registerables);

// Router
const router = useRouter();

// State
const isLoading = ref(false);
const isRefreshing = ref(false);
const error = ref<string | null>(null);
const rates = ref<ExchangeRate[]>([]);
const lastUpdateTime = ref<string | null>(null);

// Filters and View
const searchQuery = ref('');
const selectedBaseCurrency = ref('');
const viewMode = ref<'grid' | 'table'>('table');
const sortBy = ref<'pair' | 'rate' | 'change' | 'volume'>('pair');

// Modal State
const showDetailsModal = ref(false);
const selectedRate = ref<any | null>(null);
const conversionAmount = ref(1);

// Chart State
const chartCanvas = ref<HTMLCanvasElement | null>(null);
const chartInstance = ref<Chart | null>(null);
const chartPeriod = ref(30);
const chartLoading = ref(false);
const chartError = ref<string | null>(null);

// Toast State
const showToast = ref(false);
const toastMessage = ref('');
const toastType = ref<'success' | 'error' | 'info'>('info');

// Popular Currencies
const popularCurrencies = [
  'USD', 'EUR', 'GBP', 'JPY', 'CHF', 'CAD', 'AUD', 'NZD',
  'CNY', 'INR', 'BRL', 'MXN', 'ZAR', 'RUB', 'SGD', 'HKD'
];

// TODO: This should be handled by the backend with actual data
const enhancedRates = computed(() => {
  return rates.value.map(rate => ({
    ...rate,
    // Mock additional data 
    change24h: (Math.random() * 10 - 5), // -5% to +5%
    high24h: rate.rate * (1 + Math.random() * 0.05),
    low24h: rate.rate * (1 - Math.random() * 0.05),
    open24h: rate.rate * (1 + (Math.random() * 0.04 - 0.02)),
    volume: Math.floor(Math.random() * 10000000) + 1000000,
  }));
});

// Filtered Rates
const filteredRates = computed(() => {
  let filtered = enhancedRates.value;

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toUpperCase();
    filtered = filtered.filter(rate => 
      rate.baseCurrency.includes(query) || 
      rate.targetCurrency.includes(query) ||
      `${rate.baseCurrency}/${rate.targetCurrency}`.includes(query)
    );
  }

  // Filter by base currency
  if (selectedBaseCurrency.value) {
    filtered = filtered.filter(rate => rate.baseCurrency === selectedBaseCurrency.value);
  }

  // Sort
  filtered = [...filtered].sort((a, b) => {
    switch (sortBy.value) {
      case 'rate':
        return b.rate - a.rate;
      case 'change':
        return (b.change24h || 0) - (a.change24h || 0);
      case 'volume':
        return (b.volume || 0) - (a.volume || 0);
      case 'pair':
      default:
        return `${a.baseCurrency}/${a.targetCurrency}`.localeCompare(`${b.baseCurrency}/${b.targetCurrency}`);
    }
  });

  return filtered;
});

// Market Statistics
const marketStats = computed(() => {
  const total = enhancedRates.value.length;
  const gainers = enhancedRates.value.filter(r => (r.change24h || 0) > 0).length;
  const losers = enhancedRates.value.filter(r => (r.change24h || 0) < 0).length;
  const volatile = enhancedRates.value.filter(r => Math.abs(r.change24h || 0) > 2).length;

  return { totalPairs: total, gainers, losers, volatile };
});

// Formatting Functions
const formatRate = (value: number | undefined): string => {
  if (value === undefined || value === null) return '0.0000';
  return value.toLocaleString('en-US', {
    minimumFractionDigits: 4,
    maximumFractionDigits: 4
  });
};

const formatChangePercent = (value: number | undefined): string => {
  if (value === undefined || value === null) return '0.00%';
  const sign = value >= 0 ? '+' : '';
  return `${sign}${value.toFixed(2)}%`;
};

const formatVolume = (value: number | undefined): string => {
  if (value === undefined || value === null) return 'N/A';
  if (value >= 1000000) {
    return `${(value / 1000000).toFixed(2)}M`;
  }
  if (value >= 1000) {
    return `${(value / 1000).toFixed(2)}K`;
  }
  return value.toString();
};

const formatTime = (timestamp: string): string => {
  try {
    const date = new Date(timestamp);
    return date.toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  } catch {
    return 'Invalid date';
  }
};

const getRateChangeClass = (change: number | undefined) => {
  if (!change) return 'border-blueprint-border text-blueprint-text-secondary bg-blueprint-surface';
  if (change > 0) return 'border-blueprint-primary text-blueprint-primary bg-green-50';
  if (change < 0) return 'border-blueprint-error text-blueprint-error bg-red-50';
  return 'border-blueprint-border text-blueprint-text-secondary bg-blueprint-surface';
};

// Data Fetching
const fetchRates = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    const response = await getLatestRates();
    rates.value = response.rates;
    lastUpdateTime.value = response.timestamp;
  } catch (err: any) {
    console.error('Failed to fetch rates:', err);
    error.value = err.response?.data?.message || err.message || 'Failed to load exchange rates. Please try again.';
    showToastMessage('Failed to load exchange rates', 'error');
  } finally {
    isLoading.value = false;
  }
};

const refreshAllRates = async () => {
  isRefreshing.value = true;
  
  try {
    await refreshRates();
    await fetchRates();
    showToastMessage('Exchange rates refreshed successfully', 'success');
  } catch (err: any) {
    console.error('Failed to refresh rates:', err);
    showToastMessage(
      err.response?.data?.message || 'Failed to refresh rates. You may need admin privileges.',
      'error'
    );
  } finally {
    isRefreshing.value = false;
  }
};

// Modal Functions
const openRateDetails = async (rate: any) => {
  selectedRate.value = rate;
  conversionAmount.value = 1;
  showDetailsModal.value = true;
  
  // Load chart data after modal is visible
  await nextTick();
  loadChartData();
};

const closeDetailsModal = () => {
  showDetailsModal.value = false;
  selectedRate.value = null;
  
  // Destroy chart instance
  if (chartInstance.value) {
    chartInstance.value.destroy();
    chartInstance.value = null;
  }
};

// Chart Functions
const loadChartData = async () => {
  if (!selectedRate.value) {
    console.log('No selected rate');
    return;
  }
  
  console.log('Loading chart data for:', selectedRate.value.baseCurrency, selectedRate.value.targetCurrency, 'Period:', chartPeriod.value);
  
  chartLoading.value = true;
  chartError.value = null;
  
  try {
    const response = await getRateHistory(
      selectedRate.value.baseCurrency,
      selectedRate.value.targetCurrency,
      Number(chartPeriod.value)
    );
    
    console.log('History response:', response);
    
    if (response.history && response.history.length > 0) {
      // Wait for next tick to ensure canvas is in DOM
      await nextTick();
      await new Promise(resolve => setTimeout(resolve, 50)); // Extra delay to ensure DOM is ready
      
      if (chartCanvas.value) {
        console.log('Canvas found, rendering chart');
        renderChart(response.history);
      } else {
        console.error('Canvas still not available after waiting');
        chartError.value = 'Chart canvas not ready. Please try again.';
      }
    } else {
      chartError.value = 'No historical data available for this currency pair';
    }
  } catch (err: any) {
    console.error('Failed to load chart data:', err);
    chartError.value = err.response?.data?.message || err.message || 'Failed to load historical data';
  } finally {
    chartLoading.value = false;
  }
};

const renderChart = (history: ExchangeRate[]) => {
  if (!chartCanvas.value) {
    console.error('Canvas element not found');
    return;
  }
  
  console.log('Rendering chart with', history.length, 'data points');
  
  // Destroy existing chart
  if (chartInstance.value) {
    chartInstance.value.destroy();
  }
  
  // Sort by timestamp ascending
  const sortedHistory = [...history].sort((a, b) => 
    new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
  );
  
  // Prepare data
  const labels = sortedHistory.map(item => {
    const date = new Date(item.timestamp);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  });
  
  const rates = sortedHistory.map(item => item.rate);
  
  console.log('Chart labels:', labels);
  console.log('Chart rates:', rates);
  
  // Calculate min/max for better scaling
  const minRate = Math.min(...rates);
  const maxRate = Math.max(...rates);
  const padding = (maxRate - minRate) * 0.1 || maxRate * 0.05; // Fallback if no variation
  
  const config: ChartConfiguration = {
    type: 'line',
    data: {
      labels,
      datasets: [
        {
          label: 'Exchange Rate',
          data: rates,
          borderColor: '#22C55E',
          backgroundColor: 'rgba(34, 197, 94, 0.1)',
          borderWidth: 2,
          fill: true,
          tension: 0,
          pointRadius: 3,
          pointBackgroundColor: '#22C55E',
          pointBorderColor: '#FFFFFF',
          pointBorderWidth: 2,
          pointHoverRadius: 5,
          pointHoverBackgroundColor: '#22C55E',
          pointHoverBorderColor: '#FFFFFF',
          pointHoverBorderWidth: 2,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false,
        },
        tooltip: {
          backgroundColor: '#FFFFFF',
          titleColor: '#0F172A',
          bodyColor: '#0F172A',
          borderColor: '#E2E8F0',
          borderWidth: 1,
          padding: 12,
          displayColors: false,
          callbacks: {
            label: function(context) {
              return 'Rate: ' + context.parsed.y.toFixed(4);
            },
          },
        },
      },
      scales: {
        x: {
          grid: {
            display: true,
            color: '#E2E8F0',
          },
          ticks: {
            color: '#475569',
            font: {
              family: 'Roboto Mono, Consolas, monospace',
              size: 10,
            },
          },
        },
        y: {
          grid: {
            display: true,
            color: '#E2E8F0',
          },
          ticks: {
            color: '#475569',
            font: {
              family: 'Roboto Mono, Consolas, monospace',
              size: 10,
            },
            callback: function(value) {
              return typeof value === 'number' ? value.toFixed(4) : value;
            },
          },
          min: minRate - padding,
          max: maxRate + padding,
        },
      },
    },
  };
  
  try {
    chartInstance.value = new Chart(chartCanvas.value, config);
    console.log('Chart created successfully');
  } catch (error) {
    console.error('Error creating chart:', error);
  }
};

// Navigation Functions
const createAlertForPair = (baseCurrency: string, targetCurrency: string) => {
  router.push({
    name: 'alerts',
    query: { base: baseCurrency, target: targetCurrency }
  });
};

// Toast Functions
const showToastMessage = (message: string, type: 'success' | 'error' | 'info') => {
  toastMessage.value = message;
  toastType.value = type;
  showToast.value = true;
};

// Lifecycle
onMounted(() => {
  fetchRates();
});
</script>

<style scoped>
/* Modal Transitions */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .bg-blueprint-surface,
.modal-leave-active .bg-blueprint-surface {
  transition: transform 0.3s ease;
}

.modal-enter-from .bg-blueprint-surface,
.modal-leave-to .bg-blueprint-surface {
  transform: scale(0.95);
}

/* Custom Scrollbar */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #F8FAFC;
}

::-webkit-scrollbar-thumb {
  background: #E2E8F0;
  border-radius: 0;
}

::-webkit-scrollbar-thumb:hover {
  background: #CBD5E1;
}
</style>
