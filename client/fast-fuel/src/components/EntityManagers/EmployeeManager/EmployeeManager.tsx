import type { components } from '../../../types/api';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';

type Employee = components['schemas']['EmployeeResponseDto'];
type StationCategory = components['schemas']['StationCategoryResponseDto'];

type EmployeeFormValues = Employee & { password?: string | null };

export type EmployeeManagerProps = {
  employees: Employee[];
  refetchEmployees: () => void;
  stationCategories: StationCategory[];
};

export const EmployeeManager = ({
  employees,
  refetchEmployees,
  stationCategories,
}: EmployeeManagerProps) => {
  const stationCategoryOptions = (stationCategories ?? []).map((sc) => ({
    value: sc.id,
    label: sc.name,
  }));

  const tableColumns: ColumnDefinition<Employee>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Username', accessor: 'userName' },
    { header: 'Email', accessor: 'email' },
    { header: 'User Type', accessor: 'userType' },
    {
      header: 'Station Categories',
      render: (e) => {
        if (!e.stationCategoryIds?.length) return 'None';
        return e.stationCategoryIds
          .map((id) => stationCategoryOptions.find((o) => o.value === id)?.label ?? `#${id}`)
          .join(', ');
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
      type: 'text',
      key: 'email',
      label: 'Email',
      initialValue: '',
      nullable: 'never',
      required: 'always',
    },
    {
      type: 'text',
      key: 'password',
      label: 'Password',
      initialValue: '',
      nullable: 'always',
      required: 'never',
    },
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
    },
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
    themeId: null,
    password: values.password ?? null,
    shiftIds: values.shiftIds,
    stationCategoryIds: values.stationCategoryIds,
  });

  const handleSubmit = (values: EmployeeFormValues, mode: 'create' | 'edit') => {
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
    />
  );
};
