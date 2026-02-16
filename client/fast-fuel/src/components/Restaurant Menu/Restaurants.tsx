import './Restaurants.css';
import { Button, Container, Flex } from '@mantine/core';
import { RestaurantTable } from './Restaurant Table/RestaurantTable';
import type { components } from '../../types/api';
import { useState } from 'react';
import { RestaurantEditor } from './Restaurant Table/RestaurantEditor.tsx';

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
      <Container className="restaurant">
        <Flex className="restaurant-header">
          <h2>Restaurants</h2>
          <Button className="add-restaurant-button" onClick={openRestaurantEditorCreate}>
            Add Restaurant
          </Button>
        </Flex>
        <Container className="restaurant-list">
          <RestaurantTable
            restaurants={restaurants}
            openRestaurantEditor={openRestaurantEditorEdit}
            refetchRestaurants={refetchRestaurants}
          />
        </Container>
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
