<template>
  <div class="min-h-screen bg-[#F8FAFC]">
    <div class="px-6 py-8 mx-auto max-w-7xl lg:px-8">
      <!-- Header Section -->
      <div class="mb-8">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between">
          <div class="text-center md:text-left mb-4 md:mb-0">
            <h1 class="text-3xl font-bold tracking-tight text-[#0F172A] mb-2">
              User Management
            </h1>
            <p class="text-base text-gray-600 max-w-2xl">
              Manage user accounts, roles, and access permissions
            </p>
          </div>
          <button
            @click="openCreateModal"
            class="bg-[#22C55E] text-white px-4 py-3 sm:px-6 font-semibold hover:bg-[#16A34A] active:bg-[#15803D] transition-colors duration-200 border border-solid border-[#22C55E] w-full sm:w-auto touch-manipulation"
          >
            + Create User
          </button>
        </div>
      </div>

      <!-- Statistics Cards -->
      <div class="grid grid-cols-1 gap-4 mb-8 md:grid-cols-3">
        <div class="bg-white border border-solid border-[#E2E8F0] p-6">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-10 h-10 bg-[#22C55E] border border-solid border-[#E2E8F0] mr-4">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Total Users</h2>
              <p class="text-2xl font-mono font-bold text-[#0F172A]">{{ users.length }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white border border-solid border-[#E2E8F0] p-6">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-10 h-10 bg-[#22C55E] border border-solid border-[#E2E8F0] mr-4">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Active Users</h2>
              <p class="text-2xl font-mono font-bold text-[#0F172A]">{{ activeUsersCount }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white border border-solid border-[#E2E8F0] p-6">
          <div class="flex items-center">
            <div class="flex items-center justify-center w-10 h-10 bg-[#22C55E] border border-solid border-[#E2E8F0] mr-4">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
              </svg>
            </div>
            <div class="flex-1">
              <h2 class="text-sm font-semibold text-gray-600 mb-1">Admins</h2>
              <p class="text-2xl font-mono font-bold text-[#0F172A]">{{ adminCount }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Search and Filter -->
      <div class="bg-white border border-solid border-[#E2E8F0] p-6 mb-6">
        <div class="flex flex-col md:flex-row gap-4">
          <div class="flex-1">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by email, name..."
              class="w-full px-4 py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E] font-mono"
            />
          </div>
          <div class="w-full md:w-48">
            <select
              v-model="filterRole"
              class="w-full px-4 py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E]"
            >
              <option value="">All Roles</option>
              <option value="User">User</option>
              <option value="Admin">Admin</option>
            </select>
          </div>
          <div class="w-full md:w-48">
            <select
              v-model="filterStatus"
              class="w-full px-4 py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E]"
            >
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Users Table (Desktop) -->
      <div class="hidden md:block bg-white border border-solid border-[#E2E8F0]">
        <div v-if="loading" class="p-12 text-center">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-solid border-[#E2E8F0] border-t-[#22C55E]"></div>
          <p class="mt-4 text-gray-600">Loading users...</p>
        </div>

        <div v-else-if="error" class="p-12 text-center text-red-600">
          <p>{{ error }}</p>
          <button @click="loadUsers" class="mt-4 text-[#22C55E] hover:underline">Retry</button>
        </div>

        <div v-else-if="filteredUsers.length === 0" class="p-12 text-center text-gray-600">
          <p>No users found</p>
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-[#F8FAFC] border-b border-solid border-[#E2E8F0]">
              <tr>
                <th class="px-6 py-4 text-left text-xs font-bold text-[#0F172A] uppercase tracking-wider">ID</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-[#0F172A] uppercase tracking-wider">User</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-[#0F172A] uppercase tracking-wider">Email</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-[#0F172A] uppercase tracking-wider">Role</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-[#0F172A] uppercase tracking-wider">Status</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-[#0F172A] uppercase tracking-wider">Alerts</th>
                <th class="px-6 py-4 text-left text-xs font-bold text-[#0F172A] uppercase tracking-wider">Created</th>
                <th class="px-6 py-4 text-right text-xs font-bold text-[#0F172A] uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#E2E8F0]">
              <tr v-for="user in filteredUsers" :key="user.id" class="hover:bg-[#F8FAFC] transition-colors duration-150">
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="font-mono text-sm text-[#0F172A]">{{ user.id }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex items-center justify-center w-10 h-10 bg-[#22C55E] border border-solid border-[#E2E8F0] mr-3">
                      <span class="text-white font-bold text-sm">{{ getInitials(user.firstName, user.lastName) }}</span>
                    </div>
                    <div>
                      <div class="text-sm font-semibold text-[#0F172A]">{{ user.firstName }} {{ user.lastName }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="font-mono text-sm text-[#0F172A]">{{ user.email }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span 
                    :class="[
                      'px-3 py-1 text-xs font-semibold border border-solid',
                      user.role === 'Admin' 
                        ? 'bg-[#22C55E] text-white border-[#22C55E]' 
                        : 'bg-white text-[#0F172A] border-[#E2E8F0]'
                    ]"
                  >
                    {{ user.role }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span 
                    :class="[
                      'px-3 py-1 text-xs font-semibold border border-solid',
                      user.isActive 
                        ? 'bg-[#22C55E] text-white border-[#22C55E]' 
                        : 'bg-gray-200 text-gray-700 border-gray-300'
                    ]"
                  >
                    {{ user.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="font-mono text-sm text-[#0F172A]">{{ user.alertCount }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-600">{{ formatDate(user.createdAt) }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right">
                  <div class="flex justify-end gap-2">
                    <button
                      @click="openViewModal(user)"
                      class="px-3 py-1 text-xs font-semibold bg-white text-[#0F172A] border border-solid border-[#E2E8F0] hover:bg-[#F8FAFC] transition-colors duration-150"
                      title="View Details"
                    >
                      View
                    </button>
                    <button
                      @click="openEditModal(user)"
                      class="px-3 py-1 text-xs font-semibold bg-white text-[#0F172A] border border-solid border-[#E2E8F0] hover:bg-[#F8FAFC] transition-colors duration-150"
                      title="Edit User"
                    >
                      Edit
                    </button>
                    <button
                      @click="confirmToggleStatus(user)"
                      :class="[
                        'px-3 py-1 text-xs font-semibold border border-solid transition-colors duration-150',
                        user.isActive 
                          ? 'bg-white text-red-600 border-red-600 hover:bg-red-50' 
                          : 'bg-white text-[#22C55E] border-[#22C55E] hover:bg-green-50'
                      ]"
                      :title="user.isActive ? 'Deactivate' : 'Activate'"
                    >
                      {{ user.isActive ? 'Deactivate' : 'Activate' }}
                    </button>
                    <button
                      @click="confirmDelete(user)"
                      class="px-3 py-1 text-xs font-semibold bg-white text-red-600 border border-solid border-red-600 hover:bg-red-50 transition-colors duration-150"
                      title="Delete User"
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Users Cards (Mobile) -->
      <div class="md:hidden space-y-4">
        <div v-if="loading" class="bg-white border border-solid border-[#E2E8F0] p-12 text-center">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-solid border-[#E2E8F0] border-t-[#22C55E]"></div>
          <p class="mt-4 text-gray-600">Loading users...</p>
        </div>

        <div v-else-if="error" class="bg-white border border-solid border-[#E2E8F0] p-12 text-center text-red-600">
          <p>{{ error }}</p>
          <button @click="loadUsers" class="mt-4 text-[#22C55E] hover:underline">Retry</button>
        </div>

        <div v-else-if="filteredUsers.length === 0" class="bg-white border border-solid border-[#E2E8F0] p-12 text-center text-gray-600">
          <p>No users found</p>
        </div>

        <div v-else v-for="user in filteredUsers" :key="user.id" class="bg-white border border-solid border-[#E2E8F0] p-4">
          <!-- User Header -->
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center">
              <div class="flex items-center justify-center w-12 h-12 bg-[#22C55E] border border-solid border-[#E2E8F0] mr-3">
                <span class="text-white font-bold">{{ getInitials(user.firstName, user.lastName) }}</span>
              </div>
              <div>
                <h3 class="text-sm font-bold text-[#0F172A]">{{ user.firstName }} {{ user.lastName }}</h3>
                <p class="font-mono text-xs text-gray-600">ID: {{ user.id }}</p>
              </div>
            </div>
            <div class="flex gap-1">
              <span 
                :class="[
                  'px-2 py-1 text-xs font-semibold border border-solid',
                  user.role === 'Admin' 
                    ? 'bg-[#22C55E] text-white border-[#22C55E]' 
                    : 'bg-white text-[#0F172A] border-[#E2E8F0]'
                ]"
              >
                {{ user.role }}
              </span>
            </div>
          </div>

          <!-- User Details -->
          <div class="space-y-2 mb-4 pb-4 border-b border-solid border-[#E2E8F0]">
            <div class="flex justify-between items-center">
              <span class="text-xs font-semibold text-gray-600">Email</span>
              <span class="font-mono text-xs text-[#0F172A] truncate ml-2">{{ user.email }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-xs font-semibold text-gray-600">Status</span>
              <span 
                :class="[
                  'px-2 py-1 text-xs font-semibold border border-solid',
                  user.isActive 
                    ? 'bg-[#22C55E] text-white border-[#22C55E]' 
                    : 'bg-gray-200 text-gray-700 border-gray-300'
                ]"
              >
                {{ user.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-xs font-semibold text-gray-600">Alerts</span>
              <span class="font-mono text-xs text-[#0F172A]">{{ user.alertCount }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-xs font-semibold text-gray-600">Created</span>
              <span class="text-xs text-gray-600">{{ formatDate(user.createdAt) }}</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="grid grid-cols-2 gap-2">
            <button
              @click="openViewModal(user)"
              class="px-3 py-2 text-xs font-semibold bg-white text-[#0F172A] border border-solid border-[#E2E8F0] hover:bg-[#F8FAFC] transition-colors duration-150 active:bg-[#E2E8F0]"
            >
              View
            </button>
            <button
              @click="openEditModal(user)"
              class="px-3 py-2 text-xs font-semibold bg-white text-[#0F172A] border border-solid border-[#E2E8F0] hover:bg-[#F8FAFC] transition-colors duration-150 active:bg-[#E2E8F0]"
            >
              Edit
            </button>
            <button
              @click="confirmToggleStatus(user)"
              :class="[
                'px-3 py-2 text-xs font-semibold border border-solid transition-colors duration-150',
                user.isActive 
                  ? 'bg-white text-red-600 border-red-600 hover:bg-red-50 active:bg-red-100' 
                  : 'bg-white text-[#22C55E] border-[#22C55E] hover:bg-green-50 active:bg-green-100'
              ]"
            >
              {{ user.isActive ? 'Deactivate' : 'Activate' }}
            </button>
            <button
              @click="confirmDelete(user)"
              class="px-3 py-2 text-xs font-semibold bg-white text-red-600 border border-solid border-red-600 hover:bg-red-50 transition-colors duration-150 active:bg-red-100"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- View User Modal -->
    <div v-if="showViewModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-2 sm:p-4" @click.self="closeViewModal">
      <div class="bg-white border border-solid border-[#E2E8F0] max-w-2xl w-full max-h-[95vh] sm:max-h-[90vh] overflow-y-auto" @click.stop>
        <div class="sticky top-0 bg-white border-b border-solid border-[#E2E8F0] px-6 py-4 flex justify-between items-center">
          <h2 class="text-xl font-bold text-[#0F172A]">User Details</h2>
          <button @click="closeViewModal" class="text-gray-600 hover:text-[#0F172A] transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>
        <div class="p-6" v-if="selectedUser">
          <div class="space-y-4">
            <div class="flex items-center mb-6">
              <div class="flex items-center justify-center w-16 h-16 bg-[#22C55E] border border-solid border-[#E2E8F0] mr-4">
                <span class="text-white font-bold text-xl">{{ getInitials(selectedUser.firstName, selectedUser.lastName) }}</span>
              </div>
              <div>
                <h3 class="text-xl font-bold text-[#0F172A]">{{ selectedUser.firstName }} {{ selectedUser.lastName }}</h3>
                <p class="font-mono text-gray-600">{{ selectedUser.email }}</p>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="bg-[#F8FAFC] border border-solid border-[#E2E8F0] p-4">
                <p class="text-xs font-semibold text-gray-600 mb-1">User ID</p>
                <p class="font-mono text-[#0F172A]">{{ selectedUser.id }}</p>
              </div>
              <div class="bg-[#F8FAFC] border border-solid border-[#E2E8F0] p-4">
                <p class="text-xs font-semibold text-gray-600 mb-1">Role</p>
                <p class="text-[#0F172A]">{{ selectedUser.role }}</p>
              </div>
              <div class="bg-[#F8FAFC] border border-solid border-[#E2E8F0] p-4">
                <p class="text-xs font-semibold text-gray-600 mb-1">Status</p>
                <span 
                  :class="[
                    'px-3 py-1 text-xs font-semibold border border-solid inline-block',
                    selectedUser.isActive 
                      ? 'bg-[#22C55E] text-white border-[#22C55E]' 
                      : 'bg-gray-200 text-gray-700 border-gray-300'
                  ]"
                >
                  {{ selectedUser.isActive ? 'Active' : 'Inactive' }}
                </span>
              </div>
              <div class="bg-[#F8FAFC] border border-solid border-[#E2E8F0] p-4">
                <p class="text-xs font-semibold text-gray-600 mb-1">Alert Count</p>
                <p class="font-mono text-[#0F172A]">{{ selectedUser.alertCount }}</p>
              </div>
              <div class="bg-[#F8FAFC] border border-solid border-[#E2E8F0] p-4">
                <p class="text-xs font-semibold text-gray-600 mb-1">Created At</p>
                <p class="text-sm text-[#0F172A]">{{ formatFullDate(selectedUser.createdAt) }}</p>
              </div>
              <div class="bg-[#F8FAFC] border border-solid border-[#E2E8F0] p-4">
                <p class="text-xs font-semibold text-gray-600 mb-1">Email</p>
                <p class="font-mono text-sm text-[#0F172A] break-all">{{ selectedUser.email }}</p>
              </div>
            </div>
          </div>
        </div>
        <div class="border-t border-solid border-[#E2E8F0] px-6 py-4 flex justify-end">
          <button
            @click="closeViewModal"
            class="px-6 py-3 bg-[#22C55E] text-white font-semibold hover:bg-[#16A34A] active:bg-[#15803D] transition-colors duration-200 border border-solid border-[#22C55E] touch-manipulation w-full sm:w-auto"
          >
            Close
          </button>
        </div>
      </div>
    </div>

    <!-- Create/Edit User Modal -->
    <div v-if="showFormModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-2 sm:p-4" @click.self="closeFormModal">
      <div class="bg-white border border-solid border-[#E2E8F0] max-w-xl w-full max-h-[95vh] sm:max-h-[90vh] overflow-y-auto" @click.stop>
        <div class="sticky top-0 bg-white border-b border-solid border-[#E2E8F0] px-6 py-4 flex justify-between items-center">
          <h2 class="text-xl font-bold text-[#0F172A]">{{ isEditMode ? 'Edit User' : 'Create User' }}</h2>
          <button @click="closeFormModal" class="text-gray-600 hover:text-[#0F172A] transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>
        <form @submit.prevent="handleSubmit" class="p-6">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-semibold text-[#0F172A] mb-2">Email <span class="text-red-600">*</span></label>
              <input
                v-model="formData.email"
                type="email"
                required
                class="w-full px-3 py-2.5 sm:px-4 sm:py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E] font-mono text-base"
                placeholder="user@example.com"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-semibold text-[#0F172A] mb-2">First Name <span class="text-red-600">*</span></label>
                <input
                  v-model="formData.firstName"
                  type="text"
                  required
                  class="w-full px-3 py-2.5 sm:px-4 sm:py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E] text-base"
                  placeholder="John"
                />
              </div>
              <div>
                <label class="block text-sm font-semibold text-[#0F172A] mb-2">Last Name <span class="text-red-600">*</span></label>
                <input
                  v-model="formData.lastName"
                  type="text"
                  required
                  class="w-full px-3 py-2.5 sm:px-4 sm:py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E] text-base"
                  placeholder="Doe"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-semibold text-[#0F172A] mb-2">
                Password {{ isEditMode ? '(leave blank to keep current)' : '' }} <span v-if="!isEditMode" class="text-red-600">*</span>
              </label>
              <input
                v-model="formData.password"
                type="password"
                :required="!isEditMode"
                :minlength="8"
                class="w-full px-3 py-2.5 sm:px-4 sm:py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E] font-mono text-base"
                placeholder="Min 8 characters"
              />
              <p class="text-xs text-gray-600 mt-1">Minimum 8 characters</p>
            </div>

            <div>
              <label class="block text-sm font-semibold text-[#0F172A] mb-2">Role <span class="text-red-600">*</span></label>
              <select
                v-model="formData.role"
                required
                class="w-full px-3 py-2.5 sm:px-4 sm:py-2 border border-solid border-[#E2E8F0] focus:outline-none focus:border-[#22C55E] text-base"
              >
                <option value="User">User</option>
                <option value="Admin">Admin</option>
              </select>
            </div>

            <div class="flex items-center">
              <input
                v-model="formData.isActive"
                type="checkbox"
                id="isActive"
                class="w-4 h-4 text-[#22C55E] border-[#E2E8F0] focus:ring-[#22C55E]"
              />
              <label for="isActive" class="ml-2 text-sm text-[#0F172A]">Active User</label>
            </div>

            <div v-if="formError" class="bg-red-50 border border-solid border-red-300 p-4">
              <p class="text-sm text-red-600">{{ formError }}</p>
            </div>
          </div>

          <div class="mt-6 flex flex-col sm:flex-row justify-end gap-3 sm:gap-4">
            <button
              type="button"
              @click="closeFormModal"
              class="px-6 py-3 bg-white text-[#0F172A] border border-solid border-[#E2E8F0] font-semibold hover:bg-[#F8FAFC] active:bg-[#E2E8F0] transition-colors duration-200 touch-manipulation order-2 sm:order-1"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="formSubmitting"
              class="px-6 py-3 bg-[#22C55E] text-white font-semibold hover:bg-[#16A34A] active:bg-[#15803D] transition-colors duration-200 border border-solid border-[#22C55E] disabled:opacity-50 disabled:cursor-not-allowed touch-manipulation order-1 sm:order-2"
            >
              {{ formSubmitting ? 'Saving...' : (isEditMode ? 'Update User' : 'Create User') }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Confirmation Modal -->
    <div v-if="showConfirmModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-2 sm:p-4" @click.self="closeConfirmModal">
      <div class="bg-white border border-solid border-[#E2E8F0] max-w-md w-full" @click.stop>
        <div class="border-b border-solid border-[#E2E8F0] px-6 py-4">
          <h2 class="text-xl font-bold text-[#0F172A]">Confirm Action</h2>
        </div>
        <div class="p-6">
          <p class="text-[#0F172A]">{{ confirmMessage }}</p>
        </div>
        <div class="border-t border-solid border-[#E2E8F0] px-6 py-4 flex flex-col sm:flex-row justify-end gap-3 sm:gap-4">
          <button
            @click="closeConfirmModal"
            class="px-6 py-3 bg-white text-[#0F172A] border border-solid border-[#E2E8F0] font-semibold hover:bg-[#F8FAFC] active:bg-[#E2E8F0] transition-colors duration-200 touch-manipulation order-2 sm:order-1"
          >
            Cancel
          </button>
          <button
            @click="executeConfirmAction"
            :disabled="confirmSubmitting"
            :class="[
              'px-6 py-3 font-semibold transition-colors duration-200 border border-solid disabled:opacity-50 disabled:cursor-not-allowed touch-manipulation order-1 sm:order-2',
              confirmAction === 'delete' 
                ? 'bg-red-600 text-white border-red-600 hover:bg-red-700 active:bg-red-800' 
                : 'bg-[#22C55E] text-white border-[#22C55E] hover:bg-[#16A34A] active:bg-[#15803D]'
            ]"
          >
            {{ confirmSubmitting ? 'Processing...' : 'Confirm' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Toast Notifications -->
    <div class="fixed top-2 right-2 sm:top-4 sm:right-4 z-50 space-y-2 max-w-[calc(100vw-1rem)] sm:max-w-md">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        :class="[
          'px-4 py-3 sm:px-6 sm:py-4 border border-solid min-w-0 sm:min-w-[300px] shadow-lg transition-all duration-300',
          'text-sm sm:text-base',
          toast.type === 'success' ? 'bg-white border-[#22C55E] text-[#0F172A]' : 'bg-white border-red-600 text-red-600'
        ]"
      >
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <svg v-if="toast.type === 'success'" class="w-5 h-5 text-[#22C55E] mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
            <svg v-else class="w-5 h-5 text-red-600 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
            <span class="font-semibold">{{ toast.message }}</span>
          </div>
          <button @click="removeToast(toast.id)" class="ml-4 hover:opacity-70">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { getAllUsers, createUser, updateUser, deleteUser, toggleUserStatus } from '@/services/adminService';

interface ErrorResponse {
  response?: {
    data?: {
      message?: string;
    };
  };
}

interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  alertCount: number;
}

interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error';
}

const users = ref<User[]>([]);
const loading = ref(true);
const error = ref('');
const searchQuery = ref('');
const filterRole = ref('');
const filterStatus = ref('');

// Modal states
const showViewModal = ref(false);
const showFormModal = ref(false);
const showConfirmModal = ref(false);
const selectedUser = ref<User | null>(null);
const isEditMode = ref(false);

// Form data
const formData = ref({
  email: '',
  password: '',
  firstName: '',
  lastName: '',
  role: 'User',
  isActive: true
});
const formError = ref('');
const formSubmitting = ref(false);

// Confirmation
const confirmMessage = ref('');
const confirmAction = ref('');
const confirmUserId = ref<number | null>(null);
const confirmSubmitting = ref(false);

// Toasts
const toasts = ref<Toast[]>([]);
let toastIdCounter = 0;

// Computed
const activeUsersCount = computed(() => users.value.filter(u => u.isActive).length);
const adminCount = computed(() => users.value.filter(u => u.role === 'Admin').length);

const filteredUsers = computed(() => {
  let filtered = users.value;

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    filtered = filtered.filter(u =>
      u.email.toLowerCase().includes(query) ||
      u.firstName.toLowerCase().includes(query) ||
      u.lastName.toLowerCase().includes(query)
    );
  }

  if (filterRole.value) {
    filtered = filtered.filter(u => u.role === filterRole.value);
  }

  if (filterStatus.value) {
    filtered = filtered.filter(u =>
      filterStatus.value === 'active' ? u.isActive : !u.isActive
    );
  }

  return filtered;
});

// Methods
const loadUsers = async () => {
  try {
    loading.value = true;
    error.value = '';
    const response = await getAllUsers();
    users.value = response.users;
  } catch (err: unknown) {
    const errorResponse = err as ErrorResponse;
    error.value = errorResponse.response?.data?.message || 'Failed to load users';
    showToast('Failed to load users', 'error');
  } finally {
    loading.value = false;
  }
};

const getInitials = (firstName: string, lastName: string) => {
  return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffDays = Math.floor(diffMs / 86400000);

  if (diffDays < 1) return 'Today';
  if (diffDays < 2) return 'Yesterday';
  if (diffDays < 7) return `${diffDays} days ago`;
  
  return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
};

const formatFullDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  });
};

// View Modal
const openViewModal = (user: User) => {
  selectedUser.value = user;
  showViewModal.value = true;
};

const closeViewModal = () => {
  showViewModal.value = false;
  selectedUser.value = null;
};

// Form Modal
const openCreateModal = () => {
  isEditMode.value = false;
  formData.value = {
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    role: 'User',
    isActive: true
  };
  formError.value = '';
  showFormModal.value = true;
};

const openEditModal = (user: User) => {
  isEditMode.value = true;
  selectedUser.value = user;
  formData.value = {
    email: user.email,
    password: '',
    firstName: user.firstName,
    lastName: user.lastName,
    role: user.role,
    isActive: user.isActive
  };
  formError.value = '';
  showFormModal.value = true;
};

const closeFormModal = () => {
  showFormModal.value = false;
  selectedUser.value = null;
  formError.value = '';
};

const handleSubmit = async () => {
  try {
    formSubmitting.value = true;
    formError.value = '';

    if (isEditMode.value && selectedUser.value) {
      await updateUser(selectedUser.value.id, formData.value);
      showToast('User updated successfully', 'success');
    } else {
      await createUser(formData.value);
      showToast('User created successfully', 'success');
    }

    closeFormModal();
    await loadUsers();
  } catch (err: unknown) {
    const errorResponse = err as ErrorResponse;
    formError.value = errorResponse.response?.data?.message || 'Failed to save user';
  } finally {
    formSubmitting.value = false;
  }
};

// Confirmation Modal
const confirmToggleStatus = (user: User) => {
  selectedUser.value = user;
  confirmUserId.value = user.id;
  confirmAction.value = 'toggleStatus';
  confirmMessage.value = `Are you sure you want to ${user.isActive ? 'deactivate' : 'activate'} ${user.firstName} ${user.lastName}?`;
  showConfirmModal.value = true;
};

const confirmDelete = (user: User) => {
  selectedUser.value = user;
  confirmUserId.value = user.id;
  confirmAction.value = 'delete';
  confirmMessage.value = `Are you sure you want to delete ${user.firstName} ${user.lastName}? This action cannot be undone.`;
  showConfirmModal.value = true;
};

const closeConfirmModal = () => {
  showConfirmModal.value = false;
  confirmUserId.value = null;
  confirmAction.value = '';
  confirmMessage.value = '';
  selectedUser.value = null;
};

const executeConfirmAction = async () => {
  if (!confirmUserId.value) return;

  try {
    confirmSubmitting.value = true;

    if (confirmAction.value === 'delete') {
      await deleteUser(confirmUserId.value);
      showToast('User deleted successfully', 'success');
    } else if (confirmAction.value === 'toggleStatus') {
      await toggleUserStatus(confirmUserId.value);
      showToast('User status updated successfully', 'success');
    }

    closeConfirmModal();
    await loadUsers();
  } catch (err: unknown) {
    const errorResponse = err as ErrorResponse;
    showToast(errorResponse.response?.data?.message || 'Action failed', 'error');
  } finally {
    confirmSubmitting.value = false;
  }
};

// Toast
const showToast = (message: string, type: 'success' | 'error' = 'success') => {
  const id = toastIdCounter++;
  toasts.value.push({ id, message, type });
  setTimeout(() => removeToast(id), 5000);
};

const removeToast = (id: number) => {
  const index = toasts.value.findIndex(t => t.id === id);
  if (index > -1) {
    toasts.value.splice(index, 1);
  }
};

onMounted(() => {
  loadUsers();
});
</script>
