import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { useApi } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const EmployeeManager = () => {
  const { useNecessaryPerm, useRecommendedPerm, useNoPerm } = useApi('EmployeeManager');
  const stationCategoryReadApi = useRecommendedPerm('Permission:StationCategory:Read');

  const [
    { data: stationCategories = [] },
    { data: restaurants = [] },
    { data: employees = [], refetch: refetchEmployees },
  ] = useConditionalSuspenseQueries([
    stationCategoryReadApi?.queryOptions('get', '/api/StationCategory'),
    useNoPerm().queryOptions('get', '/api/Restaurant'),
    useNecessaryPerm('Permission:Employee:Read').queryOptions('get', '/api/Employee'),
  ]);

  type Employee = (typeof employees)[number];
  type EmployeeFormValues = Employee & { password?: string | null };

  const stationCategoryOptions = stationCategories.map((sc) => ({
    value: sc.id,
    label: sc.name,
  }));
  const restaurantOptions = restaurants.map((r) => ({
    value: r.id,
    label: r.name,
  }));

  const tableColumns: ColumnDefinition<Employee>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Username', accessor: 'userName' },
    { header: 'Email', accessor: 'email' },
    { header: 'User Type', accessor: 'userType' },
    {
      header: 'Works At',
      render: (e: Employee) =>
        restaurantOptions.find((o) => o.value === e.worksAtRestaurantId)?.label,
    },
    ...(stationCategoryReadApi
      ? [
          {
            header: 'Station Categories',
            render: (e: Employee) => {
              if (!e.stationCategoryIds?.length) return 'None';
              return e.stationCategoryIds
                .map((id) => stationCategoryOptions.find((o) => o.value === id)?.label ?? `#${id}`)
                .join(', ');
            },
          },
        ]
      : []),
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
    {
      type: 'numericSelect',
      key: 'worksAtRestaurantId',
      label: 'Works At',
      initialValue: null,
      nullable: 'never',
      required: 'always',
      fieldProps: {
        data: restaurantOptions,
        placeholder: 'Select restaurant...',
        searchable: true,
      },
    } satisfies Field,
    ...(stationCategoryReadApi
      ? [
          {
            type: 'numericMultiSelect',
            key: 'stationCategoryIds',
            label: 'Station Categories',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            fieldProps: {
              data: stationCategoryOptions,
              placeholder: 'Search station categories...',
              searchable: true,
              clearable: true,
            },
          } satisfies Field,
        ]
      : []),
  ];

  const createEmployee = useRecommendedPerm('Permission:Employee:Create')?.useMutation(
    'post',
    '/api/Employee',
    {
      onSuccess: () => refetchEmployees(),
    },
  ).mutateAsync;
  const updateEmployee = useRecommendedPerm('Permission:Employee:Update')?.useMutation(
    'put',
    '/api/Employee/{id}',
    {
      onSuccess: () => refetchEmployees(),
    },
  ).mutateAsync;
  const deleteEmployee = useRecommendedPerm('Permission:Employee:Delete')?.useMutation(
    'delete',
    '/api/Employee/{id}',
    {
      onSuccess: () => refetchEmployees(),
    },
  ).mutate;

  const toRequestDto = (values: EmployeeFormValues) => ({
    name: values.name,
    email: values.email,
    userName: values.userName,
    password: values.password ?? null,
    shiftIds: values.shiftIds ?? [],
    stationCategoryIds: values.stationCategoryIds,
    worksAtRestaurantId: values.worksAtRestaurantId,
  });

  const handleSubmit = async (values: EmployeeFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create' && createEmployee) {
      await createEmployee({ body: toRequestDto(values) });
    } else if (mode === 'edit' && updateEmployee) {
      await updateEmployee({ params: { path: { id: values.id } }, body: toRequestDto(values) });
    }
  };

  return (
    <EntityManager<EmployeeFormValues>
      title="Employees"
      entityName="employee"
      data={employees}
      tableColumns={tableColumns as ColumnDefinition<EmployeeFormValues>[]}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={
        deleteEmployee ? (e) => deleteEmployee({ params: { path: { id: e.id } } }) : undefined
      }
      canCreate={!!createEmployee}
      canEdit={!!updateEmployee}
      canDelete={!!deleteEmployee}
    />
  );
};
