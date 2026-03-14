import type { components } from '../../../types/api';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';

type StationCategory = components['schemas']['StationCategoryResponseDto'];
type Ingredient = components['schemas']['IngredientResponseDto'];

export type StationCategoryManagerProps = {
  stationCategories: StationCategory[];
  refetchStationCategories: () => void;
  ingredients: Ingredient[];
};

export const StationCategoryManager = ({
  stationCategories,
  refetchStationCategories,
  ingredients,
}: StationCategoryManagerProps) => {
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
    />
  );
};
