import type { components, paths } from '../types/api';
import createFetchClient, { type Middleware } from 'openapi-fetch';
import createClient from 'openapi-react-query';
import { router } from './router.tsx';
import { notifications } from '@mantine/notifications';
import { queryClient } from './query-client.ts';

export const triggerPermissionsRefresh = () => {
  const currentUserQueryKey = myCurrentUserQueryOptions().queryKey;
  const permissionsQueryKey = myPermissionsQueryOptions().queryKey;
  const rolesQueryKey = myRolesQueryOptions().queryKey;
  void queryClient.invalidateQueries({ queryKey: currentUserQueryKey });
  void queryClient.invalidateQueries({ queryKey: permissionsQueryKey });
  void queryClient.invalidateQueries({ queryKey: rolesQueryKey });
  void queryClient.refetchQueries({ queryKey: currentUserQueryKey, type: 'all' });
  void queryClient.refetchQueries({ queryKey: permissionsQueryKey, type: 'all' });
  void queryClient.refetchQueries({ queryKey: rolesQueryKey, type: 'all' });
};

export const clearAuthData = () => {
  // Remove auth-scoped data without triggering new /my requests during logout.
  const currentUserQueryKey = myCurrentUserQueryOptions().queryKey;
  const permissionsQueryKey = myPermissionsQueryOptions().queryKey;
  const rolesQueryKey = myRolesQueryOptions().queryKey;
  queryClient.removeQueries({ queryKey: currentUserQueryKey });
  queryClient.removeQueries({ queryKey: permissionsQueryKey });
  queryClient.removeQueries({ queryKey: rolesQueryKey });
};

const API_BASE_URL = 'http://localhost:5249';

const authenticationMiddleware: Middleware = {
  async onResponse({ response }) {
    const skipPaths = ['/api/User/me', '/api/Auth/login', '/api/Customer'];
    const shouldSkip = skipPaths.some((path) => response.url.includes(path));

    const isOnRegister = router.state.location.pathname.startsWith('/register');

    if (response.status === 401 && !shouldSkip && !isOnRegister) {
      await router.navigate('/login');
    }
    return response;
  },
};

const errorResponseMiddleware: Middleware = {
  async onResponse({ response }) {
    if (response.ok || response.status === 401) {
      return response;
    }
    if (response.status === 403 && !response.url.includes('/api/Permission/my')) {
      triggerPermissionsRefresh();
    }
    if (
      response.url.includes('/api/User/me') ||
      response.url.includes('/api/Auth/login') ||
      response.url.includes('/api/Role/my')
    ) {
      return response;
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

export const fetchClient = createFetchClient<paths>({
  baseUrl: API_BASE_URL,
  credentials: 'include',
});
fetchClient.use(authenticationMiddleware, errorResponseMiddleware);

export const apiClient = createClient(fetchClient);

export const myCurrentUserQueryOptions = () =>
  apiClient.queryOptions('get', '/api/User/me', undefined, {
    staleTime: 5 * 60 * 1000,
    gcTime: 30 * 60 * 1000,
  });

export const myPermissionsQueryOptions = () =>
  apiClient.queryOptions('get', '/api/Permission/my', undefined, {
    staleTime: 5 * 60 * 1000,
    gcTime: 30 * 60 * 1000,
  });

export const myRolesQueryOptions = () =>
  apiClient.queryOptions('get', '/api/Role/my', undefined, {
    staleTime: 5 * 60 * 1000,
    gcTime: 30 * 60 * 1000,
  });

export const pagesQueryOptions = () =>
  apiClient.queryOptions('get', '/api/Page', undefined, {
    staleTime: 30 * 60 * 1000,
    gcTime: 60 * 60 * 1000,
  });
