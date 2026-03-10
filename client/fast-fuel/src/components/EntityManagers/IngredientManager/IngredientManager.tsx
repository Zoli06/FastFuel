import type { components } from '../../../types/api';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import { Image } from '@mantine/core';
import type { Field } from '../../EntityManager/EntityEditor';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';

type Ingredient = components['schemas']['IngredientResponseDto'];
type Allergy = components['schemas']['AllergyResponseDto'];
type StationCategory = components['schemas']['StationCategoryResponseDto'];

export type IngredientManagerProps = {
  ingredients: Ingredient[];
  refetchIngredients: () => void;
  allergies: Allergy[];
  stationCategories: StationCategory[];
};

export const IngredientManager = ({
  ingredients,
  refetchIngredients,
  allergies,
  stationCategories,
}: IngredientManagerProps) => {
  const tableColumns: ColumnDefinition<Ingredient>[] = [
    { header: 'Name', accessor: 'name' },
    {
      header: 'Image',
      render: (ingredient) =>
        ingredient.imageUrl ? (
          <Image src={ingredient.imageUrl} alt={ingredient.name} width={50} height={50} />
        ) : (
          'No image'
        ),
    },
    {
      header: 'Allergies',
      render: (ingredient) => {
        if (!ingredient.allergyIds?.length) return 'None';
        return ingredient.allergyIds
          .map((id) => allergies.find((a) => a.id === id)?.name ?? id)
          .join(', ');
      },
    },
    {
      header: 'Station Categories',
      render: (ingredient) => {
        if (!ingredient.stationCategoryIds?.length) return 'None';
        return ingredient.stationCategoryIds
          .map((id) => stationCategories.find((sc) => sc.id === id)?.name ?? id)
          .join(', ');
      },
    },
    {
      header: 'Default Timer Value',
      accessor: 'defaultTimerValueSeconds',
      render: (r) => r.defaultTimerValueSeconds + 's',
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
      key: 'imageUrl',
      label: 'Image URL',
      nullable: 'always',
      required: 'never',
      initialValue: '',
    },
    {
      type: 'numericMultiSelect',
      key: 'allergyIds',
      label: 'Allergies',
      initialValue: [],
      nullable: 'never',
      required: 'never',
      selectProps: {
        data: allergies.map((allergy) => ({ value: allergy.id, label: allergy.name })),
        placeholder: 'Select allergies',
        searchable: true,
      },
    },
    {
      type: 'numericMultiSelect',
      key: 'stationCategoryIds',
      label: 'Station Categories',
      initialValue: [],
      nullable: 'never',
      required: 'never',
      selectProps: {
        data: stationCategories.map((sc) => ({ value: sc.id, label: sc.name })),
        placeholder: 'Select station categories',
        searchable: true,
      },
    },
    {
      type: 'number',
      key: 'defaultTimerValueSeconds',
      label: 'Default Timer Value (seconds)',
      initialValue: 0,
      nullable: 'never',
      required: 'always',
    },
  ];

  const { mutate: createIngredient } = apiClient.useMutation('post', '/api/Ingredient', {
    onSuccess: () => refetchIngredients(),
  });
  const { mutate: updateIngredient } = apiClient.useMutation('put', '/api/Ingredient/{id}', {
    onSuccess: () => refetchIngredients(),
  });
  const { mutate: deleteIngredient } = apiClient.useMutation('delete', '/api/Ingredient/{id}', {
    onSuccess: () => refetchIngredients(),
  });

  const handleSubmit = (values: Ingredient, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createIngredient({ body: values });
    } else {
      updateIngredient({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Ingredient>
      title="Ingredients"
      entityName="ingredients"
      data={ingredients}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(r) => deleteIngredient({ params: { path: { id: r.id } } })}
    />
  );
};
