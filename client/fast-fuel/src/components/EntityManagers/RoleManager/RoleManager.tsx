import {
  Badge,
  Button,
  Checkbox,
  Group,
  MultiSelect,
  Stack,
  Text,
  TextInput,
  ThemeIcon,
} from '@mantine/core';
import { $api } from '../../../lib/api.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';
import { pageDefinitions, type Page, type Permission } from '../../../lib/page-definitions.ts';
import type { components } from '../../../types/api-schema.generated.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import { NumericMultiSelect } from '../../common/NumericCombobox/NumericMultiSelect.tsx';

export const RoleManager = () => {
  const { noPerm, necessaryPerm, recommendedPerm } = $api('RoleManager');
  type PageKey = keyof typeof pageDefinitions;
  const { data: pagePermissions = [] } = noPerm().useSuspenseQuery('get', '/api/Page');
  const userReadApi = recommendedPerm('Permission:User:Read');
  const permissionReadApi = recommendedPerm('Permission:Permission:Read');

  const pagePermissionsByPage = Object.fromEntries(
    pagePermissions.map((entry) => [entry.page, entry]),
  ) as Partial<
    Record<
      Page,
      {
        necessaryPermissions: Permission[];
        recommendedPermissions: Permission[];
        requiresDefaultRole: components['schemas']['DefaultRole'][];
      }
    >
  >;

  const getPagePermissions = (page: Page) => {
    const entry = pagePermissionsByPage[page];
    return {
      necessaryPermissions: entry?.necessaryPermissions ?? [],
      recommendedPermissions: entry?.recommendedPermissions ?? [],
      requiresDefaultRole: entry?.requiresDefaultRole ?? [],
    };
  };

  const [
    { data: users = [] },
    { data: permissions = [] },
    { data: roles = [], refetch: refetchRoles },
  ] = useConditionalSuspenseQueries([
    userReadApi?.queryOptions('get', '/api/User'),
    permissionReadApi?.queryOptions('get', '/api/Permission'),
    necessaryPerm('Permission:Role:Read').queryOptions('get', '/api/Role'),
  ]);

  type Role = (typeof roles)[number];
  type RoleFormValues = Pick<Role, 'id' | 'name' | 'permissions' | 'pages' | 'userIds'>;

  const pageEntries = (
    Object.entries(pageDefinitions) as [PageKey, (typeof pageDefinitions)[PageKey]][]
  ).sort(([, left], [, right]) => left.displayName.localeCompare(right.displayName));

  const getMissingNecessaryPermissions = (pages: Page[], selectedPermissions: Permission[]) => {
    const permissionSet = new Set(selectedPermissions);
    const missing = pages.flatMap(
      (page) =>
        getPagePermissions(page).necessaryPermissions.filter(
          (permission) => !permissionSet.has(permission),
        ) ?? [],
    );

    return [...new Set(missing)];
  };

  const getPagesPermissionError = (pages: Page[], selectedPermissions: Permission[]) => {
    const missing = getMissingNecessaryPermissions(pages, selectedPermissions);
    if (missing.length === 0) return undefined;
    return `Cannot remove permissions required by displayed pages: ${missing.join(', ')}`;
  };

  const getPagesDefaultRoleError = (pages: Page[], roleName: string, isDefaultRole: boolean) => {
    const restrictedPages = pages.filter((page) => {
      const requiredDefaultRoles = getPagePermissions(page).requiresDefaultRole;
      if (!requiredDefaultRoles || requiredDefaultRoles.length === 0) return false;
      if (!isDefaultRole) return true;
      return !requiredDefaultRoles.includes(roleName as components['schemas']['DefaultRole']);
    });

    if (restrictedPages.length === 0) return undefined;

    const pageDescriptions = restrictedPages.map((page) => {
      const definition = pageDefinitions[page as PageKey];
      const roles = getPagePermissions(page).requiresDefaultRole.join(', ') || '-';
      return `${definition.displayName} (default roles: ${roles})`;
    });

    return `These pages can only be assigned to matching default roles: ${pageDescriptions.join('; ')}`;
  };

  const userOptions = users.map((user) => ({
    value: user.id,
    label: `${user.name} (${user.userType})`,
  }));
  const permissionOptions = permissions.map((permission: Permission) => ({
    value: permission,
    label: permission,
  }));

  const tableColumns: ColumnDefinition<Role>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Is Default', render: (r) => (r.isDefault ? 'Yes' : 'No') },
    {
      header: 'Permissions Immutable',
      render: (r) => (r.arePermissionsImmutable ? 'Yes' : 'No'),
    },
  ];

  const isEditingDefaultRole = (roleId: number | undefined, mode: 'create' | 'edit') => {
    if (mode !== 'edit' || roleId == null) return false;
    return !!roles.find((r) => r.id === roleId)?.isDefault;
  };

  const editorFields: Field[] = [
    {
      type: 'custom',
      render: (form, mode) => {
        const disableName = isEditingDefaultRole(form.getValues().id as number | undefined, mode);

        return (
          <TextInput
            key={form.key('name')}
            label="Name"
            required
            disabled={disableName}
            {...form.getInputProps('name')}
          />
        );
      },
    },
    ...(permissionReadApi
      ? [
          {
            type: 'custom',
            render: (form, mode) => {
              const roleId = form.getValues().id as number | undefined;
              const role =
                mode === 'edit' && roleId != null
                  ? roles.find((candidate) => candidate.id === roleId)
                  : undefined;
              const isPermissionsImmutable = !!role?.arePermissionsImmutable;
              const selectedPermissions =
                (form.getValues().permissions as Permission[] | undefined) ?? [];

              return (
                <MultiSelect
                  key={form.key('permissions')}
                  label="Permissions"
                  data={permissionOptions}
                  placeholder="Search permissions..."
                  searchable
                  clearable
                  disabled={isPermissionsImmutable}
                  value={selectedPermissions}
                  error={form.errors.permissions}
                  onChange={(value) => {
                    if (isPermissionsImmutable) return;
                    const nextPermissions = value as Permission[];
                    form.setFieldValue('permissions', nextPermissions);

                    const selectedPages = (form.getValues().pages as Page[] | undefined) ?? [];
                    const pagesPermissionError = getPagesPermissionError(
                      selectedPages,
                      nextPermissions,
                    );
                    if (pagesPermissionError) {
                      form.setFieldError('permissions', pagesPermissionError);
                    } else {
                      form.clearFieldError('permissions');
                    }
                  }}
                />
              );
            },
          } satisfies Field,
        ]
      : []),
    ...(userReadApi
      ? [
          {
            type: 'custom',
            render: (form, mode) => {
              const disableUsers = isEditingDefaultRole(
                form.getValues().id as number | undefined,
                mode,
              );

              return (
                <NumericMultiSelect
                  key={form.key('userIds')}
                  label="Users"
                  data={userOptions}
                  placeholder="Search users..."
                  searchable
                  clearable
                  disabled={disableUsers}
                  {...form.getInputProps('userIds')}
                />
              );
            },
          } satisfies Field,
        ]
      : []),
    {
      type: 'custom',
      render: (form, mode) => {
        const values = form.getValues() as RoleFormValues;
        const role =
          mode === 'edit' && values.id != null
            ? roles.find((candidate) => candidate.id === values.id)
            : undefined;
        const roleName = (values.name ?? role?.name ?? '').trim();
        const isDefaultRole = !!role?.isDefault;
        const isPermissionsImmutable = !!role?.arePermissionsImmutable;
        const selectedPermissions =
          (form.getValues().permissions as Permission[] | undefined) ?? [];
        const selectedPages = (form.getValues().pages as Page[] | undefined) ?? [];
        const selectedPermissionSet = new Set(selectedPermissions);

        const setSelectionErrors = (nextPages: Page[], nextPermissions: Permission[]) => {
          const pagesPermissionError = getPagesPermissionError(nextPages, nextPermissions);
          if (pagesPermissionError) {
            form.setFieldError('permissions', pagesPermissionError);
          } else {
            form.clearFieldError('permissions');
          }

          const pagesDefaultRoleError = getPagesDefaultRoleError(
            nextPages,
            roleName,
            isDefaultRole,
          );
          if (pagesDefaultRoleError) {
            form.setFieldError('pages', pagesDefaultRoleError);
          } else {
            form.clearFieldError('pages');
          }
        };

        const updatePages = (nextPages: Page[]) => {
          form.setFieldValue('pages', nextPages);
          setSelectionErrors(nextPages, selectedPermissions);
        };

        return (
          <Stack gap="xs">
            <Text fw={500} size="sm">
              Displayed Pages
            </Text>
            {pageEntries.map(([page, definition]) => {
              const Icon = definition.icon;
              const pagePermissionDefinition = getPagePermissions(page);
              const hasRequiredPermissions = pagePermissionDefinition.necessaryPermissions.every(
                (permission) => selectedPermissionSet.has(permission),
              );
              const requiredDefaultRoles = pagePermissionDefinition.requiresDefaultRole;
              const hasRequiredDefaultRole =
                !requiredDefaultRoles ||
                requiredDefaultRoles.length === 0 ||
                (isDefaultRole &&
                  requiredDefaultRoles.includes(roleName as components['schemas']['DefaultRole']));
              const isChecked = selectedPages.includes(page);
              const canTogglePage = hasRequiredPermissions && (hasRequiredDefaultRole || isChecked);

              const addNecessaryPermissions = () => {
                const newPermissions = [...selectedPermissions];
                pagePermissionDefinition.necessaryPermissions.forEach((permission) => {
                  if (!newPermissions.includes(permission)) {
                    newPermissions.push(permission);
                  }
                });
                form.setFieldValue('permissions', newPermissions);
                setSelectionErrors(selectedPages, newPermissions);
              };

              const addAllPermissions = () => {
                const newPermissions = [...selectedPermissions];
                [
                  ...pagePermissionDefinition.necessaryPermissions,
                  ...pagePermissionDefinition.recommendedPermissions,
                ].forEach((permission) => {
                  if (!newPermissions.includes(permission)) {
                    newPermissions.push(permission);
                  }
                });
                form.setFieldValue('permissions', newPermissions);
                setSelectionErrors(selectedPages, newPermissions);
              };

              const statusColor = hasRequiredDefaultRole
                ? hasRequiredPermissions
                  ? 'green'
                  : 'red'
                : 'orange';
              const statusLabel = hasRequiredDefaultRole
                ? hasRequiredPermissions
                  ? 'Available'
                  : 'Missing required permissions'
                : `Requires default role: ${(requiredDefaultRoles ?? []).join(', ')}`;

              return (
                <Stack
                  key={page}
                  gap={4}
                  p="xs"
                  style={{
                    border: '1px solid var(--mantine-color-gray-3)',
                    borderRadius: '8px',
                  }}
                >
                  <Group justify="space-between" align="center" wrap="nowrap">
                    <Group gap="sm" wrap="nowrap">
                      <Checkbox
                        checked={isChecked}
                        disabled={!canTogglePage}
                        onChange={(event) => {
                          if (!hasRequiredPermissions || !hasRequiredDefaultRole) return;
                          if (event.currentTarget.checked) {
                            updatePages([...selectedPages, page]);
                          } else {
                            updatePages(
                              selectedPages.filter((currentPage) => currentPage !== page),
                            );
                          }
                        }}
                      />
                      <ThemeIcon variant="light" color={definition.color}>
                        <Icon size={16} />
                      </ThemeIcon>
                      <Text size="sm" fw={500}>
                        {definition.displayName}
                      </Text>
                    </Group>
                    <Badge color={statusColor} variant="light" size="sm">
                      {statusLabel}
                    </Badge>
                  </Group>
                  <Text size="xs" c="dimmed">
                    Required: {pagePermissionDefinition.necessaryPermissions.join(', ') || '-'}
                  </Text>
                  <Text size="xs" c="dimmed">
                    Recommended: {pagePermissionDefinition.recommendedPermissions.join(', ') || '-'}
                  </Text>
                  <Group gap="sm">
                    <Button
                      size="xs"
                      variant="light"
                      disabled={isPermissionsImmutable}
                      onClick={addNecessaryPermissions}
                    >
                      Add Necessary Permissions
                    </Button>
                    <Button
                      size="xs"
                      variant="light"
                      disabled={isPermissionsImmutable}
                      onClick={addAllPermissions}
                    >
                      Add All Permissions
                    </Button>
                  </Group>
                </Stack>
              );
            })}
          </Stack>
        );
      },
    } satisfies Field,
  ];

  const createRole = recommendedPerm('Permission:Role:Create')?.useMutation('post', '/api/Role', {
    onSuccess: () => refetchRoles(),
  }).mutate;
  const updateRole = recommendedPerm('Permission:Role:Update')?.useMutation(
    'put',
    '/api/Role/{id}',
    {
      onSuccess: () => refetchRoles(),
    },
  ).mutate;
  const deleteRole = recommendedPerm('Permission:Role:Delete')?.useMutation(
    'delete',
    '/api/Role/{id}',
    {
      onSuccess: () => refetchRoles(),
    },
  ).mutate;

  const validateRole = (values: RoleFormValues) => {
    const pagesPermissionError = getPagesPermissionError(
      values.pages ?? [],
      (values.permissions as Permission[] | undefined) ?? [],
    );
    const role =
      values.id != null ? roles.find((candidate) => candidate.id === values.id) : undefined;
    const roleName = (values.name ?? role?.name ?? '').trim();
    const isDefaultRole = !!role?.isDefault;
    const pagesDefaultRoleError = getPagesDefaultRoleError(
      values.pages ?? [],
      roleName,
      isDefaultRole,
    );

    return {
      ...(pagesPermissionError ? { permissions: pagesPermissionError } : {}),
      ...(pagesDefaultRoleError ? { pages: pagesDefaultRoleError } : {}),
    };
  };

  const canDeleteRole = (role: Role) => !role.isDefault;

  const handleSubmit = (values: RoleFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create' && createRole) {
      createRole({ body: values });
    } else if (mode === 'edit') {
      if (!updateRole) return;
      updateRole({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Role, RoleFormValues>
      title="Roles"
      entityName="role"
      data={roles}
      tableColumns={tableColumns}
      editorFields={editorFields}
      validate={validateRole}
      onSubmit={handleSubmit}
      onDelete={(r) => {
        if (!deleteRole) return;
        if (!canDeleteRole(r)) return;
        deleteRole({ params: { path: { id: r.id } } });
      }}
      canCreate={!!createRole}
      canEdit={!!updateRole}
      canDelete={!!deleteRole}
      canEditItem={() => true}
      canDeleteItem={canDeleteRole}
    />
  );
};
