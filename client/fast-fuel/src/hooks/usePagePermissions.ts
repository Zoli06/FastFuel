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

type SplitPagePermissions<P extends Page> = {
  necessary: PermissionMap<NecessaryPagePermissions<P>>;
  recommended: PermissionMap<RecommendedPagePermissions<P>>;
  all: PermissionMap<PagePermissions<P>>;
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
  void page;
  const { data: permissions } = useSuspenseQuery(myPermissionsQueryOptions());
  const allPermissionsMap = buildPermissionMap(permissions) as PermissionMap<PagePermissions<P>>;

  if (options?.split) {
    return {
      necessary: allPermissionsMap as PermissionMap<NecessaryPagePermissions<P>>,
      recommended: allPermissionsMap as PermissionMap<RecommendedPagePermissions<P>>,
      all: allPermissionsMap,
    };
  }

  // This narrows compile-time access to only permissions declared for the page.
  return allPermissionsMap;
}
