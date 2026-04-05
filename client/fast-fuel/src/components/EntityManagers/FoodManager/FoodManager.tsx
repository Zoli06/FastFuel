import { $api } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const FoodManager = () => {
  const { necessaryPerm, recommendedPerm } = $api('FoodManager');
  const ingredientReadApi = recommendedPerm('Permission:Ingredient:Read');

  const [{ data: ingredients = [] }, { data: foods = [], refetch: refetchFoods }] =
    useConditionalSuspenseQueries([
      ingredientReadApi?.queryOptions('get', '/api/Ingredient'),
      necessaryPerm('Permission:Food:Read').queryOptions('get', '/api/Food'),
    ]);

  type Food = (typeof foods)[number];
  type Ingredient = { id: number; name: string };

  const ingredientNameById = new Map(
    ingredients.map((ingredient: Ingredient) => [ingredient.id, ingredient.name]),
  );
  const ingredientOptions = ingredients.map((ingredient: Ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<Food>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Price', accessor: 'price' },
    { header: 'Description', accessor: 'description' },
    ...(ingredientReadApi
      ? [
          {
            header: 'Ingredients',
            render: (food: Food) => {
              if (!food.ingredients?.length) return 'None';
              return food.ingredients
                .map(
                  (fi: { ingredientId: number }) =>
                    ingredientNameById.get(fi.ingredientId) ?? `#${fi.ingredientId}`,
                )
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
      type: 'number',
      key: 'price',
      label: 'Price',
      initialValue: 0,
      nullable: 'never',
      required: 'always',
    },
    {
      type: 'text',
      key: 'description',
      label: 'Description',
      nullable: 'always',
      required: 'never',
      initialValue: '',
    },
    {
      type: 'text',
      key: 'imageUrl',
      label: 'Image URL',
      nullable: 'always',
      required: 'never',
      initialValue: '',
    },
    ...(ingredientReadApi
      ? [
          {
            type: 'fieldset',
            key: 'ingredients-fieldset',
            legend: 'Ingredients',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            label: 'Ingredients',
            fields: [
              {
                type: 'list',
                key: 'ingredients',
                label: 'Ingredients',
                initialValue: [],
                nullable: 'never',
                required: 'never',
                items: [
                  {
                    type: 'numericSelect',
                    key: 'ingredientId',
                    label: 'Ingredient',
                    initialValue: 0,
                    nullable: 'never',
                    required: 'always',
                    fieldProps: {
                      data: ingredientOptions,
                      placeholder: 'Select ingredient',
                      searchable: true,
                    },
                  },
                  {
                    type: 'number',
                    key: 'quantity',
                    label: 'Quantity',
                    initialValue: 1,
                    nullable: 'never',
                    required: 'always',
                  },
                  {
                    type: 'text',
                    key: 'unit',
                    label: 'Unit',
                    initialValue: 'pcs',
                    nullable: 'never',
                    required: 'always',
                  },
                ],
              },
            ],
          } satisfies Field,
        ]
      : []),
  ];

  const createFood = recommendedPerm('Permission:Food:Create')?.useMutation('post', '/api/Food', {
    onSuccess: () => refetchFoods(),
  }).mutate;
  const updateFood = recommendedPerm('Permission:Food:Update')?.useMutation(
    'put',
    '/api/Food/{id}',
    {
      onSuccess: () => refetchFoods(),
    },
  ).mutate;
  const deleteFood = recommendedPerm('Permission:Food:Delete')?.useMutation(
    'delete',
    '/api/Food/{id}',
    {
      onSuccess: () => refetchFoods(),
    },
  ).mutate;

  const handleSubmit = (values: Food, mode: 'create' | 'edit') => {
    if (mode === 'create' && createFood) {
      createFood({ body: values });
    } else if (mode === 'edit' && updateFood) {
      updateFood({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Food>
      title="Foods"
      entityName="foods"
      data={foods}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={deleteFood ? (r) => deleteFood({ params: { path: { id: r.id } } }) : undefined}
      canCreate={!!createFood}
      canEdit={!!updateFood}
      canDelete={!!deleteFood}
    />
  );
};
