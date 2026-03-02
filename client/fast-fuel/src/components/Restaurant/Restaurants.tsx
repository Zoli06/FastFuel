import { Box, Button, Container, Flex, Title } from '@mantine/core';
import { RestaurantTable } from './Restaurant Table/RestaurantTable';
import type { components } from '../../types/api';
import { useState } from 'react';
import { RestaurantEditor } from './Restaurant Table/RestaurantEditor';

export type RestaurantsProps = {
  restaurants: components['schemas']['RestaurantResponseDto'][];
  refetchRestaurants: () => void;
};

export const Restaurants = ({ restaurants, refetchRestaurants }: RestaurantsProps) => {
  const [isEditorOpen, setIsEditorOpen] = useState(false);
  const [editingRestaurant, setEditingRestaurant] = useState<
    components['schemas']['RestaurantResponseDto'] | null
  >(null);
  const [isAdding, setIsAdding] = useState(false);

  const openRestaurantEditorCreate = () => {
    setEditingRestaurant(null);
    setIsAdding(true);
    setIsEditorOpen(true);
  };

  const openRestaurantEditorEdit = (restaurant: components['schemas']['RestaurantResponseDto']) => {
    setEditingRestaurant(restaurant);
    setIsAdding(false);
    setIsEditorOpen(true);
  };

  const closeRestaurantEditor = () => {
    setEditingRestaurant(null);
    setIsAdding(false);
    setIsEditorOpen(false);
  };

  return (
    <>
      <Container mt={20} mx="auto" bg="beige" p={15} bd="1px solid black">
        {/* Header */}
        <Flex
          justify="space-between"
          align="center"
          w="100%"
          pb="xs"
          style={{ borderBottom: '1px solid black' }}
        >
          <Title order={2}>Restaurants</Title>

          <Button onClick={openRestaurantEditorCreate} bg="gray" c="black" bd="1px solid black">
            Add Restaurant
          </Button>
        </Flex>

        {/* List */}
        <Box
          style={{
            flex: 1,
            overflowY: 'auto',
          }}
          px={20}
          py={10}
        >
          <RestaurantTable
            restaurants={restaurants}
            openRestaurantEditor={openRestaurantEditorEdit}
            refetchRestaurants={refetchRestaurants}
          />
        </Box>
      </Container>

      <RestaurantEditor
        mode={isAdding ? 'create' : 'edit'}
        restaurant={editingRestaurant}
        opened={isEditorOpen}
        refetchRestaurants={refetchRestaurants}
        onClose={closeRestaurantEditor}
      />
    </>
  );
};
