import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const MachineManager = () => {
  const { necessary, recommended } = usePagePermissions('MachineManager', { split: true });

  const [{ data: restaurants = [] }, { data: machines = [], refetch: refetchMachines }] =
    useConditionalSuspenseQueries([
      recommended.Restaurant.Read && apiClient.queryOptions('get', '/api/Restaurant'),
      necessary.Machine.Read && apiClient.queryOptions('get', '/api/Machine'),
    ]);

  type Machine = (typeof machines)[number];
  type MachineFormValues = Machine & { password?: string | null };

  const restaurantOptions = restaurants.map((r) => ({
    value: r.id,
    label: r.name,
  }));

  const tableColumns: ColumnDefinition<Machine>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Username', accessor: 'userName' },
    {
      header: 'Located At',
      render: (m: Machine) => {
        if (recommended.Restaurant.Read) {
          return (
            restaurantOptions.find((o) => o.value === m.locatedAtRestaurantId)?.label ??
            `#${m.locatedAtRestaurantId}`
          );
        }
        return `#${m.locatedAtRestaurantId}`;
      },
    },
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
      type: 'password',
      key: 'password',
      label: 'Password',
      initialValue: '',
      nullable: 'edit',
      required: 'create',
    },
    {
      type: 'numericSelect',
      key: 'locatedAtRestaurantId',
      label: 'Located At',
      initialValue: null,
      nullable: 'never',
      required: 'always',
      fieldProps: {
        data: restaurantOptions,
        placeholder: 'Select restaurant...',
        searchable: true,
      },
    } satisfies Field,
  ];

  const { mutate: createMachine } = apiClient.useMutation('post', '/api/Machine', {
    onSuccess: () => refetchMachines(),
  });
  const { mutate: updateMachine } = apiClient.useMutation('put', '/api/Machine/{id}', {
    onSuccess: () => refetchMachines(),
  });
  const { mutate: deleteMachine } = apiClient.useMutation('delete', '/api/Machine/{id}', {
    onSuccess: () => refetchMachines(),
  });

  const toRequestDto = (values: MachineFormValues) => ({
    name: values.name,
    userName: values.userName,
    themeId: null,
    password: values.password ?? null,
    locatedAtRestaurantId: values.locatedAtRestaurantId,
  });

  const handleSubmit = (values: MachineFormValues, mode: 'create' | 'edit') => {
    console.log(values);
    console.log(toRequestDto(values));

    if (mode === 'create') {
      createMachine({ body: toRequestDto(values) });
    } else {
      updateMachine({ params: { path: { id: values.id } }, body: toRequestDto(values) });
    }
  };

  return (
    <EntityManager<MachineFormValues>
      title="Machines"
      entityName="machine"
      data={machines}
      tableColumns={tableColumns as ColumnDefinition<MachineFormValues>[]}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(m) => deleteMachine({ params: { path: { id: m.id } } })}
      canCreate={recommended.Machine.Create}
      canEdit={recommended.Machine.Update}
      canDelete={recommended.Machine.Delete}
    />
  );
};
