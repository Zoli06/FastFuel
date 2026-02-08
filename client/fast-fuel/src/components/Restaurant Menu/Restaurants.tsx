import './Restaurants.css';
import { HeaderGeneral } from '../Headers/HeaderGeneral';
import { Footer } from '../Footer/Footer';
import { Button, Container, Flex } from '@mantine/core';
import { RestaurantTable } from './Restaurant Table/RestaurantTable';

export const Restaurant = () => {
  return (
    <>
      <HeaderGeneral title="Restaurants" />

      <Container className="restaurant">
        <Flex className="restaurant-header">
          <h2>Restaurants</h2>
          <Button className="add-restaurant-button">Add Restaurant</Button>
        </Flex>
        <Container className="restaurant-list">
          <RestaurantTable />
        </Container>
      </Container>

      <Footer />
    </>
  );
};
