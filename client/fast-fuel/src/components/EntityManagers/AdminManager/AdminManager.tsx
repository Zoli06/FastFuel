import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';
import type { components } from '../../../types/api';

export const AdminManager = () => {
  const { recommended } = usePagePermissions('AdminManager', { split: true });

  const [{ data: admins = [], refetch: refetchAdmins }] = useConditionalSuspenseQueries([
    apiClient.queryOptions('get', '/api/Admin'),
  ]);

  type Admin = (typeof admins)[number];
  type AdminFormValues = Admin & { password?: string | null };

  const tableColumns: ColumnDefinition<Admin>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Username', accessor: 'userName' },
    { header: 'Email', accessor: 'email' },
  ];

  const editorFields: Field[] = [
    {
      type: 'text',
      key: 'name',
      label: 'Name',
      initialValue: '',
      nullable: 'never',
      required: 'always',
    },
    {
      type: 'text',
      key: 'userName',
      label: 'Username',
      initialValue: '',
      nullable: 'never',
      required: 'always',
    },
    {
      type: 'email',
      key: 'email',
      label: 'Email',
      initialValue: '',
      nullable: 'never',
      required: 'always',
    },
    {
      type: 'password',
      key: 'password',
      label: 'Password',
      initialValue: '',
      nullable: 'edit',
      required: 'create',
    },
  ];

  const { mutate: createAdmin } = apiClient.useMutation('post', '/api/Admin', {
    onSuccess: () => refetchAdmins(),
  });
  const { mutate: updateAdmin } = apiClient.useMutation('put', '/api/Admin/{id}', {
    onSuccess: () => refetchAdmins(),
  });
  const { mutate: deleteAdmin } = apiClient.useMutation('delete', '/api/Admin/{id}', {
    onSuccess: () => refetchAdmins(),
  });

  const toRequestDto = (values: AdminFormValues) =>
    ({
      name: values.name,
      email: values.email,
      userName: values.userName,
      themeId: null,
      password: values.password ?? null,
    }) as components['schemas']['AdminRequestDto'];

  const handleSubmit = (values: AdminFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createAdmin({ body: toRequestDto(values) });
    } else {
      updateAdmin({ params: { path: { id: values.id } }, body: toRequestDto(values) });
    }
  };

  return (
    <EntityManager<AdminFormValues>
      title="Admins"
      entityName="admin"
      data={admins}
      tableColumns={tableColumns as ColumnDefinition<AdminFormValues>[]}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(a) => deleteAdmin({ params: { path: { id: a.id } } })}
      canCreate={recommended.Admin.Create}
      canEdit={recommended.Admin.Update}
      canDelete={recommended.Admin.Delete}
    />
  );
};
