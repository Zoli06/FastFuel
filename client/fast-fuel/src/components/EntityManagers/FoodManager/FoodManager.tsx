import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { useSuspensePermissions } from '../../../hooks/useSuspensePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const FoodManager = () => {
  const can = useSuspensePermissions();

  const [{ data: ingredients = [] }, { data: foods = [], refetch: refetchFoods }] =
    useConditionalSuspenseQueries([
      can.Ingredient.Read && apiClient.queryOptions('get', '/api/Ingredient'),
      apiClient.queryOptions('get', '/api/Food'),
    ]);

  type Food = (typeof foods)[number];

  const ingredientNameById = new Map(
    ingredients.map((ingredient) => [ingredient.id, ingredient.name]),
  );
  const ingredientOptions = ingredients.map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<Food>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Price', accessor: 'price' },
    { header: 'Description', accessor: 'description' },
    ...(can.Ingredient.Read
      ? [
          {
            header: 'Ingredients',
            render: (food: Food) => {
              if (!food.ingredients?.length) return 'None';
              return food.ingredients
                .map((fi) => ingredientNameById.get(fi.ingredientId) ?? `#${fi.ingredientId}`)
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
    ...(can.Ingredient.Read
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

  const { mutate: createFood } = apiClient.useMutation('post', '/api/Food', {
    onSuccess: () => refetchFoods(),
  });
  const { mutate: updateFood } = apiClient.useMutation('put', '/api/Food/{id}', {
    onSuccess: () => refetchFoods(),
  });
  const { mutate: deleteFood } = apiClient.useMutation('delete', '/api/Food/{id}', {
    onSuccess: () => refetchFoods(),
  });

  const handleSubmit = (values: Food, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createFood({ body: values });
    } else {
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
      onDelete={(r) => deleteFood({ params: { path: { id: r.id } } })}
      canCreate={can.Food.Create}
      canEdit={can.Food.Update}
      canDelete={can.Food.Delete}
    />
  );
};
