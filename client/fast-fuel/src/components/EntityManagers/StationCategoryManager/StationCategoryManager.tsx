import { useApi } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const StationCategoryManager = () => {
  const { useNecessaryPerm, useRecommendedPerm } = useApi('StationCategoryManager');
  const ingredientReadApi = useRecommendedPerm('Permission:Ingredient:Read');

  const [
    { data: ingredients = [] },
    { data: stationCategories = [], refetch: refetchStationCategories },
  ] = useConditionalSuspenseQueries([
    ingredientReadApi?.queryOptions('get', '/api/Ingredient'),
    useNecessaryPerm('Permission:StationCategory:Read').queryOptions('get', '/api/StationCategory'),
  ]);

  type StationCategory = (typeof stationCategories)[number];

  const ingredientOptions = ingredients.map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<StationCategory>[] = [
    { header: 'Name', accessor: 'name' },
    ...(ingredientReadApi
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
    ...(ingredientReadApi
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

  const createStationCategory = useRecommendedPerm(
    'Permission:StationCategory:Create',
  )?.useMutation('post', '/api/StationCategory', {
    onSuccess: () => refetchStationCategories(),
  }).mutateAsync;
  const updateStationCategory = useRecommendedPerm(
    'Permission:StationCategory:Update',
  )?.useMutation('put', '/api/StationCategory/{id}', {
    onSuccess: () => refetchStationCategories(),
  }).mutateAsync;
  const deleteStationCategory = useRecommendedPerm(
    'Permission:StationCategory:Delete',
  )?.useMutation('delete', '/api/StationCategory/{id}', {
    onSuccess: () => refetchStationCategories(),
  }).mutate;

  const handleSubmit = async (values: StationCategory, mode: 'create' | 'edit') => {
    if (mode === 'create' && createStationCategory) {
      await createStationCategory({ body: values });
    } else if (mode === 'edit' && updateStationCategory) {
      await updateStationCategory({ params: { path: { id: values.id } }, body: values });
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
      onDelete={
        deleteStationCategory
          ? (r) => deleteStationCategory({ params: { path: { id: r.id } } })
          : undefined
      }
      canCreate={!!createStationCategory}
      canEdit={!!updateStationCategory}
      canDelete={!!deleteStationCategory}
    />
  );
};
