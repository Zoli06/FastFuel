import { apiClient } from '../lib/api-client.ts';

export const useRestaurants = () => {
  return apiClient.useQuery('get', '/api/Restaurant');
};
