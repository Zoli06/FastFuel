import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { useSuspensePermissions } from '../../../hooks/useSuspensePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const OrderManager = () => {
  const can = useSuspensePermissions();

  const [
    { data: menus = [] },
    { data: foods = [] },
    { data: restaurants = [] },
    { data: orders = [], refetch: refetchOrders },
  ] = useConditionalSuspenseQueries([
    can.Menu.Read && apiClient.queryOptions('get', '/api/Menu'),
    can.Food.Read && apiClient.queryOptions('get', '/api/Food'),
    can.Restaurant.Read && apiClient.queryOptions('get', '/api/Restaurant'),
    apiClient.queryOptions('get', '/api/Order'),
  ]);

  type Order = (typeof orders)[number];

  const menuNameById = new Map(menus.map((m) => [m.id, m.name]));
  const foodNameById = new Map(foods.map((f) => [f.id, f.name]));
  const restaurantNameById = new Map(restaurants.map((r) => [r.id, r.name]));

  const menuOptions = menus.map((m) => ({ value: m.id, label: m.name }));
  const foodOptions = foods.map((f) => ({ value: f.id, label: f.name }));
  const restaurantOptions = restaurants.map((r) => ({ value: r.id, label: r.name }));

  const tableColumns: ColumnDefinition<Order>[] = [
    { header: 'Order #', accessor: 'orderNumber' },
    ...(can.Restaurant.Read
      ? [
          {
            header: 'Restaurant',
            render: (order: Order) =>
              restaurantNameById.get(order.restaurantId) ?? `#${order.restaurantId}`,
          },
        ]
      : []),
    { header: 'Status', accessor: 'status' },
    { header: 'Price', render: (order) => `${order.price.toFixed(2)}` },
    ...(can.Menu.Read
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
    ...(can.Food.Read
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
    ...(can.Restaurant.Read
      ? [
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
          } satisfies Field,
        ]
      : []),
    ...(can.Menu.Read
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
    ...(can.Food.Read
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

  const { mutate: createOrder } = apiClient.useMutation('post', '/api/Order', {
    onSuccess: () => refetchOrders(),
  });
  const { mutate: updateOrder } = apiClient.useMutation('put', '/api/Order/{id}', {
    onSuccess: () => refetchOrders(),
  });
  const { mutate: deleteOrder } = apiClient.useMutation('delete', '/api/Order/{id}', {
    onSuccess: () => refetchOrders(),
  });

  const handleSubmit = (values: Order, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createOrder({ body: values });
    } else {
      updateOrder({ params: { path: { id: values.id } }, body: values });
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
      onDelete={(o) => deleteOrder({ params: { path: { id: o.id } } })}
      canCreate={can.Order.Create}
      canEdit={can.Order.Update}
      canDelete={can.Order.Delete}
    />
  );
};
