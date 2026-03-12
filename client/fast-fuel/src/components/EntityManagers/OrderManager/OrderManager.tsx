import type { components } from '../../../types/api';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';

type Order = components['schemas']['OrderResponseDto'];
type Menu = components['schemas']['MenuResponseDto'];
type Food = components['schemas']['FoodResponseDto'];
type Restaurant = components['schemas']['RestaurantResponseDto'];
type OrderStatus = components['schemas']['OrderStatus'];

const ORDER_STATUS_OPTIONS: OrderStatus[] = ['Pending', 'InProgress', 'Completed', 'Cancelled'];

export type OrderManagerProps = {
  orders: Order[];
  refetchOrders: () => void;
  menus: Menu[];
  foods: Food[];
  restaurants: Restaurant[];
};

export const OrderManager = ({
  orders,
  refetchOrders,
  menus,
  foods,
  restaurants,
}: OrderManagerProps) => {
  const menuNameById = new Map((menus ?? []).map((m) => [m.id, m.name]));
  const foodNameById = new Map((foods ?? []).map((f) => [f.id, f.name]));
  const restaurantNameById = new Map((restaurants ?? []).map((r) => [r.id, r.name]));

  const menuOptions = (menus ?? []).map((m) => ({ value: m.id, label: m.name }));
  const foodOptions = (foods ?? []).map((f) => ({ value: f.id, label: f.name }));
  const restaurantOptions = (restaurants ?? []).map((r) => ({ value: r.id, label: r.name }));

  const tableColumns: ColumnDefinition<Order>[] = [
    { header: 'Order #', accessor: 'orderNumber' },
    {
      header: 'Restaurant',
      render: (order) => restaurantNameById.get(order.restaurantId) ?? `#${order.restaurantId}`,
    },
    { header: 'Status', accessor: 'status' },
    { header: 'Price', render: (order) => `${order.price.toFixed(2)}` },
    {
      header: 'Menus',
      render: (order) => {
        if (!order.menus?.length) return 'None';
        return order.menus
          .map((om) => {
            const name = menuNameById.get(om.menuId) ?? `#${om.menuId}`;
            return `${name} ×${om.quantity}`;
          })
          .join(', ');
      },
    },
    {
      header: 'Foods',
      render: (order) => {
        if (!order.foods?.length) return 'None';
        return order.foods
          .map((of) => {
            const name = foodNameById.get(of.foodId) ?? `#${of.foodId}`;
            return `${name} ×${of.quantity}`;
          })
          .join(', ');
      },
    },
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
      selectProps: {
        data: restaurantOptions,
        placeholder: 'Select restaurant',
        searchable: true,
      },
    },
    {
      type: 'select',
      key: 'status',
      label: 'Status',
      initialValue: 'Pending',
      nullable: 'never',
      required: 'always',
      selectProps: {
        data: ORDER_STATUS_OPTIONS,
        allowDeselect: false,
      },
    },
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
              selectProps: {
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
    },
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
              selectProps: {
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
    },
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
    />
  );
};
