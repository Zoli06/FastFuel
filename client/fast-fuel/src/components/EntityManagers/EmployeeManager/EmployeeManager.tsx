import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const EmployeeManager = () => {
  const { recommended } = usePagePermissions('EmployeeManager');

  const [
    { data: stationCategories = [] },
    { data: restaurants = [] },
    { data: employees = [], refetch: refetchEmployees },
  ] = useConditionalSuspenseQueries([
    recommended.StationCategory.Read && apiClient.queryOptions('get', '/api/StationCategory'),
    recommended.Restaurant.Read && apiClient.queryOptions('get', '/api/Restaurant'),
    apiClient.queryOptions('get', '/api/Employee'),
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
      render: (e: Employee) => {
        if (recommended.Restaurant.Read) {
          return (
            restaurantOptions.find((o) => o.value === e.worksAtRestaurantId)?.label ??
            `#${e.worksAtRestaurantId}`
          );
        }
        return `#${e.worksAtRestaurantId}`;
      },
    },
    ...(recommended.StationCategory.Read
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
    ...(recommended.StationCategory.Read
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

  const { mutate: createEmployee } = apiClient.useMutation('post', '/api/Employee', {
    onSuccess: () => refetchEmployees(),
  });
  const { mutate: updateEmployee } = apiClient.useMutation('put', '/api/Employee/{id}', {
    onSuccess: () => refetchEmployees(),
  });
  const { mutate: deleteEmployee } = apiClient.useMutation('delete', '/api/Employee/{id}', {
    onSuccess: () => refetchEmployees(),
  });

  const toRequestDto = (values: EmployeeFormValues) => ({
    name: values.name,
    email: values.email,
    userName: values.userName,
    password: values.password ?? null,
    shiftIds: values.shiftIds ?? [],
    stationCategoryIds: values.stationCategoryIds,
    worksAtRestaurantId: values.worksAtRestaurantId,
  });

  const handleSubmit = (values: EmployeeFormValues, mode: 'create' | 'edit') => {
    console.log(values);
    console.log(toRequestDto(values));

    if (mode === 'create') {
      createEmployee({ body: toRequestDto(values) });
    } else {
      updateEmployee({ params: { path: { id: values.id } }, body: toRequestDto(values) });
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
      onDelete={(e) => deleteEmployee({ params: { path: { id: e.id } } })}
      canCreate={recommended.Employee.Create}
      canEdit={recommended.Employee.Update}
      canDelete={recommended.Employee.Delete}
    />
  );
};
