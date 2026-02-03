<template>
  <!-- Modal Overlay -->
  <Teleport to="body">
    <Transition name="modal">
      <div 
        v-if="isOpen"
        class="fixed inset-0 z-50 flex items-center justify-center bg-blueprint-text bg-opacity-80"
        @click.self="closeModal"
      >
        <!-- Modal Container -->
        <div class="bg-white border-2 border-blueprint-border w-full max-w-2xl mx-4">
          <!-- Header -->
          <div class="border-b border-blueprint-border px-6 py-4 flex justify-between items-center bg-white">
            <div>
              <h2 class="font-sans text-lg font-bold text-blueprint-text uppercase">
                {{ isEdit ? 'Edit Alert' : 'Set Currency Alert' }}
              </h2>
              <p class="font-sans text-xs text-blueprint-text-secondary mt-1">
                {{ isEdit ? 'Update your monitoring parameters' : 'Configure your forex rate monitoring parameters' }}
              </p>
            </div>
            <button 
              @click="closeModal"
              class="text-blueprint-text hover:text-blueprint-error transition-colors"
              type="button"
            >
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
              </svg>
            </button>
          </div>

          <!-- Form Content -->
          <form @submit.prevent="handleSubmit" class="p-6 bg-white">
            <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
              <!-- Base Currency -->
              <div>
                <label class="block font-sans text-xs font-bold text-blueprint-text uppercase mb-2">
                  Base Currency
                </label>
                <select 
                  v-model="formData.baseCurrency"
                  :disabled="isEdit"
                  class="w-full px-4 py-3 border border-blueprint-border bg-blueprint-surface text-blueprint-text font-mono text-sm focus:outline-none focus:border-2 focus:border-blueprint-primary disabled:opacity-50"
                  required
                >
                  <option value="">Select Base</option>
                  <option v-for="currency in currencies" :key="currency" :value="currency">
                    {{ currency }}
                  </option>
                </select>
              </div>

              <!-- Target Currency -->
              <div>
                <label class="block font-sans text-xs font-bold text-blueprint-text uppercase mb-2">
                  Target Currency
                </label>
                <select 
                  v-model="formData.targetCurrency"
                  :disabled="isEdit"
                  class="w-full px-4 py-3 border border-blueprint-border bg-blueprint-surface text-blueprint-text font-mono text-sm focus:outline-none focus:border-2 focus:border-blueprint-primary disabled:opacity-50"
                  required
                >
                  <option value="">Select Target</option>
                  <option v-for="currency in currencies" :key="currency" :value="currency">
                    {{ currency }}
                  </option>
                </select>
              </div>

              <!-- Condition -->
              <div>
                <label class="block font-sans text-xs font-bold text-blueprint-text uppercase mb-2">
                  Trigger Condition
                </label>
                <select 
                  v-model="formData.condition"
                  class="w-full px-4 py-3 border border-blueprint-border bg-blueprint-surface text-blueprint-text font-sans text-sm focus:outline-none focus:border-2 focus:border-blueprint-primary"
                  required
                >
                  <option value="1">GREATER THAN (>)</option>
                  <option value="2">LESS THAN (<)</option>
                  <option value="3">EQUAL TO (=)</option>
                </select>
              </div>

              <!-- Target Rate -->
              <div>
                <label class="block font-sans text-xs font-bold text-blueprint-text uppercase mb-2">
                  Target Rate
                </label>
                <input 
                  v-model="formData.targetRate"
                  type="text"
                  inputmode="decimal"
                  pattern="[0-9]*\.?[0-9]{0,4}"
                  placeholder="0.0000"
                  class="w-full px-4 py-3 border border-blueprint-border bg-blueprint-surface text-blueprint-text font-mono text-sm focus:outline-none focus:border-2 focus:border-blueprint-primary"
                  required
                />
                <p class="font-sans text-xs text-blueprint-text-secondary mt-1">Enter rate with up to 4 decimal places</p>
              </div>
            </div>

            <!-- Active Status (Edit Mode Only) -->
            <div v-if="isEdit" class="mt-4 flex items-center">
              <input
                v-model="formData.isActive"
                type="checkbox"
                id="isActive"
                class="w-4 h-4 text-blueprint-primary border-blueprint-border focus:ring-blueprint-primary"
              />
              <label for="isActive" class="ml-2 font-sans text-sm font-bold text-blueprint-text uppercase">
                Alert is active
              </label>
            </div>

            <!-- Action Buttons -->
            <div class="mt-6 flex gap-3 justify-end">
              <button 
                type="button"
                @click="closeModal"
                class="px-6 py-3 border border-blueprint-border bg-blueprint-bg text-blueprint-text font-sans text-sm font-bold uppercase hover:bg-blueprint-text hover:text-white transition-colors"
              >
                Cancel
              </button>
              <button 
                type="submit"
                :disabled="isSubmitting"
                class="px-6 py-3 bg-blueprint-primary border border-blueprint-primary text-white font-sans text-sm font-bold uppercase hover:bg-blueprint-text hover:border-blueprint-text transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {{ isSubmitting ? (isEdit ? 'UPDATING...' : 'CREATING...') : (isEdit ? 'UPDATE ALERT' : 'CREATE ALERT') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

export interface AlertFormData {
  baseCurrency: string;
  targetCurrency: string;
  condition: string | number;
  targetRate: string | number | null;
  isActive?: boolean;
}

interface Props {
  isOpen: boolean;
  prefillPair?: string;
  isEdit?: boolean;
  initialData?: AlertFormData | null;
}

const props = withDefaults(defineProps<Props>(), {
  prefillPair: '',
  isEdit: false,
  initialData: null,
});

const emit = defineEmits<{
  'close': []
  'submit': [data: AlertFormData]
}>();

// ISO 4217 Major Currencies
const currencies = [
  'USD', 'EUR', 'GBP', 'JPY', 'CHF', 'CAD', 'AUD', 'NZD',
  'CNY', 'INR', 'BRL', 'MXN', 'ZAR', 'SGD', 'HKD', 'SEK',
  'NOK', 'DKK', 'KRW', 'TRY', 'RUB', 'PLN', 'THB', 'MYR',
  'IDR', 'PHP', 'CZK', 'ILS', 'CLP', 'AED', 'SAR', 'MWK'
];

const formData = ref<AlertFormData>({
  baseCurrency: '',
  targetCurrency: '',
  condition: '1',
  targetRate: null,
  isActive: true,
});

const isSubmitting = ref(false);

// Watch for initialData when opening in edit mode
watch(() => props.isOpen, (isOpen) => {
  if (isOpen && props.isEdit && props.initialData) {
    formData.value = { 
      ...props.initialData,
      targetRate: props.initialData.targetRate?.toString() || null 
    };
  } else if (isOpen && !props.isEdit) {
    // Reset for create mode
    formData.value = {
      baseCurrency: '',
      targetCurrency: '',
      condition: '1',
      targetRate: null,
      isActive: true,
    };
    
    // Apply prefill if available
    if (props.prefillPair && props.prefillPair.includes('/')) {
      const [base, target] = props.prefillPair.split('/');
      formData.value.baseCurrency = base;
      formData.value.targetCurrency = target;
    }
  }
});

// Watch for prefill from currency pair click (for create mode)
watch(() => props.prefillPair, (newPair) => {
  if (newPair && newPair.includes('/') && !props.isEdit) {
    const [base, target] = newPair.split('/');
    formData.value.baseCurrency = base;
    formData.value.targetCurrency = target;
  }
});

const closeModal = () => {
  emit('close');
  // Reset form after animation
  setTimeout(() => {
    formData.value = {
      baseCurrency: '',
      targetCurrency: '',
      condition: '1',
      targetRate: null,
      isActive: true,
    };
    isSubmitting.value = false;
  }, 300);
};

const handleSubmit = () => {
  if (isSubmitting.value) return;
  
  // Validate target rate format
  const rateValue = formData.value.targetRate;
  if (rateValue === null || rateValue === undefined || isNaN(parseFloat(rateValue.toString()))) {
    return;
  }

  isSubmitting.value = true;
  
  emit('submit', {
    ...formData.value,
    targetRate: parseFloat(rateValue.toString()).toFixed(4), // Ensure 4 decimal precision
  });
  // We don't automatically close here in case the parent wants to handle errors
  // But UserDashboardView expects it to close. 
  // AlertsView will also expect it to close or will close it itself.
  // Actually, AlertsView's submitAlert closes it on success.
  // The original AlertForm closed it immediately.
  closeModal();
};
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.2s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
</style>
