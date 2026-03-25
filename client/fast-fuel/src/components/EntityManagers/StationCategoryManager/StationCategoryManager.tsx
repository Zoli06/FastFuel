import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const StationCategoryManager = () => {
  const { necessary, recommended } = usePagePermissions('StationCategoryManager', { split: true });

  const [
    { data: ingredients = [] },
    { data: stationCategories = [], refetch: refetchStationCategories },
  ] = useConditionalSuspenseQueries([
    recommended.Ingredient.Read && apiClient.queryOptions('get', '/api/Ingredient'),
    necessary.StationCategory.Read && apiClient.queryOptions('get', '/api/StationCategory'),
  ]);

  type StationCategory = (typeof stationCategories)[number];

  const ingredientOptions = ingredients.map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<StationCategory>[] = [
    { header: 'Name', accessor: 'name' },
    ...(recommended.Ingredient.Read
      ? [
          {
            header: 'Ingredients',
            render: (r: StationCategory) => {
              if (!r.ingredientIds?.length) return 'None';
              return r.ingredientIds
                .map((id) => ingredientOptions.find((o) => o.value === id)?.label ?? `#${id}`)
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
    ...(recommended.Ingredient.Read
      ? [
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
          } satisfies Field,
        ]
      : []),
  ];

  const { mutate: createStationCategory } = apiClient.useMutation('post', '/api/StationCategory', {
    onSuccess: () => refetchStationCategories(),
  });
  const { mutate: updateStationCategory } = apiClient.useMutation(
    'put',
    '/api/StationCategory/{id}',
    { onSuccess: () => refetchStationCategories() },
  );
  const { mutate: deleteStationCategory } = apiClient.useMutation(
    'delete',
    '/api/StationCategory/{id}',
    { onSuccess: () => refetchStationCategories() },
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
      canCreate={recommended.StationCategory.Create}
      canEdit={recommended.StationCategory.Update}
      canDelete={recommended.StationCategory.Delete}
    />
  );
};
