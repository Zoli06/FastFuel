import { Button, Table } from '@mantine/core';
import type { components } from '../../../types/api';
import { apiClient } from '../../../apiClient.ts';

export type RestaurantRowProps = {
  restaurant: components['schemas']['RestaurantResponseDto'];
  openRestaurantEditor: () => void;
  refetchRestaurants: () => void;
};

const maxLength = 100;
const getDisplayedDescription = (description: string | null) => {
  if (!description) {
    return 'No description provided';
  }
  return description.length > maxLength ? `${description.substring(0, maxLength)}...` : description;
};

export const RestaurantRow = ({
  restaurant,
  openRestaurantEditor,
  refetchRestaurants,
}: RestaurantRowProps) => {
  const { mutate: deleteRestaurant } = apiClient.useMutation('delete', '/api/Restaurant/{id}', {
    onSuccess: () => {
      refetchRestaurants();
    },
  });

  return (
    <Table.Tr>
      <Table.Td>{restaurant.name}</Table.Td>
      <Table.Td>{restaurant.address} </Table.Td>
      <Table.Td>{getDisplayedDescription(restaurant.description)} </Table.Td>
      <Table.Td>{restaurant.phone} </Table.Td>
      <Table.Td>
        <Button onClick={openRestaurantEditor}>Edit</Button>
      </Table.Td>
      <Table.Td>
        <Button
          onClick={() => deleteRestaurant({ params: { path: { id: restaurant.id } } })}
          color="red"
        >
          Delete
        </Button>
      </Table.Td>
    </Table.Tr>
  );
};
