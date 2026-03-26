import { useSuspenseQuery } from '@tanstack/react-query';
import { myPermissionsQueryOptions } from '../lib/api-client.ts';
import { pageDefinitions } from '../lib/page-permissions.ts';
import { buildPermissionMap, type PermissionMap } from '../lib/buildPermissionMap.ts';

type Page = keyof typeof pageDefinitions;

type NecessaryPagePermissions<P extends Page> =
  (typeof pageDefinitions)[P]['necessaryPermissions'][number];

type RecommendedPagePermissions<P extends Page> =
  (typeof pageDefinitions)[P]['recommendedPermissions'][number];

type PagePermissions<P extends Page> =
  | (typeof pageDefinitions)[P]['necessaryPermissions'][number]
  | (typeof pageDefinitions)[P]['recommendedPermissions'][number];

type AllTrue<M> = { [R in keyof M]: { [A in keyof M[R]]: true } };

export type SplitPagePermissions<P extends Page> =
  | {
      hasNecessary: false;
      necessary: PermissionMap<NecessaryPagePermissions<P>>;
      recommended: PermissionMap<RecommendedPagePermissions<P>>;
    }
  | {
      hasNecessary: true;
      necessary: AllTrue<PermissionMap<NecessaryPagePermissions<P>>>;
      recommended: PermissionMap<RecommendedPagePermissions<P>>;
    };

type UsePagePermissionsOptions = {
  split?: boolean;
};

export function usePagePermissions<P extends Page>(
  page: P,
  options: { split: true },
): SplitPagePermissions<P>;
export function usePagePermissions<P extends Page>(
  page: P,
  options?: UsePagePermissionsOptions,
): PermissionMap<PagePermissions<P>>;
export function usePagePermissions<P extends Page>(
  page: P,
  options?: UsePagePermissionsOptions,
): PermissionMap<PagePermissions<P>> | SplitPagePermissions<P> {
  const { data: permissions } = useSuspenseQuery(myPermissionsQueryOptions());
  const allPermissionsMap = buildPermissionMap(permissions) as PermissionMap<PagePermissions<P>>;

  if (options?.split) {
    const hasNecessary = pageDefinitions[page].necessaryPermissions.every((p) => {
      const [, resource, action] = (p as string).split(':');
      return (
        (allPermissionsMap as Record<string, Record<string, boolean>>)[resource]?.[action] ?? false
      );
    });

    if (!hasNecessary) {
      return {
        hasNecessary: false,
        necessary: allPermissionsMap as PermissionMap<NecessaryPagePermissions<P>>,
        recommended: allPermissionsMap as PermissionMap<RecommendedPagePermissions<P>>,
      };
    }

    return {
      hasNecessary: true,
      necessary: allPermissionsMap as AllTrue<PermissionMap<NecessaryPagePermissions<P>>>,
      recommended: allPermissionsMap as PermissionMap<RecommendedPagePermissions<P>>,
    };
  }

  // This narrows compile-time access to only permissions declared for the page.
  return allPermissionsMap;
}

export function assertNecessaryPermissions<P extends Page>(
  perms: SplitPagePermissions<P>,
): asserts perms is Extract<SplitPagePermissions<P>, { hasNecessary: true }> {
  if (!perms.hasNecessary) {
    throw new Error('Rendered without necessary permissions');
  }
}
