import type { paths } from './types/api';
import createFetchClient from 'openapi-fetch';
import createClient from 'openapi-react-query';

const fetchClient = createFetchClient<paths>({
  // TODO: move this to .env
  baseUrl: 'http://localhost:5249',
});
export const apiClient = createClient(fetchClient);
