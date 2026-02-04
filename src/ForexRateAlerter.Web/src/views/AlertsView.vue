<template>
  <div class="min-h-screen bg-slate-50">
    <div class="px-6 py-8 mx-auto max-w-7xl lg:px-8">
      <!-- Header Section -->
      <div class="mb-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between">
          <div class="text-center md:text-left mb-4 md:mb-0">
            <h1 class="text-2xl sm:text-3xl font-bold tracking-tight text-slate-900 mb-2">
              Forex Rate Alerts
            </h1>
            <p class="text-sm sm:text-base text-gray-600">
              Manage your currency pair alerts and monitor exchange rate thresholds
            </p>
          </div>
          <button
            @click="openCreateModal"
            class="inline-flex items-center justify-center px-4 py-3 sm:px-6 text-sm font-semibold text-white bg-green-500 border border-solid border-gray-300 hover:bg-green-600 active:bg-green-700 transition-colors duration-200 w-full md:w-auto touch-manipulation"
          >
            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
            </svg>
            Create New Alert
          </button>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="grid grid-cols-1 gap-4 mb-8 md:grid-cols-4">
        <div class="bg-white border border-solid border-gray-300 p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Total Alerts</p>
              <p class="text-2xl font-mono font-bold text-slate-900 mt-1">{{ stats.total }}</p>
            </div>
            <div class="w-10 h-10 bg-gray-100 border border-solid border-gray-300 flex items-center justify-center">
              <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4m-6 2a9 9 0 110-18 9 9 0 010 18z"/>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white border border-solid border-gray-300 p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Active</p>
              <p class="text-2xl font-mono font-bold text-green-600 mt-1">{{ stats.active }}</p>
            </div>
            <div class="w-10 h-10 bg-green-50 border border-solid border-green-200 flex items-center justify-center">
              <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white border border-solid border-gray-300 p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Inactive</p>
              <p class="text-2xl font-mono font-bold text-gray-500 mt-1">{{ stats.inactive }}</p>
            </div>
            <div class="w-10 h-10 bg-gray-50 border border-solid border-gray-300 flex items-center justify-center">
              <svg class="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white border border-solid border-gray-300 p-5">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Triggered Today</p>
              <p class="text-2xl font-mono font-bold text-slate-900 mt-1">{{ stats.triggeredToday }}</p>
            </div>
            <div class="w-10 h-10 bg-gray-100 border border-solid border-gray-300 flex items-center justify-center">
              <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4"/>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white border border-solid border-gray-300 p-4 mb-6">
        <div class="flex flex-col gap-4">
          <div class="flex-1">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by currency pair (e.g., USD/EUR)"
              class="block w-full px-3 py-2.5 sm:px-4 sm:py-2 text-base text-slate-900 placeholder-gray-500 bg-white border border-solid border-gray-300 focus:outline-none focus:border-green-500 transition-all duration-200"
            />
          </div>
          <div class="flex gap-2 overflow-x-auto pb-1">
            <button
              @click="filterStatus = 'all'"
              :class="[
                'px-4 py-2.5 text-sm font-medium transition-colors duration-200 whitespace-nowrap touch-manipulation',
                filterStatus === 'all'
                  ? 'bg-gray-100 text-slate-900 border border-solid border-gray-300'
                  : 'text-gray-700 hover:bg-gray-50 active:bg-gray-100 border border-solid border-transparent'
              ]"
            >
              All
            </button>
            <button
              @click="filterStatus = 'active'"
              :class="[
                'px-4 py-2.5 text-sm font-medium transition-colors duration-200 whitespace-nowrap touch-manipulation',
                filterStatus === 'active'
                  ? 'bg-green-50 text-green-700 border border-solid border-green-200'
                  : 'text-gray-700 hover:bg-gray-50 active:bg-gray-100 border border-solid border-transparent'
              ]"
            >
              Active
            </button>
            <button
              @click="filterStatus = 'inactive'"
              :class="[
                'px-4 py-2.5 text-sm font-medium transition-colors duration-200 whitespace-nowrap touch-manipulation',
                filterStatus === 'inactive'
                  ? 'bg-gray-100 text-slate-900 border border-solid border-gray-300'
                  : 'text-gray-700 hover:bg-gray-50 active:bg-gray-100 border border-solid border-transparent'
              ]"
            >
              Inactive
            </button>
          </div>
        </div>
      </div>

      <!-- Alerts List -->
      <div v-if="loading" class="bg-white border border-solid border-gray-300 p-12">
        <div class="flex flex-col items-center justify-center">
          <svg class="animate-spin h-10 w-10 text-green-500 mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          <p class="text-gray-600">Loading alerts...</p>
        </div>
      </div>

      <div v-else-if="filteredAlerts.length === 0" class="bg-white border border-solid border-gray-300 p-12">
        <div class="text-center">
          <div class="mx-auto w-16 h-16 bg-gray-100 border border-solid border-gray-300 flex items-center justify-center mb-4">
            <svg class="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-5 5v-5zM9 12l2 2 4-4m-6 2a9 9 0 110-18 9 9 0 010 18z"/>
            </svg>
          </div>
          <h3 class="text-lg font-semibold text-slate-900 mb-2">No alerts found</h3>
          <p class="text-gray-600 mb-6">
            {{ searchQuery || filterStatus !== 'all' ? 'Try adjusting your filters' : 'Get started by creating your first alert' }}
          </p>
          <button
            v-if="!searchQuery && filterStatus === 'all'"
            @click="openCreateModal"
            class="inline-flex items-center px-6 py-3 text-sm font-semibold text-white bg-green-500 border border-solid border-gray-300 hover:bg-green-600 transition-colors duration-200"
          >
            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
            </svg>
            Create Your First Alert
          </button>
        </div>
      </div>

      <!-- Alerts Display (Desktop Table & Mobile Cards) -->
      <div v-else>
        <!-- Desktop Table View -->
        <div class="hidden md:block bg-white border border-solid border-gray-300">
          <!-- Table Header -->
          <div class="grid grid-cols-12 gap-4 px-6 py-4 bg-gray-50 border-b border-solid border-gray-300 text-sm font-semibold text-gray-700">
            <div class="col-span-3">Currency Pair</div>
            <div class="col-span-2">Condition</div>
            <div class="col-span-2">Target Rate</div>
            <div class="col-span-2">Status</div>
            <div class="col-span-2">Last Triggered</div>
            <div class="col-span-1 text-right">Actions</div>
          </div>

          <!-- Table Body -->
          <div
            v-for="alert in filteredAlerts"
            :key="alert.id"
            class="grid grid-cols-12 gap-4 px-6 py-4 border-b border-solid border-gray-200 hover:bg-gray-50 transition-colors duration-150"
          >
            <div class="col-span-3 flex items-center">
              <div class="w-8 h-8 bg-gray-100 border border-solid border-gray-300 flex items-center justify-center mr-3">
                <svg class="w-4 h-4 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <span class="text-base font-mono font-bold text-slate-900">
                {{ alert.baseCurrency }}/{{ alert.targetCurrency }}
              </span>
            </div>

            <div class="col-span-2 flex items-center">
              <span class="px-3 py-1 text-sm font-medium bg-gray-100 border border-solid border-gray-300 text-gray-700">
                {{ formatCondition(alert.condition) }}
              </span>
            </div>

            <div class="col-span-2 flex items-center">
              <span class="text-base font-mono font-bold text-slate-900">
                {{ formatRate(alert.targetRate) }}
              </span>
            </div>

            <div class="col-span-2 flex items-center">
              <span
                :class="[
                  'inline-flex items-center px-3 py-1 text-sm font-medium border border-solid',
                  alert.isActive
                    ? 'bg-green-50 border-green-200 text-green-700'
                    : 'bg-gray-50 border-gray-300 text-gray-600'
                ]"
              >
                <span
                  :class="[
                    'w-2 h-2 mr-2',
                    alert.isActive ? 'bg-green-500' : 'bg-gray-400'
                  ]"
                ></span>
                {{ alert.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>

            <div class="col-span-2 flex items-center">
              <span class="text-sm text-gray-600">
                {{ alert.lastTriggeredAt ? formatDate(alert.lastTriggeredAt) : 'Never' }}
              </span>
            </div>

            <div class="col-span-1 flex items-center justify-end gap-2">
              <button
                @click="toggleAlertStatus(alert)"
                :title="alert.isActive ? 'Deactivate' : 'Activate'"
                class="p-2 text-gray-600 hover:text-slate-900 hover:bg-gray-100 border border-solid border-transparent hover:border-gray-300 transition-all duration-200"
              >
                <svg v-if="alert.isActive" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z"/>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </button>

              <button
                @click="openEditModal(alert)"
                title="Edit"
                class="p-2 text-gray-600 hover:text-slate-900 hover:bg-gray-100 border border-solid border-transparent hover:border-gray-300 transition-all duration-200"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                </svg>
              </button>

              <button
                @click="confirmDeleteAlert(alert)"
                title="Delete"
                class="p-2 text-red-600 hover:text-red-700 hover:bg-red-50 border border-solid border-transparent hover:border-red-200 transition-all duration-200"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                </svg>
              </button>
            </div>
          </div>
        </div>

        <!-- Mobile Card View -->
        <div class="md:hidden space-y-4">
        <div
          v-for="alert in filteredAlerts"
          :key="alert.id"
          class="bg-white border border-solid border-gray-300 p-4"
        >
          <!-- Card Header -->
          <div class="flex items-start justify-between mb-4">
            <div class="flex items-center flex-1">
              <div class="w-10 h-10 bg-gray-100 border border-solid border-gray-300 flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-base font-mono font-bold text-slate-900">
                  {{ alert.baseCurrency }}/{{ alert.targetCurrency }}
                </h3>
                <p class="text-xs text-gray-600 mt-1">
                  {{ formatCondition(alert.condition) }} {{ formatRate(alert.targetRate) }}
                </p>
              </div>
            </div>
          </div>

          <!-- Card Details -->
          <div class="space-y-2 mb-4 pb-4 border-b border-solid border-gray-200">
            <div class="flex justify-between items-center">
              <span class="text-xs font-semibold text-gray-600">Status</span>
              <span
                :class="[
                  'inline-flex items-center px-2 py-1 text-xs font-medium border border-solid',
                  alert.isActive
                    ? 'bg-green-50 border-green-200 text-green-700'
                    : 'bg-gray-50 border-gray-300 text-gray-600'
                ]"
              >
                <span
                  :class="[
                    'w-1.5 h-1.5 mr-1.5',
                    alert.isActive ? 'bg-green-500' : 'bg-gray-400'
                  ]"
                ></span>
                {{ alert.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-xs font-semibold text-gray-600">Last Triggered</span>
              <span class="text-xs text-gray-600">
                {{ alert.lastTriggeredAt ? formatDate(alert.lastTriggeredAt) : 'Never' }}
              </span>
            </div>
          </div>

          <!-- Card Actions -->
          <div class="grid grid-cols-3 gap-2">
            <button
              @click="toggleAlertStatus(alert)"
              class="inline-flex items-center justify-center px-3 py-2.5 text-xs font-semibold text-gray-700 bg-white border border-solid border-gray-300 hover:bg-gray-50 active:bg-gray-100 transition-colors duration-200 touch-manipulation"
            >
              <svg v-if="alert.isActive" class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
              <svg v-else class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z"/>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
              {{ alert.isActive ? 'Pause' : 'Resume' }}
            </button>
            <button
              @click="openEditModal(alert)"
              class="inline-flex items-center justify-center px-3 py-2.5 text-xs font-semibold text-gray-700 bg-white border border-solid border-gray-300 hover:bg-gray-50 active:bg-gray-100 transition-colors duration-200 touch-manipulation"
            >
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
              </svg>
              Edit
            </button>
            <button
              @click="confirmDeleteAlert(alert)"
              class="inline-flex items-center justify-center px-3 py-2.5 text-xs font-semibold text-red-600 bg-white border border-solid border-red-200 hover:bg-red-50 active:bg-red-100 transition-colors duration-200 touch-manipulation"
            >
              <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
              </svg>
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
    </div>

    <!-- Create/Edit Modal -->
    <AlertForm
      :is-open="showModal"
      :is-edit="isEditMode"
      :initial-data="alertToEdit"
      @close="closeModal"
      @submit="handleAlertSubmit"
    />

    <!-- Delete Confirmation Modal -->
    <div
      v-if="showDeleteModal"
      class="fixed inset-0 z-50 overflow-y-auto"
      @click.self="closeDeleteModal"
    >
      <div class="flex items-center justify-center min-h-screen px-2 sm:px-4">
        <div class="fixed inset-0 bg-black bg-opacity-50 transition-opacity"></div>
        
        <div class="relative bg-white border border-solid border-gray-300 w-full max-w-md p-6 sm:p-8 z-10" @click.stop>
          <!-- Modal Header -->
          <div class="mb-6">
            <div class="w-12 h-12 bg-red-100 border border-solid border-red-200 flex items-center justify-center mb-4">
              <svg class="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
              </svg>
            </div>
            <h2 class="text-xl font-bold text-slate-900 mb-2">Delete Alert</h2>
            <p class="text-gray-600">
              Are you sure you want to delete this alert? This action cannot be undone.
            </p>
            <div v-if="alertToDelete" class="mt-4 p-4 bg-gray-50 border border-solid border-gray-200">
              <p class="text-sm font-mono font-bold text-slate-900">
                {{ alertToDelete.baseCurrency }}/{{ alertToDelete.targetCurrency }}
              </p>
              <p class="text-sm text-gray-600 mt-1">
                {{ formatCondition(alertToDelete.condition) }} {{ formatRate(alertToDelete.targetRate) }}
              </p>
            </div>
          </div>

          <!-- Modal Actions -->
          <div class="flex flex-col sm:flex-row gap-3">
            <button
              @click="deleteAlert"
              :disabled="deleting"
              class="flex-1 inline-flex justify-center items-center px-6 py-3 text-sm font-semibold text-white bg-red-600 border border-solid border-red-600 hover:bg-red-700 active:bg-red-800 focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 touch-manipulation order-1 sm:order-1"
            >
              <span v-if="!deleting">Delete Alert</span>
              <span v-else class="flex items-center">
                <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Deleting...
              </span>
            </button>
            <button
              @click="closeDeleteModal"
              :disabled="deleting"
              class="flex-1 inline-flex justify-center items-center px-6 py-3 text-sm font-semibold text-gray-700 bg-white border border-solid border-gray-300 hover:bg-gray-50 active:bg-gray-100 focus:outline-none disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 touch-manipulation order-2 sm:order-2"
            >
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { getUserAlerts, createAlert as createAlertAPI, updateAlert as updateAlertAPI, deleteAlert as deleteAlertAPI } from '@/services/alertService';
import AlertForm, { type AlertFormData } from '@/components/AlertForm.vue';
import { formatRate, formatCondition, formatDate } from '@/utils/formatters';

interface Alert {
  id: number;
  baseCurrency: string;
  targetCurrency: string;
  condition: string;
  targetRate: number;
  isActive: boolean;
  createdAt: string;
  lastTriggeredAt: string | null;
}

const alerts = ref<Alert[]>([]);
const loading = ref(true);
const searchQuery = ref('');
const filterStatus = ref<'all' | 'active' | 'inactive'>('all');

// Modal states
const showModal = ref(false);
const showDeleteModal = ref(false);
const isEditMode = ref(false);
const submitting = ref(false);
const deleting = ref(false);
const alertToDelete = ref<Alert | null>(null);

// Form data
const alertToEdit = ref<AlertFormData | null>(null);
const editingAlertId = ref<number | null>(null);

// Statistics
const stats = computed(() => {
  const total = alerts.value.length;
  const active = alerts.value.filter(a => a.isActive).length;
  const inactive = total - active;
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const triggeredToday = alerts.value.filter(a => {
    if (!a.lastTriggeredAt) return false;
    const triggeredDate = new Date(a.lastTriggeredAt);
    triggeredDate.setHours(0, 0, 0, 0);
    return triggeredDate.getTime() === today.getTime();
  }).length;

  return { total, active, inactive, triggeredToday };
});

// Filtered alerts
const filteredAlerts = computed(() => {
  let filtered = alerts.value;

  // Filter by status
  if (filterStatus.value === 'active') {
    filtered = filtered.filter(a => a.isActive);
  } else if (filterStatus.value === 'inactive') {
    filtered = filtered.filter(a => !a.isActive);
  }

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    filtered = filtered.filter(a => {
      const pair = `${a.baseCurrency}/${a.targetCurrency}`.toLowerCase();
      return pair.includes(query);
    });
  }

  return filtered;
});

// Load alerts
const loadAlerts = async () => {
  try {
    loading.value = true;
    const data = await getUserAlerts();
    alerts.value = data.alerts;
  } catch (error) {
    console.error('Failed to load alerts:', error);
  } finally {
    loading.value = false;
  }
};

// Modal management
const openCreateModal = () => {
  isEditMode.value = false;
  alertToEdit.value = null;
  showModal.value = true;
};

const openEditModal = (alert: Alert) => {
  isEditMode.value = true;
  editingAlertId.value = alert.id;
  
  // Map backend naming to form IDs if necessary
  const conditionMap: Record<string, string> = {
    'GreaterThan': '1',
    'LessThan': '2',
    'EqualTo': '3',
    '1': '1',
    '2': '2',
    '3': '3'
  };

  alertToEdit.value = {
    baseCurrency: alert.baseCurrency,
    targetCurrency: alert.targetCurrency,
    condition: conditionMap[alert.condition] || alert.condition,
    targetRate: alert.targetRate,
    isActive: alert.isActive,
  };
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  isEditMode.value = false;
  editingAlertId.value = null;
  alertToEdit.value = null;
};

const confirmDeleteAlert = (alert: Alert) => {
  alertToDelete.value = alert;
  showDeleteModal.value = true;
};

const closeDeleteModal = () => {
  showDeleteModal.value = false;
  alertToDelete.value = null;
};

// Submit alert (create or update)
// Map numeric condition to string enum
const mapCondition = (condition: string | number): 'GreaterThan' | 'LessThan' | 'EqualTo' => {
  const conditionStr = String(condition);
  const conditionMap: Record<string, 'GreaterThan' | 'LessThan' | 'EqualTo'> = {
    '1': 'GreaterThan',
    '2': 'LessThan',
    '3': 'EqualTo',
    'GreaterThan': 'GreaterThan',
    'LessThan': 'LessThan',
    'EqualTo': 'EqualTo',
  };
  return conditionMap[conditionStr] || 'GreaterThan';
};

const handleAlertSubmit = async (data: AlertFormData) => {
  if (submitting.value) return;

  try {
    submitting.value = true;
    
    if (isEditMode.value && editingAlertId.value) {
      // Update existing alert
      const updateData = {
        condition: mapCondition(data.condition),
        targetRate: parseFloat(data.targetRate!.toString()),
        isActive: data.isActive,
      };
      await updateAlertAPI(editingAlertId.value, updateData);
    } else {
      // Create new alert
      const createData = {
        baseCurrency: data.baseCurrency,
        targetCurrency: data.targetCurrency,
        condition: mapCondition(data.condition),
        targetRate: parseFloat(data.targetRate!.toString()),
      };
      await createAlertAPI(createData);
    }

    await loadAlerts();
    closeModal();
  } catch (error) {
    console.error('Failed to save alert:', error);
  } finally {
    submitting.value = false;
  }
};

// Toggle alert status
const toggleAlertStatus = async (alert: Alert) => {
  try {
    await updateAlertAPI(alert.id, { isActive: !alert.isActive });
    await loadAlerts();
  } catch (error) {
    console.error('Failed to toggle alert status:', error);
  }
};

// Delete alert
const deleteAlert = async () => {
  if (!alertToDelete.value || deleting.value) return;

  try {
    deleting.value = true;
    await deleteAlertAPI(alertToDelete.value.id);
    await loadAlerts();
    closeDeleteModal();
  } catch (error) {
    console.error('Failed to delete alert:', error);
  } finally {
    deleting.value = false;
  }
};

onMounted(() => {
  loadAlerts();
});
</script>
