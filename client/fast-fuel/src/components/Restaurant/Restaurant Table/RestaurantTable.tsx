import { RestaurantRow } from './RestaurantRow';
import { Table } from '@mantine/core';
import type { components } from '../../../types/api';

export type RestaurantTableProps = {
  restaurants: components['schemas']['RestaurantResponseDto'][];
  refetchRestaurants: () => void;
  openRestaurantEditor: (restaurant: components['schemas']['RestaurantResponseDto']) => void;
};

export const RestaurantTable = ({
  restaurants,
  refetchRestaurants,
  openRestaurantEditor,
}: RestaurantTableProps) => {
  return (
    <Table>
      <Table.Thead>
        <Table.Tr>
          <Table.Th>Name</Table.Th>
          <Table.Th>Address</Table.Th>
          <Table.Th>Description</Table.Th>
          <Table.Th>Phone</Table.Th>
          <Table.Th>Edit</Table.Th>
          <Table.Th>Delete</Table.Th>
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>
        {restaurants.map((restaurant) => (
          <RestaurantRow
            key={restaurant.id}
            restaurant={restaurant}
            openRestaurantEditor={() => openRestaurantEditor(restaurant)}
            refetchRestaurants={refetchRestaurants}
          />
        ))}
      </Table.Tbody>
    </Table>
  );
};
