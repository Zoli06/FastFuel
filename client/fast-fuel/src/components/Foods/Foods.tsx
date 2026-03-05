import type { components } from '../../types/api';
import { apiClient } from '../../apiClient.ts';
import { EntityManager } from '../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../GenericTable/GenericTable.tsx';
import type { FieldOrFieldset } from '../GenericEditor';

export type FoodProps = {
  foods: components['schemas']['FoodResponseDto'][];
  ingredients: components['schemas']['IngredientResponseDto'][];
  refetchFoods: () => void;
};

type FoodDto = components['schemas']['FoodResponseDto'];

type FoodIngredientFormValues = {
  ingredientId: number;
  quantity: number;
  unit: string;
};

type FoodFormValues = {
  name: string;
  price: number;
  description: string;
  imageUrl: string;
  ingredients: FoodIngredientFormValues[];
  id: number;
};

export const Foods = ({ foods, ingredients, refetchFoods }: FoodProps) => {
  const ingredientNameById = new Map(
    (ingredients ?? []).map((ingredient) => [ingredient.id, ingredient.name]),
  );
  const ingredientOptions = (ingredients ?? []).map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const editorFields: FieldOrFieldset<FoodFormValues>[] = [
    { type: 'text', key: 'name', label: 'Name', required: true, initialValue: '' },
    { type: 'number', key: 'price', label: 'Price', required: true, initialValue: 0 },
    { type: 'text', key: 'description', label: 'Description', nullable: true, initialValue: '' },
    { type: 'text', key: 'imageUrl', label: 'Image URL', nullable: true, initialValue: '' },
    {
      type: 'fieldset',
      legend: 'Ingredients',
      fields: [
        {
          type: 'list',
          key: 'ingredients',
          label: 'Ingredients',
          addButtonLabel: 'Add Ingredient',
          initialValue: [],
          itemFields: [
            {
              type: 'numericSelect',
              key: 'ingredientId',
              label: 'Ingredient',
              required: true,
              initialValue: 0,
              selectProps: {
                data: ingredientOptions,
                placeholder: 'Select ingredient',
                searchable: true,
              },
            },
            { type: 'number', key: 'quantity', label: 'Quantity', required: true, initialValue: 1 },
            { type: 'text', key: 'unit', label: 'Unit', required: true, initialValue: 'pcs' },
          ],
        },
      ],
    },
  ];

  const tableColumns: ColumnDefinition<FoodDto>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Price', accessor: 'price' },
    { header: 'Description', accessor: 'description' },
    {
      header: 'Ingredients',
      render: (food) => {
        if (!food.ingredients?.length) return 'None';
        return food.ingredients
          .map((fi) => ingredientNameById.get(fi.ingredientId) ?? `#${fi.ingredientId}`)
          .join(', ');
      },
    },
    { header: 'Menus', accessor: 'menuIds' },
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

  const handleSubmit = (values: FoodFormValues, mode: 'create' | 'edit', item: FoodDto | null) => {
    const body = values;
    if (mode === 'create') {
      createFood({ body });
    } else {
      updateFood({ params: { path: { id: item!.id } }, body });
    }
  };

  return (
    <EntityManager<FoodDto, FoodFormValues>
      title="Foods"
      entityName="foods"
      data={foods}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(r) => deleteFood({ params: { path: { id: r.id } } })}
    />
  );
};
