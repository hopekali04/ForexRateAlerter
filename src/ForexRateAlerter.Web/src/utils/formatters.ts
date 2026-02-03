/**
 * Formats exchange rate numeric values to standard 4-decimal precision string
 */
export const formatRate = (rate: number): string => {
  return rate?.toFixed(4) || '0.0000';
};

/**
 * Maps condition identifiers to their display labels or symbols
 */
export const formatCondition = (condition: string | number, type: 'full' | 'symbol' = 'full'): string => {
  const conditionStr = String(condition);
  
  const fullMap: Record<string, string> = {
    '1': 'Greater Than (>)',
    '2': 'Less Than (<)',
    '3': 'Equal To (=)',
    'GreaterThan': 'Greater Than (>)',
    'LessThan': 'Less Than (<)',
    'EqualTo': 'Equal To (=)',
  };

  const symbolMap: Record<string, string> = {
    '1': '>',
    '2': '<',
    '3': '=',
    'GreaterThan': '>',
    'LessThan': '<',
    'EqualTo': '=',
  };

  if (type === 'symbol') {
    return symbolMap[conditionStr] || '?';
  }
  
  return fullMap[conditionStr] || conditionStr;
};

/**
 * Formats a date string to a human-readable relative or absolute format
 */
export const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMs / 3600000);
  const diffDays = Math.floor(diffMs / 86400000);

  if (diffMins < 1) return 'Just now';
  if (diffMins < 60) return `${diffMins}m ago`;
  if (diffHours < 24) return `${diffHours}h ago`;
  if (diffDays < 7) return `${diffDays}d ago`;
  
  return date.toLocaleDateString();
};
