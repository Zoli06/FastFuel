import type { components, paths } from '../types/api';
import createFetchClient, { type Middleware } from 'openapi-fetch';
import createClient from 'openapi-react-query';
import { router } from './router.tsx';
import { notifications } from '@mantine/notifications';

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
  baseUrl: 'http://localhost:5249',
  credentials: 'include',
});
fetchClient.use(authenticationMiddleware, errorResponseMiddleware);

export const apiClient = createClient(fetchClient);
