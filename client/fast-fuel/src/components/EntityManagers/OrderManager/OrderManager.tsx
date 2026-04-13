import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { useApi } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const OrderManager = () => {
  const { useNecessaryPerm, useRecommendedPerm, useNoPerm } = useApi('OrderManager');

  const menuReadApi = useRecommendedPerm('Permission:Menu:Read');
  const foodReadApi = useRecommendedPerm('Permission:Food:Read');
  const userReadApi = useRecommendedPerm('Permission:User:Read');

  const [
    { data: menus = [] },
    { data: foods = [] },
    { data: users = [] },
    { data: restaurants = [] },
    { data: orders = [], refetch: refetchOrders },
  ] = useConditionalSuspenseQueries([
    menuReadApi?.queryOptions('get', '/api/Menu'),
    foodReadApi?.queryOptions('get', '/api/Food'),
    userReadApi?.queryOptions('get', '/api/User'),
    useNoPerm().queryOptions('get', '/api/Restaurant'),
    useNecessaryPerm('Permission:Order:Read').queryOptions('get', '/api/Order'),
  ]);

  type Order = (typeof orders)[number];

  const menuNameById = new Map(menus.map((m) => [m.id, m.name]));
  const foodNameById = new Map(foods.map((f) => [f.id, f.name]));
  const userNameById = new Map(users.map((u) => [u.id, u.name]));
  const restaurantNameById = new Map(restaurants.map((r) => [r.id, r.name]));

  const menuOptions = menus.map((m) => ({ value: m.id, label: m.name }));
  const foodOptions = foods.map((f) => ({ value: f.id, label: f.name }));
  const restaurantOptions = restaurants.map((r) => ({ value: r.id, label: r.name }));

  const tableColumns: ColumnDefinition<Order>[] = [
    { header: 'Order #', accessor: 'orderNumber' },
    {
      header: 'Restaurant',
      render: (order: Order) => restaurantNameById.get(order.restaurantId),
    },
    ...(userReadApi
      ? [
          {
            header: 'Ordered By',
            render: (order: Order) => userNameById.get(order.userId) ?? `#${order.userId}`,
          },
        ]
      : []),
    { header: 'Status', accessor: 'status' },
    { header: 'Price', render: (order) => `${order.price.toFixed(2)}` },
    ...(menuReadApi
      ? [
          {
            header: 'Menus',
            render: (order: Order) => {
              if (!order.menus?.length) return 'None';
              return order.menus
                .map((om) => {
                  const name = menuNameById.get(om.menuId) ?? `#${om.menuId}`;
                  return `${name} ×${om.quantity}`;
                })
                .join(', ');
            },
          },
        ]
      : []),
    ...(foodReadApi
      ? [
          {
            header: 'Foods',
            render: (order: Order) => {
              if (!order.foods?.length) return 'None';
              return order.foods
                .map((of) => {
                  const name = foodNameById.get(of.foodId) ?? `#${of.foodId}`;
                  return `${name} ×${of.quantity}`;
                })
                .join(', ');
            },
          },
        ]
      : []),
    {
      header: 'Created At',
      render: (order) => new Date(order.createdAt).toLocaleString(),
    },
  ];

  const editorFields: Field[] = [
    {
      type: 'numericSelect',
      key: 'restaurantId',
      label: 'Restaurant',
      initialValue: 0,
      nullable: 'never',
      required: 'always',
      fieldProps: {
        data: restaurantOptions,
        placeholder: 'Select restaurant',
        searchable: true,
      },
    },
    ...(menuReadApi
      ? [
          {
            type: 'fieldset',
            key: 'menus-fieldset',
            legend: 'Menus',
            initialValue: [],
            nullable: 'never',
            required: 'never',
            label: 'Menus',
            fields: [
              {
                type: 'list',
                key: 'menus',
                label: 'Menus',
                initialValue: [],
                nullable: 'never',
                required: 'never',
                items: [
                  {
                    type: 'numericSelect',
                    key: 'menuId',
                    label: 'Menu',
                    initialValue: 0,
                    nullable: 'never',
                    required: 'always',
                    fieldProps: {
                      data: menuOptions,
                      placeholder: 'Select menu',
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
                  {
                    type: 'text',
                    key: 'specialInstructions',
                    label: 'Special Instructions',
                    initialValue: '',
                    nullable: 'always',
                    required: 'never',
                  },
                ],
              },
            ],
          } satisfies Field,
        ]
      : []),
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
                  {
                    type: 'text',
                    key: 'specialInstructions',
                    label: 'Special Instructions',
                    initialValue: '',
                    nullable: 'always',
                    required: 'never',
                  },
                ],
              },
            ],
          } satisfies Field,
        ]
      : []),
  ];

  const createOrder = useRecommendedPerm('Permission:Order:Create')?.useMutation(
    'post',
    '/api/Order',
    {
      onSuccess: () => refetchOrders(),
    },
  ).mutateAsync;
  const updateOrder = useRecommendedPerm('Permission:Order:Update')?.useMutation(
    'put',
    '/api/Order/{id}',
    {
      onSuccess: () => refetchOrders(),
    },
  ).mutateAsync;
  const deleteOrder = useRecommendedPerm('Permission:Order:Delete')?.useMutation(
    'delete',
    '/api/Order/{id}',
    {
      onSuccess: () => refetchOrders(),
    },
  ).mutate;

  const handleSubmit = async (values: Order, mode: 'create' | 'edit') => {
    if (mode === 'create' && createOrder) {
      await createOrder({ body: values });
    } else if (mode === 'edit' && updateOrder) {
      await updateOrder({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Order>
      title="Orders"
      entityName="order"
      data={orders}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={deleteOrder ? (o) => deleteOrder({ params: { path: { id: o.id } } }) : undefined}
      canCreate={!!createOrder}
      canEdit={!!updateOrder}
      canDelete={!!deleteOrder}
    />
  );
};
