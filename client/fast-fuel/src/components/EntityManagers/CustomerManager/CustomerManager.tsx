import type { components } from '../../../types/api';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';

type Customer = components['schemas']['CustomerResponseDto'];

type CustomerFormValues = Customer & { password?: string | null };

export type CustomerManagerProps = {
  customers: Customer[];
  refetchCustomers: () => void;
};

export const CustomerManager = ({ customers, refetchCustomers }: CustomerManagerProps) => {
  const tableColumns: ColumnDefinition<Customer>[] = [
    //     name: string
    //   email: string
    //   userName: string
    //   themeId: number | null
    //   roleIds: number[]
    //   userType: string
    //   id: number
    // } & {
    //   orderIds?: number[]
    // }

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
      type: 'text',
      key: 'email',
      label: 'Email',
      initialValue: '',
      nullable: 'never',
      required: 'always',
    },
  ];

  const { mutate: createCustomer } = apiClient.useMutation('post', '/api/Customer', {
    onSuccess: () => refetchCustomers(),
  });
  const { mutate: updateCustomer } = apiClient.useMutation('put', '/api/Customer/{id}', {
    onSuccess: () => refetchCustomers(),
  });
  const { mutate: deleteCustomer } = apiClient.useMutation('delete', '/api/Customer/{id}', {
    onSuccess: () => refetchCustomers(),
  });

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
    } else {
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
      onDelete={(e) => deleteCustomer({ params: { path: { id: e.id } } })}
    />
  );
};
