import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import { Image } from '@mantine/core';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { useApi } from '../../../lib/api.ts';
import { getDisplayedDescription } from '../../../lib/description.ts';
import { validateOptionalImageUrl } from '../../../lib/url-validation.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const MenuManager = () => {
  const { useNecessaryPerm, useRecommendedPerm } = useApi('MenuManager');
  const foodReadApi = useRecommendedPerm('Permission:Food:Read');

  const [{ data: foods = [] }, { data: menus = [], refetch: refetchMenus }] =
    useConditionalSuspenseQueries([
      foodReadApi?.queryOptions('get', '/api/Food'),
      useNecessaryPerm('Permission:Menu:Read').queryOptions('get', '/api/Menu'),
    ]);

  type Menu = (typeof menus)[number];

  const foodNameById = new Map(foods.map((food) => [food.id, food.name]));
  const foodOptions = foods.map((food) => ({
    value: food.id,
    label: food.name,
  }));

  const tableColumns: ColumnDefinition<Menu>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'Price', accessor: 'price' },
    { header: 'Description', render: (menu) => getDisplayedDescription(menu.description) },
    {
      header: 'Image',
      render: (menu) =>
        menu.imageUrl ? (
          <Image src={menu.imageUrl} alt={menu.name} width={50} height={50} />
        ) : (
          'No image'
        ),
    },
    ...(foodReadApi
      ? [
          {
            header: 'Foods',
            render: (menu: Menu) => {
              if (!menu.foods?.length) return 'None';
              return menu.foods
                .map((mf) => {
                  const name = foodNameById.get(mf.foodId) ?? `#${mf.foodId}`;
                  return `${name} ${mf.quantity}`;
                })
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
      fieldProps: {
        min: 0,
        step: 0.01,
      },
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
    ...(foodReadApi
      ? [
          {
            type: 'fieldset',
            key: 'foods-fieldset',
            legend: 'Foods',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            label: 'Foods',
            fields: [
              {
                type: 'list',
                key: 'foods',
                label: 'Foods',
                initialValue: [],
                nullable: 'never',
                required: 'never',
                items: [
                  {
                    type: 'numericSelect',
                    key: 'foodId',
                    label: 'Food',
                    initialValue: 0,
                    nullable: 'never',
                    required: 'always',
                    fieldProps: {
                      data: foodOptions,
                      placeholder: 'Select food',
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
                    fieldProps: {
                      min: 1,
                      step: 1,
                    },
                  },
                ],
              },
            ],
          } satisfies Field,
        ]
      : []),
  ];

  const createMenu = useRecommendedPerm('Permission:Menu:Create')?.useMutation(
    'post',
    '/api/Menu',
    {
      onSuccess: () => refetchMenus(),
    },
  ).mutateAsync;
  const updateMenu = useRecommendedPerm('Permission:Menu:Update')?.useMutation(
    'put',
    '/api/Menu/{id}',
    {
      onSuccess: () => refetchMenus(),
    },
  ).mutateAsync;
  const deleteMenu = useRecommendedPerm('Permission:Menu:Delete')?.useMutation(
    'delete',
    '/api/Menu/{id}',
    {
      onSuccess: () => refetchMenus(),
    },
  ).mutate;

  const handleSubmit = async (values: Menu, mode: 'create' | 'edit') => {
    if (mode === 'create' && createMenu) {
      await createMenu({ body: values });
    } else if (mode === 'edit' && updateMenu) {
      await updateMenu({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Menu>
      title="Menus"
      entityName="menus"
      data={menus}
      tableColumns={tableColumns}
      editorFields={editorFields}
      validate={{
        imageUrl: validateOptionalImageUrl,
      }}
      onSubmit={handleSubmit}
      onDelete={deleteMenu ? (r) => deleteMenu({ params: { path: { id: r.id } } }) : undefined}
      canCreate={!!createMenu}
      canEdit={!!updateMenu}
      canDelete={!!deleteMenu}
    />
  );
};
