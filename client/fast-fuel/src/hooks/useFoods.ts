import { apiClient } from '../lib/api-client.ts';

export const useFoods = () => {
  return apiClient.useQuery('get', '/api/Food');
};
