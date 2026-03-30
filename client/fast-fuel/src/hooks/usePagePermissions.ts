import { useMemo } from 'react';
import { useSuspenseQuery } from '@tanstack/react-query';
import { myPermissionsQueryOptions, pagesQueryOptions } from '../lib/api-client.ts';
import type { Page, Permission } from '../lib/page-definitions.ts';
import { buildPermissionMap, type PermissionMap } from '../lib/buildPermissionMap.ts';

type AllTrue<M> = { [R in keyof M]: { [A in keyof M[R]]: true } };

export type SplitPagePermissions =
  | {
      hasNecessary: false;
      necessary: PermissionMap<Permission>;
      recommended: PermissionMap<Permission>;
    }
  | {
      hasNecessary: true;
      necessary: AllTrue<PermissionMap<Permission>>;
      recommended: PermissionMap<Permission>;
    };

const EMPTY_PAGE_PERMISSIONS = {
  necessaryPermissions: [] as Permission[],
  recommendedPermissions: [] as Permission[],
};

export function usePagePermissions(page: Page): SplitPagePermissions {
  const { data: permissions } = useSuspenseQuery(myPermissionsQueryOptions());
  const { data: pages } = useSuspenseQuery(pagesQueryOptions());

  const pagePermissions = useMemo(() => {
    const pageMap = Object.fromEntries(pages.map((entry) => [entry.page, entry])) as Partial<
      Record<
        Page,
        {
          necessaryPermissions: Permission[];
          recommendedPermissions: Permission[];
        }
      >
    >;

    return pageMap[page] ?? EMPTY_PAGE_PERMISSIONS;
  }, [page, pages]);

  const allPermissionsMap = buildPermissionMap(permissions) as PermissionMap<Permission>;

  const hasNecessary = pagePermissions.necessaryPermissions.every((permission) => {
    const [, resource, action] = permission.split(':');
    return (
      (allPermissionsMap as Record<string, Record<string, boolean>>)[resource]?.[action] ?? false
    );
  });

  if (!hasNecessary) {
    return {
      hasNecessary: false,
      necessary: allPermissionsMap,
      recommended: allPermissionsMap,
    };
  }

  return {
    hasNecessary: true,
    necessary: allPermissionsMap as AllTrue<PermissionMap<Permission>>,
    recommended: allPermissionsMap,
  };
}
