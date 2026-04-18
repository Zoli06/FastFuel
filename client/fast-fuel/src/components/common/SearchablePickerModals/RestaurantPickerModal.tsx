import { Text } from '@mantine/core';
import { SearchablePickerModal } from './SearchablePickerModal.tsx';

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
  title?: string;
  emptyMessage?: string;
};

export const RestaurantPickerModal = ({
  opened,
  restaurantId,
  searchValue,
  restaurants,
  onSearchChange,
  onClose,
  onSelectRestaurant,
  title = 'Where are you ordering from?',
  emptyMessage = 'No restaurants found',
}: RestaurantPickerModalProps) => {
  return (
    <SearchablePickerModal
      opened={opened}
      title={title}
      hasSelection={restaurantId !== null}
      searchValue={searchValue}
      searchPlaceholder="Search restaurants..."
      onSearchChange={onSearchChange}
      onClose={onClose}
      items={restaurants}
      getItemKey={(r) => r.id}
      isItemSelected={(r) => restaurantId === r.id}
      onSelectItem={(r) => onSelectRestaurant(r.id)}
      renderItem={(r, selected) => (
        <Text fw={selected ? 700 : 500} size="sm" c={selected ? '#f8f0e6' : undefined}>
          {r.name}
        </Text>
      )}
      emptyMessage={emptyMessage}
    />
  );
};
