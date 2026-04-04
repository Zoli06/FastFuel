import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import { Image } from '@mantine/core';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { $api } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const MenuManager = () => {
  const { necessaryPerm, recommendedPerm } = $api('MenuManager');
  const foodReadApi = recommendedPerm('Permission:Food:Read');

  const [{ data: foods = [] }, { data: menus = [], refetch: refetchMenus }] =
    useConditionalSuspenseQueries([
      foodReadApi?.queryOptions('get', '/api/Food'),
      necessaryPerm('Permission:Menu:Read').queryOptions('get', '/api/Menu'),
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
    { header: 'Description', accessor: 'description' },
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
                  },
                ],
              },
            ],
          } satisfies Field,
        ]
      : []),
  ];

  const createMenu = recommendedPerm('Permission:Menu:Create')?.useMutation('post', '/api/Menu', {
    onSuccess: () => refetchMenus(),
  }).mutate;
  const updateMenu = recommendedPerm('Permission:Menu:Update')?.useMutation(
    'put',
    '/api/Menu/{id}',
    {
      onSuccess: () => refetchMenus(),
    },
  ).mutate;
  const deleteMenu = recommendedPerm('Permission:Menu:Delete')?.useMutation(
    'delete',
    '/api/Menu/{id}',
    {
      onSuccess: () => refetchMenus(),
    },
  ).mutate;

  const handleSubmit = (values: Menu, mode: 'create' | 'edit') => {
    if (mode === 'create' && createMenu) {
      createMenu({ body: values });
    } else if (mode === 'edit' && updateMenu) {
      updateMenu({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Menu>
      title="Menus"
      entityName="menus"
      data={menus}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={deleteMenu ? (r) => deleteMenu({ params: { path: { id: r.id } } }) : undefined}
      canCreate={!!createMenu}
      canEdit={!!updateMenu}
      canDelete={!!deleteMenu}
    />
  );
};
