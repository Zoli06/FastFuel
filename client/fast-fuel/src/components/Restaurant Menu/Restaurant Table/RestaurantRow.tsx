import React from 'react'
import './RestaurantRow.css'
import { Button, Container, Flex } from '@mantine/core';
  
interface RestaurantRowProps {
  id: number;
  name: string;
  address: string;
  passwordHash: string;
  openingHours: any[]; 
  description?: string;
  phone: string;
}

export const RestaurantRow = ({ id, name, address, passwordHash, openingHours, phone, description }: RestaurantRowProps) => {
  const maxLength = 100;
  const isTooLong = description && description.length > maxLength;

  const displayText = isTooLong 
    ? `${description.substring(0, maxLength)}...` 
    : description || "No description provided"; 

  return (
    <Container className='row-contianer'> 
      <Flex className='left-side'>
        <span>{id} </span>
        <span>{name} </span>
        <span>{address} </span>
        <span>{passwordHash} </span>
        <span>{JSON.stringify(openingHours)} </span>
        <span>{phone} </span>
      </Flex>

      <Flex className='right-side'>
        <p>
          {displayText}
        </p>

        {isTooLong && (
          <Button variant="transparent" color="gray" size="xs" className='expand-button'>↓</Button>
        )}
      </Flex>
    </Container>
  );
};