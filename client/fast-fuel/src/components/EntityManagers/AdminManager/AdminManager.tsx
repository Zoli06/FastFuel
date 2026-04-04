import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { $api } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';
import type { components } from '../../../types/api-schema.generated.ts';

export const AdminManager = () => {
  const { necessaryPerm, recommendedPerm } = $api('AdminManager');

  const [{ data: admins = [], refetch: refetchAdmins }] = useConditionalSuspenseQueries([
    necessaryPerm('Permission:Admin:Read').queryOptions('get', '/api/Admin'),
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

  const createAdmin = recommendedPerm('Permission:Admin:Create')?.useMutation(
    'post',
    '/api/Admin',
    {
      onSuccess: () => refetchAdmins(),
    },
  ).mutate;
  const updateAdmin = recommendedPerm('Permission:Admin:Update')?.useMutation(
    'put',
    '/api/Admin/{id}',
    {
      onSuccess: () => refetchAdmins(),
    },
  ).mutate;
  const deleteAdmin = recommendedPerm('Permission:Admin:Delete')?.useMutation(
    'delete',
    '/api/Admin/{id}',
    {
      onSuccess: () => refetchAdmins(),
    },
  ).mutate;

  const toRequestDto = (values: AdminFormValues) =>
    ({
      name: values.name,
      email: values.email,
      userName: values.userName,
      themeId: null,
      password: values.password ?? null,
    }) as components['schemas']['AdminRequestDto'];

  const handleSubmit = (values: AdminFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create' && createAdmin) {
      createAdmin({ body: toRequestDto(values) });
    } else if (mode === 'edit' && updateAdmin) {
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
      onDelete={deleteAdmin ? (a) => deleteAdmin({ params: { path: { id: a.id } } }) : undefined}
      canCreate={!!createAdmin}
      canEdit={!!updateAdmin}
      canDelete={!!deleteAdmin}
    />
  );
};
