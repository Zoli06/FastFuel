import { Anchor, Box, Center, Group, Text } from '@mantine/core';
import { Link } from 'react-router-dom';

export const Footer = () => {
  return (
    <Box h={60} pos="fixed" bottom={0} left={0} right={0} bg="darkred" c="#f8f0e6">
      <Center h="100%">
        <Group gap="md">
          <Text>(c) {new Date().getFullYear()} Fast Fuel. All rights reserved.</Text>
          <Anchor component={Link} to="/about" c="#f8f0e6" td="underline">
            About
          </Anchor>
        </Group>
      </Center>
    </Box>
  );
};
