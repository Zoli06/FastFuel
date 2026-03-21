import { TextInput } from '@mantine/core';
import { apiClient } from '../../../lib/api-client.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';
import { useSuspensePermissions } from '../../../hooks/useSuspensePermissions.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';

export const RoleManager = () => {
  const can = useSuspensePermissions();

  const [
    { data: users = [] },
    { data: permissions = [] },
    { data: roles = [], refetch: refetchRoles },
  ] = useConditionalSuspenseQueries([
    can.User.Read && apiClient.queryOptions('get', '/api/User'),
    can.Permission.Read && apiClient.queryOptions('get', '/api/Permission'),
    apiClient.queryOptions('get', '/api/Role'),
  ]);

  type Role = (typeof roles)[number];
  type RoleFormValues = Pick<Role, 'id' | 'name' | 'permissions' | 'userIds'>;

  const userOptions = users.map((user) => ({
    value: user.id,
    label: `${user.name} (${user.userType})`,
  }));
  const permissionOptions = permissions.map((permission) => ({
    value: permission,
    label: permission,
  }));

  const tableColumns: ColumnDefinition<Role>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Is Default', render: (r) => (r.isDefault ? 'Yes' : 'No') },
    { header: 'Is Immutable', render: (r) => (r.isImmutable ? 'Yes' : 'No') },
  ];

  const editorFields: Field[] = [
    {
      type: 'custom',
      render: (form, mode) => {
        const editedRole = roles.find((r) => r.id === form.getValues().id);
        const disableName = mode === 'edit' && !!editedRole?.isDefault;

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
    ...(can.Permission.Read
      ? [
          {
            type: 'multiselect',
            key: 'permissions',
            label: 'Permissions',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            fieldProps: {
              data: permissionOptions,
              placeholder: 'Search permissions...',
              searchable: true,
              clearable: true,
            },
          } satisfies Field,
        ]
      : []),
    ...(can.User.Read
      ? [
          {
            type: 'numericMultiSelect',
            key: 'userIds',
            label: 'Users',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            fieldProps: {
              data: userOptions,
              placeholder: 'Search users...',
              searchable: true,
              clearable: true,
            },
          } satisfies Field,
        ]
      : []),
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

  const toRequestDto = (values: RoleFormValues) => ({
    name: values.name,
    permissions: values.permissions,
    userIds: values.userIds,
  });

  const canEditRole = (role: Role) => !role.isImmutable;
  const canDeleteRole = (role: Role) => !role.isDefault && !role.isImmutable;

  const handleSubmit = (values: RoleFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createRole({ body: toRequestDto(values) });
    } else {
      const role = roles.find((r) => r.id === values.id);
      if (role && !canEditRole(role)) return;
      updateRole({ params: { path: { id: values.id } }, body: toRequestDto(values) });
    }
  };

  return (
    <EntityManager<Role, RoleFormValues>
      title="Roles"
      entityName="role"
      data={roles}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(r) => {
        if (!canDeleteRole(r)) return;
        deleteRole({ params: { path: { id: r.id } } });
      }}
      canCreate={can.Role.Create}
      canEdit={can.Role.Update}
      canDelete={can.Role.Delete}
      canEditItem={canEditRole}
      canDeleteItem={canDeleteRole}
    />
  );
};
