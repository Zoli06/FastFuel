import { Modal, ScrollArea, Stack, Text, TextInput, UnstyledButton } from '@mantine/core';
import { IconSearch } from '@tabler/icons-react';

type RestaurantOption = {
  id: number;
  name: string;
};

type RestaurantPickerModalProps = {
  opened: boolean;
  restaurantId: number | null;
  searchValue: string;
  restaurants: RestaurantOption[];
  onSearchChange: (value: string) => void;
  onClose: () => void;
  onSelectRestaurant: (restaurantId: number) => void;
};

export const RestaurantPickerModal = ({
  opened,
  restaurantId,
  searchValue,
  restaurants,
  onSearchChange,
  onClose,
  onSelectRestaurant,
}: RestaurantPickerModalProps) => {
  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title="Where are you ordering from?"
      centered
      size="md"
      closeOnClickOutside={restaurantId !== null}
      closeOnEscape={restaurantId !== null}
      withCloseButton={restaurantId !== null}
    >
      <Stack gap="sm">
        <TextInput
          placeholder="Search restaurants..."
          leftSection={<IconSearch size={15} />}
          value={searchValue}
          onChange={(e) => onSearchChange(e.currentTarget.value)}
          autoFocus
        />
        <ScrollArea h={320}>
          <Stack gap={4}>
            {restaurants.length === 0 && (
              <Text c="dimmed" size="sm" ta="center" py="md">
                No restaurants found
              </Text>
            )}
            {restaurants.map((r) => (
              <UnstyledButton
                key={r.id}
                onClick={() => onSelectRestaurant(r.id)}
                style={{
                  padding: '10px 14px',
                  borderRadius: 8,
                  background:
                    restaurantId === r.id
                      ? 'var(--mantine-color-orange-5)'
                      : 'var(--mantine-color-blue-9)',
                  border:
                    restaurantId === r.id
                      ? '2px solid var(--mantine-color-blue-4)'
                      : '2px solid var(--mantine-color-blue-8)',
                  transition: 'all 0.12s',
                }}
              >
                <Text
                  fw={restaurantId === r.id ? 700 : 500}
                  size="sm"
                  c={restaurantId === r.id ? 'blue.2' : 'blue.1'}
                >
                  {r.name}
                </Text>
              </UnstyledButton>
            ))}
          </Stack>
        </ScrollArea>
      </Stack>
    </Modal>
  );
};
