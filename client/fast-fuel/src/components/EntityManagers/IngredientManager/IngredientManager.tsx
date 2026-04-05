import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import { Image } from '@mantine/core';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { $api } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const IngredientManager = () => {
  const { necessaryPerm, recommendedPerm } = $api('IngredientManager');
  const allergyReadApi = recommendedPerm('Permission:Allergy:Read');
  const stationCategoryReadApi = recommendedPerm('Permission:StationCategory:Read');

  const [
    { data: allergies = [] },
    { data: stationCategories = [] },
    { data: ingredients = [], refetch: refetchIngredients },
  ] = useConditionalSuspenseQueries([
    allergyReadApi?.queryOptions('get', '/api/Allergy'),
    stationCategoryReadApi?.queryOptions('get', '/api/StationCategory'),
    necessaryPerm('Permission:Ingredient:Read').queryOptions('get', '/api/Ingredient'),
  ]);

  type Ingredient = (typeof ingredients)[number];

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
    ...(allergyReadApi
      ? [
          {
            header: 'Allergies',
            render: (ingredient: Ingredient) => {
              if (!ingredient.allergyIds?.length) return 'None';
              return ingredient.allergyIds
                .map((id) => allergies.find((a) => a.id === id)?.name ?? id)
                .join(', ');
            },
          },
        ]
      : []),
    ...(stationCategoryReadApi
      ? [
          {
            header: 'Station Categories',
            render: (ingredient: Ingredient) => {
              if (!ingredient.stationCategoryIds?.length) return 'None';
              return ingredient.stationCategoryIds
                .map((id) => stationCategories.find((sc) => sc.id === id)?.name ?? id)
                .join(', ');
            },
          },
        ]
      : []),
    {
      header: 'Default Timer Value',
      accessor: 'defaultTimerValueSeconds',
      render: (r) => r.defaultTimerValueSeconds + 's',
    },
  ];

  const allergyOptions = allergies.map((allergy) => ({ value: allergy.id, label: allergy.name }));
  const stationCategoryOptions = stationCategories.map((sc) => ({ value: sc.id, label: sc.name }));

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
    ...(allergyReadApi
      ? [
          {
            type: 'numericMultiSelect',
            key: 'allergyIds',
            label: 'Allergies',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            fieldProps: {
              data: allergyOptions,
              placeholder: 'Select allergies',
              searchable: true,
            },
          } satisfies Field,
        ]
      : []),
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
              placeholder: 'Select station categories',
              searchable: true,
            },
          } satisfies Field,
        ]
      : []),
    {
      type: 'number',
      key: 'defaultTimerValueSeconds',
      label: 'Default Timer Value (seconds)',
      initialValue: 0,
      nullable: 'never',
      required: 'always',
    },
  ];

  const createIngredient = recommendedPerm('Permission:Ingredient:Create')?.useMutation(
    'post',
    '/api/Ingredient',
    {
      onSuccess: () => refetchIngredients(),
    },
  ).mutate;
  const updateIngredient = recommendedPerm('Permission:Ingredient:Update')?.useMutation(
    'put',
    '/api/Ingredient/{id}',
    {
      onSuccess: () => refetchIngredients(),
    },
  ).mutate;
  const deleteIngredient = recommendedPerm('Permission:Ingredient:Delete')?.useMutation(
    'delete',
    '/api/Ingredient/{id}',
    {
      onSuccess: () => refetchIngredients(),
    },
  ).mutate;

  const handleSubmit = (values: Ingredient, mode: 'create' | 'edit') => {
    if (mode === 'create' && createIngredient) {
      createIngredient({ body: values });
    } else if (mode === 'edit' && updateIngredient) {
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
      onDelete={
        deleteIngredient ? (r) => deleteIngredient({ params: { path: { id: r.id } } }) : undefined
      }
      canCreate={!!createIngredient}
      canEdit={!!updateIngredient}
      canDelete={!!deleteIngredient}
    />
  );
};
