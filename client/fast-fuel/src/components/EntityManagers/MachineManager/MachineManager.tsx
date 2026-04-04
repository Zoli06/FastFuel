import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { $api } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const MachineManager = () => {
  const { necessaryPerm, recommendedPerm } = $api('MachineManager');
  const restaurantReadApi = recommendedPerm('Permission:Restaurant:Read');

  const [{ data: restaurants = [] }, { data: machines = [], refetch: refetchMachines }] =
    useConditionalSuspenseQueries([
      restaurantReadApi?.queryOptions('get', '/api/Restaurant'),
      necessaryPerm('Permission:Machine:Read').queryOptions('get', '/api/Machine'),
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
        if (restaurantReadApi) {
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

  const createMachine = recommendedPerm('Permission:Machine:Create')?.useMutation(
    'post',
    '/api/Machine',
    {
      onSuccess: () => refetchMachines(),
    },
  ).mutate;
  const updateMachine = recommendedPerm('Permission:Machine:Update')?.useMutation(
    'put',
    '/api/Machine/{id}',
    {
      onSuccess: () => refetchMachines(),
    },
  ).mutate;
  const deleteMachine = recommendedPerm('Permission:Machine:Delete')?.useMutation(
    'delete',
    '/api/Machine/{id}',
    {
      onSuccess: () => refetchMachines(),
    },
  ).mutate;

  const toRequestDto = (values: MachineFormValues) => ({
    name: values.name,
    userName: values.userName,
    themeId: null,
    password: values.password ?? null,
    locatedAtRestaurantId: values.locatedAtRestaurantId,
  });

  const handleSubmit = (values: MachineFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create' && createMachine) {
      createMachine({ body: toRequestDto(values) });
    } else if (mode === 'edit' && updateMachine) {
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
      onDelete={
        deleteMachine ? (m) => deleteMachine({ params: { path: { id: m.id } } }) : undefined
      }
      canCreate={!!createMachine}
      canEdit={!!updateMachine}
      canDelete={!!deleteMachine}
    />
  );
};
