import type { components, paths } from '../types/api';
import createFetchClient, { type Middleware } from 'openapi-fetch';
import createClient from 'openapi-react-query';
import { router } from './router.tsx';
import { notifications } from '@mantine/notifications';
import { queryClient } from './query-client.ts';

export const triggerPermissionsRefresh = () => {
  const permissionsQueryKey = myPermissionsQueryOptions().queryKey;
  const rolePagesQueryKey = myRolePagesQueryOptions().queryKey;
  void queryClient.invalidateQueries({ queryKey: permissionsQueryKey });
  void queryClient.invalidateQueries({ queryKey: rolePagesQueryKey });
  void queryClient.refetchQueries({ queryKey: permissionsQueryKey, type: 'all' });
  void queryClient.refetchQueries({ queryKey: rolePagesQueryKey, type: 'all' });
};

export const clearAuthData = () => {
  // Remove auth-scoped data without triggering new /my requests during logout.
  const permissionsQueryKey = myPermissionsQueryOptions().queryKey;
  const rolePagesQueryKey = myRolePagesQueryOptions().queryKey;
  queryClient.removeQueries({ queryKey: permissionsQueryKey });
  queryClient.removeQueries({ queryKey: rolePagesQueryKey });
};

const API_BASE_URL = 'http://localhost:5249';

const authenticationMiddleware: Middleware = {
  async onResponse({ response }) {
    if (response.status === 401) {
      await router.navigate('/login');
    }
    return response;
  },
};

const errorResponseMiddleware: Middleware = {
  async onResponse({ response }) {
    if (response.ok) {
      return response;
    }

    if (response.status === 403 && !response.url.includes('/api/Permission/my')) {
      triggerPermissionsRefresh();
    }

    const error = (await response.json().catch(() => ({
      title: 'An unknown error occurred',
    }))) as components['schemas']['ProblemDetails'];

    notifications.show({
      title: 'Error',
      message: error.title,
      color: 'red',
    });

    return response;
  },
};

const fetchClient = createFetchClient<paths>({
  // TODO: move this to .env
  baseUrl: API_BASE_URL,
  credentials: 'include',
});
fetchClient.use(authenticationMiddleware, errorResponseMiddleware);

export const apiClient = createClient(fetchClient);

export const myPermissionsQueryOptions = () =>
  apiClient.queryOptions('get', '/api/Permission/my', undefined, {
    staleTime: 5 * 60 * 1000,
    gcTime: 30 * 60 * 1000,
  });

export const myRolePagesQueryOptions = () =>
  apiClient.queryOptions('get', '/api/Role/my', undefined, {
    staleTime: 5 * 60 * 1000,
    gcTime: 30 * 60 * 1000,
    select: (roles) => [...new Set(roles.flatMap((r) => r.pages))],
  });
