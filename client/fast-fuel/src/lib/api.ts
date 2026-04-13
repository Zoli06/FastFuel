import type { components, paths } from '../types/api-schema.generated.ts';
import createFetchClient, { type Middleware } from 'openapi-fetch';
import createClient from 'openapi-react-query';
import { router } from './router.tsx';
import { notifications } from '@mantine/notifications';
import type {
  UseApi,
  UseApiWithoutSpecifiedPage,
  HooksForPerm,
  HooksWithNoPerm,
  PageName,
  PageNecessaryPermissions,
  PageRecommendedPermissions,
  PermissionName,
} from '../types/page-permissions.generated.ts';
import { QueryClient } from '@tanstack/react-query';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5249';

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
      invalidateApiCache();
    }
    if (response.url.includes('/api/User/me') || response.url.includes('/api/Auth/login')) {
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

export const queryClient = new QueryClient();

export const invalidateApiCache = () => queryClient.invalidateQueries();

const fetchClient = createFetchClient<paths>({
  baseUrl: API_BASE_URL,
  credentials: 'include',
});
fetchClient.use(authenticationMiddleware, errorResponseMiddleware);

const apiClient = createClient<paths>(fetchClient);

export function useApi<P extends PageName>(page: P): UseApi<P>;
export function useApi(page: null): UseApiWithoutSpecifiedPage;
export function useApi() {
  const useNoPerm = (): HooksWithNoPerm => apiClient as unknown as HooksWithNoPerm;

  const useHasPerm = (perm: PermissionName): boolean =>
    useNoPerm().useSuspenseQuery('get', '/api/Permission/my').data.includes(perm);

  const useNecessaryPerm = <Perm extends PageNecessaryPermissions[NonNullable<PageName>]>(
    perm: Perm,
  ): HooksForPerm<Perm> => {
    if (!useHasPerm(perm)) {
      throw new Error('User does not have necessary permission: ' + perm);
    }
    return apiClient as unknown as HooksForPerm<Perm>;
  };

  const useRecommendedPerm = <Perm extends PageRecommendedPermissions[NonNullable<PageName>]>(
    perm: Perm,
  ): HooksForPerm<Perm> | null => {
    if (!useHasPerm(perm)) {
      return null;
    }
    return apiClient as unknown as HooksForPerm<Perm>;
  };

  return { useNoPerm, useNecessaryPerm, useRecommendedPerm, invalidateApiCache };
}
