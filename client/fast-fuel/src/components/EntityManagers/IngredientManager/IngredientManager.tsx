import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import { Image } from '@mantine/core';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const IngredientManager = () => {
  const { necessary, recommended } = usePagePermissions('IngredientManager', { split: true });

  const [
    { data: allergies = [] },
    { data: stationCategories = [] },
    { data: ingredients = [], refetch: refetchIngredients },
  ] = useConditionalSuspenseQueries([
    recommended.Allergy.Read && apiClient.queryOptions('get', '/api/Allergy'),
    recommended.StationCategory.Read && apiClient.queryOptions('get', '/api/StationCategory'),
    necessary.Ingredient.Read && apiClient.queryOptions('get', '/api/Ingredient'),
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
    ...(recommended.Allergy.Read
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
    ...(recommended.StationCategory.Read
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
    ...(recommended.Allergy.Read
      ? [
          {
            type: 'numericMultiSelect',
            key: 'allergyIds',
            label: 'Allergies',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            fieldProps: {
              data: allergies.map((allergy) => ({ value: allergy.id, label: allergy.name })),
              placeholder: 'Select allergies',
              searchable: true,
            },
          } satisfies Field,
        ]
      : []),
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
              data: stationCategories.map((sc) => ({ value: sc.id, label: sc.name })),
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
      canCreate={recommended.Ingredient.Create}
      canEdit={recommended.Ingredient.Update}
      canDelete={recommended.Ingredient.Delete}
    />
  );
};
