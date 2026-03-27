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
import { apiClient } from '../../../lib/api-client.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import {
  pageDefinitions,
  type Page,
  type PageDefinition,
  type Permission,
} from '../../../lib/page-permissions.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import { NumericMultiSelect } from '../../common/NumericCombobox/NumericMultiSelect.tsx';

export const RoleManager = () => {
  const { recommended } = usePagePermissions('RoleManager', { split: true });

  const [
    { data: users = [] },
    { data: permissions = [] },
    { data: roles = [], refetch: refetchRoles },
  ] = useConditionalSuspenseQueries([
    recommended.User.Read && apiClient.queryOptions('get', '/api/User'),
    recommended.Permission.Read && apiClient.queryOptions('get', '/api/Permission'),
    apiClient.queryOptions('get', '/api/Role'),
  ]);

  type Role = (typeof roles)[number];
  type RoleFormValues = Pick<Role, 'id' | 'name' | 'permissions' | 'pages' | 'userIds'>;

  const pageEntries = (Object.entries(pageDefinitions) as [Page, PageDefinition][]).sort(
    ([, left], [, right]) => left.displayName.localeCompare(right.displayName),
  );

  const getMissingNecessaryPermissions = (pages: Page[], selectedPermissions: Permission[]) => {
    const permissionSet = new Set(selectedPermissions);
    const missing = pages.flatMap(
      (page) =>
        pageDefinitions[page]?.necessaryPermissions.filter(
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
      const requiredDefaultRoles = (pageDefinitions[page] as PageDefinition)?.requiresDefaultRole;
      if (!requiredDefaultRoles || requiredDefaultRoles.length === 0) return false;
      if (!isDefaultRole) return true;
      return !requiredDefaultRoles.includes(roleName);
    });

    if (restrictedPages.length === 0) return undefined;

    const pageDescriptions = restrictedPages.map((page) => {
      const definition = pageDefinitions[page] as PageDefinition;
      const roles = definition.requiresDefaultRole?.join(', ') ?? '-';
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
    { header: 'Is Immutable', render: (r) => (r.isImmutable ? 'Yes' : 'No') },
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
    ...(recommended.Permission.Read
      ? [
          {
            type: 'custom',
            render: (form) => {
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
                  value={selectedPermissions}
                  error={form.errors.permissions}
                  onChange={(value) => {
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
    ...(recommended.User.Read
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
              const hasRequiredPermissions = definition.necessaryPermissions.every((permission) =>
                selectedPermissionSet.has(permission),
              );
              const requiredDefaultRoles = definition.requiresDefaultRole;
              const hasRequiredDefaultRole =
                !requiredDefaultRoles ||
                requiredDefaultRoles.length === 0 ||
                (isDefaultRole && requiredDefaultRoles.includes(roleName));
              const isChecked = selectedPages.includes(page);
              const canTogglePage = hasRequiredPermissions && (hasRequiredDefaultRole || isChecked);

              const addNecessaryPermissions = () => {
                const newPermissions = [...selectedPermissions];
                definition.necessaryPermissions.forEach((permission) => {
                  if (!newPermissions.includes(permission)) {
                    newPermissions.push(permission);
                  }
                });
                form.setFieldValue('permissions', newPermissions);
                setSelectionErrors(selectedPages, newPermissions);
              };

              const addAllPermissions = () => {
                const newPermissions = [...selectedPermissions];
                [...definition.necessaryPermissions, ...definition.recommendedPermissions].forEach(
                  (permission) => {
                    if (!newPermissions.includes(permission)) {
                      newPermissions.push(permission);
                    }
                  },
                );
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
                    Required: {definition.necessaryPermissions.join(', ') || '-'}
                  </Text>
                  <Text size="xs" c="dimmed">
                    Default role: {definition.requiresDefaultRole?.join(', ') || '-'}
                  </Text>
                  <Text size="xs" c="dimmed">
                    Recommended: {definition.recommendedPermissions.join(', ') || '-'}
                  </Text>
                  <Group gap="sm">
                    <Button size="xs" variant="light" onClick={addNecessaryPermissions}>
                      Add Necessary Permissions
                    </Button>
                    <Button size="xs" variant="light" onClick={addAllPermissions}>
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

  const { mutate: createRole } = apiClient.useMutation('post', '/api/Role', {
    onSuccess: () => refetchRoles(),
  });
  const { mutate: updateRole } = apiClient.useMutation('put', '/api/Role/{id}', {
    onSuccess: () => refetchRoles(),
  });
  const { mutate: deleteRole } = apiClient.useMutation('delete', '/api/Role/{id}', {
    onSuccess: () => refetchRoles(),
  });

  const toRequestDto = (values: RoleFormValues, fallbackRole?: Role) => ({
    name: values.name,
    permissions: values.permissions ?? fallbackRole?.permissions ?? [],
    pages: values.pages ?? fallbackRole?.pages ?? [],
    userIds: values.userIds ?? fallbackRole?.userIds ?? [],
  });

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

  const canEditRole = (role: Role) => !role.isImmutable;
  const canDeleteRole = (role: Role) => !role.isDefault && !role.isImmutable;

  const handleSubmit = (values: RoleFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createRole({ body: toRequestDto(values) });
    } else {
      const role = roles.find((r) => r.id === values.id);
      if (role && !canEditRole(role)) return;
      updateRole({ params: { path: { id: values.id } }, body: toRequestDto(values, role) });
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
        if (!canDeleteRole(r)) return;
        deleteRole({ params: { path: { id: r.id } } });
      }}
      canCreate={recommended.Role.Create}
      canEdit={recommended.Role.Update}
      canDelete={recommended.Role.Delete}
      canEditItem={canEditRole}
      canDeleteItem={canDeleteRole}
    />
  );
};
