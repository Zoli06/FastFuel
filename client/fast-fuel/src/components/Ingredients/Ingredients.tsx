import type { components } from '../../types/api';
import type { ColumnDefinition } from '../GenericTable/GenericTable.tsx';
import { Image } from '@mantine/core';
import type { FieldOrFieldset } from '../GenericEditor';
import { apiClient } from '../../apiClient.ts';
import { EntityManager } from '../EntityManager/EntityManager.tsx';

type IngredientDto = components['schemas']['IngredientResponseDto'];
type AllergyDto = components['schemas']['AllergyResponseDto'];
type StationCategoryDto = components['schemas']['StationCategoryResponseDto'];

export type IngredientsProps = {
  ingredients: IngredientDto[];
  refetchIngredients: () => void;
  allergies: AllergyDto[];
  stationCategories: StationCategoryDto[];
};

type IngredientFormValues = {
  name: string;
  imageUrl: string;
  allergyIds: number[];
  stationCategoryIds: number[];
  defaultTimerValueSeconds: number;
};

export const Ingredients = ({
  ingredients,
  refetchIngredients,
  allergies,
  stationCategories,
}: IngredientsProps) => {
  const tableColumns: ColumnDefinition<IngredientDto>[] = [
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

  const editorFields: FieldOrFieldset<IngredientFormValues>[] = [
    { type: 'text', key: 'name', label: 'Name', required: true, initialValue: '' },
    { type: 'text', key: 'imageUrl', label: 'Image URL', nullable: true, initialValue: '' },
    {
      type: 'numericMultiSelect',
      key: 'allergyIds',
      label: 'Allergies',
      initialValue: [],
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
      required: true,
      initialValue: 0,
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

  const handleSubmit = (
    values: IngredientFormValues,
    mode: 'create' | 'edit',
    item: IngredientDto | null,
  ) => {
    const body = values;
    if (mode === 'create') {
      createIngredient({ body });
    } else {
      updateIngredient({ params: { path: { id: item!.id } }, body });
    }
  };

  return (
    <EntityManager<IngredientDto, IngredientFormValues>
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
