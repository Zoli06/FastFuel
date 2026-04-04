import type { components } from '../../../types/api-schema.generated.ts';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { $api } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

type Customer = components['schemas']['CustomerResponseDto'];

type CustomerFormValues = Customer & { password?: string | null };

export const CustomerManager = () => {
  const { noPerm, necessaryPerm, recommendedPerm } = $api('CustomerManager');

  const [{ data: customers = [], refetch: refetchCustomers }] = useConditionalSuspenseQueries([
    necessaryPerm('Permission:Customer:Read').queryOptions('get', '/api/Customer'),
  ]);

  const tableColumns: ColumnDefinition<Customer>[] = [
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

  const createCustomer = noPerm().useMutation('post', '/api/Customer', {
    onSuccess: () => refetchCustomers(),
  }).mutate;
  const updateCustomer = recommendedPerm('Permission:Customer:Update')?.useMutation(
    'put',
    '/api/Customer/{id}',
    {
      onSuccess: () => refetchCustomers(),
    },
  ).mutate;
  const deleteCustomer = recommendedPerm('Permission:Customer:Delete')?.useMutation(
    'delete',
    '/api/Customer/{id}',
    {
      onSuccess: () => refetchCustomers(),
    },
  ).mutate;

  const toRequestDto = (values: CustomerFormValues) =>
    ({
      name: values.name,
      email: values.email,
      userName: values.userName,
      themeId: null,
      password: values.password || null,
      // https://github.com/openapi-ts/openapi-typescript/issues/1520
    }) as components['schemas']['CustomerRequestDto'];

  const handleSubmit = (values: CustomerFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createCustomer({ body: toRequestDto(values) });
    } else if (updateCustomer) {
      updateCustomer({ params: { path: { id: values.id } }, body: toRequestDto(values) });
    }
  };

  return (
    <EntityManager<CustomerFormValues>
      title="Customers"
      entityName="customer"
      data={customers}
      tableColumns={tableColumns as ColumnDefinition<CustomerFormValues>[]}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={
        deleteCustomer ? (e) => deleteCustomer({ params: { path: { id: e.id } } }) : undefined
      }
      canCreate={true}
      canEdit={!!updateCustomer}
      canDelete={!!deleteCustomer}
    />
  );
};
