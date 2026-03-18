import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { useSuspenseQueries } from '@tanstack/react-query';

export const StationCategoryManager = () => {
  const [
    { data: permissions },
    { data: stationCategories, refetch: refetchStationCategories },
    { data: ingredients },
  ] = useSuspenseQueries({
    queries: [
      apiClient.queryOptions('get', '/api/Permission/my'),
      apiClient.queryOptions('get', '/api/StationCategory'),
      apiClient.queryOptions('get', '/api/Ingredient'),
    ] as const,
  });
  type StationCategory = (typeof stationCategories)[number];
  const ingredientOptions = (ingredients ?? []).map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<StationCategory>[] = [
    { header: 'Name', accessor: 'name' },
    {
      header: 'Ingredients',
      render: (r) => {
        if (!r.ingredientIds?.length) return 'None';
        return r.ingredientIds
          .map((id) => ingredientOptions.find((o) => o.value === id)?.label ?? `#${id}`)
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
      type: 'numericMultiSelect',
      key: 'ingredientIds',
      label: 'Ingredients',
      initialValue: [],
      nullable: 'never',
      required: 'never',
      fieldProps: {
        data: ingredientOptions,
        placeholder: 'Search ingredients...',
        searchable: true,
        clearable: true,
      },
    },
  ];

  const { mutate: createStationCategory } = apiClient.useMutation('post', '/api/StationCategory', {
    onSuccess: () => refetchStationCategories(),
  });
  const { mutate: updateStationCategory } = apiClient.useMutation(
    'put',
    '/api/StationCategory/{id}',
    {
      onSuccess: () => refetchStationCategories(),
    },
  );
  const { mutate: deleteStationCategory } = apiClient.useMutation(
    'delete',
    '/api/StationCategory/{id}',
    {
      onSuccess: () => refetchStationCategories(),
    },
  );

  const handleSubmit = (values: StationCategory, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createStationCategory({ body: values });
    } else {
      updateStationCategory({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<StationCategory>
      title="Station Categories"
      entityName="station category"
      data={stationCategories}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(r) => deleteStationCategory({ params: { path: { id: r.id } } })}
      canCreate={permissions?.includes('Permission:StationCategory:Create')}
      canEdit={permissions?.includes('Permission:StationCategory:Update')}
      canDelete={permissions?.includes('Permission:StationCategory:Delete')}
    />
  );
};
