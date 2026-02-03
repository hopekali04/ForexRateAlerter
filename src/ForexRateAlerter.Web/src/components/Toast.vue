<template>
  <Teleport to="body">
    <Transition name="toast">
      <div 
        v-if="isVisible"
        class="fixed top-4 right-4 z-50 bg-white border-2 min-w-[320px] max-w-md"
        :class="borderColorClass"
      >
        <div class="p-4 flex items-start justify-between">
          <div class="flex-1">
            <div class="flex items-center mb-1">
              <div 
                class="w-3 h-3 mr-2"
                :class="indicatorClass"
              ></div>
              <span class="font-sans text-xs font-bold text-blueprint-text uppercase">
                {{ type === 'success' ? 'SUCCESS' : type === 'error' ? 'ERROR' : 'INFO' }}
              </span>
            </div>
            <p class="font-sans text-sm text-blueprint-text">{{ message }}</p>
          </div>
          <button 
            @click="close"
            class="text-blueprint-text-secondary hover:text-blueprint-text ml-3"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="square" stroke-linejoin="miter" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';

interface Props {
  message: string;
  type: 'success' | 'error' | 'info';
  duration?: number;
  show: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  duration: 5000,
});

const emit = defineEmits<{
  'close': []
}>();

const isVisible = ref(props.show);
let timeoutId: number | null = null;

const borderColorClass = computed(() => {
  switch (props.type) {
    case 'success':
      return 'border-blueprint-primary';
    case 'error':
      return 'border-blueprint-error';
    default:
      return 'border-blueprint-border';
  }
});

const indicatorClass = computed(() => {
  switch (props.type) {
    case 'success':
      return 'bg-blueprint-primary';
    case 'error':
      return 'bg-blueprint-error';
    default:
      return 'bg-blueprint-text';
  }
});

watch(() => props.show, (newVal) => {
  isVisible.value = newVal;
  
  if (newVal && props.duration > 0) {
    if (timeoutId) {
      clearTimeout(timeoutId);
    }
    timeoutId = window.setTimeout(() => {
      close();
    }, props.duration);
  }
});

const close = () => {
  isVisible.value = false;
  emit('close');
  
  if (timeoutId) {
    clearTimeout(timeoutId);
  }
};
</script>

<style scoped>
.toast-enter-active {
  transition: all 0.2s ease-out;
}

.toast-leave-active {
  transition: all 0.2s ease-in;
}

.toast-enter-from {
  transform: translateX(100%);
  opacity: 0;
}

.toast-leave-to {
  transform: translateX(100%);
  opacity: 0;
}
/* Ensure the background is solid and opaque */
.bg-surface {
  background-color: #FFFFFF !important;
}
</style>
