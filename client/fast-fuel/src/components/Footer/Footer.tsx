import { Box, Center, Text } from '@mantine/core';

export const Footer = () => {
  return (
    <Box h={60} pos="fixed" bottom={0} left={0} right={0} bg="darkred" c="#f8f0e6">
      <Center h="100%">
        <Text>© {new Date().getFullYear()} Fast Fuel. All rights reserved.</Text>
      </Center>
    </Box>
  );
};
