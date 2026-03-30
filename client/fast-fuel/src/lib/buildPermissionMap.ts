type ExtractResource<P extends string> = P extends `Permission:${infer R}:${string}` ? R : never;
type ExtractAction<P extends string, R extends string> = P extends `Permission:${R}:${infer A}`
  ? A
  : never;

export type PermissionMap<P extends string> = {
  [R in ExtractResource<P>]: {
    [A in ExtractAction<P, R>]: boolean;
  };
};

export const buildPermissionMap = <P extends string>(permissions: P[]): PermissionMap<P> => {
  const map: Record<string, Record<string, boolean>> = new Proxy(
    {} as Record<string, Record<string, boolean>>,
    {
      get: (target, resource: string) =>
        (target[resource] ??= new Proxy({} as Record<string, boolean>, {
          get: (t, action: string) => t[action] ?? false,
        })),
    },
  );

  for (const permission of permissions) {
    const [, resource, action] = permission.split(':');
    map[resource][action] = true;
  }

  return map as PermissionMap<P>;
};
