import React from 'react'
import './RestaurantTable.css'
import { RestaurantRow } from './RestaurantRow'
import { Container } from '@mantine/core'

export const RestaurantTable = () => {
  return (
    <Container className='table-container'>
        <RestaurantRow 
          id={1} name="Test Restaurant" address="123 Main St" passwordHash="abc123" openingHours={[]} description="This is a test restaurant with a very long description that should be truncated in the displaydddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd." phone="555-1234">  
        </RestaurantRow>
    </Container>
  )
}
