import { apiClient } from '../lib/api-client.ts';

export const useMenus = () => {
  return apiClient.useQuery('get', '/api/Menu');
};
