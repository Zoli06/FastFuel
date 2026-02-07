import React from 'react'
import './HeaderGeneral.css'
import { Button, Container, Flex } from '@mantine/core';

interface HeaderProps {
  title: string;
}

export const HeaderGeneral: React.FC<HeaderProps> = ({ title }) => {
  return (
    <>
    <Flex className='header-flex'>
      <Button variant="filled" className='home-button'>Home</Button>
        <h1 className='general-header'>{title}</h1>
        <Button variant="filled" className='login-button'>Login</Button>
    </Flex>
    </>
  )
}