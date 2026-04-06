import { Text } from '@mantine/core';
import { SearchablePickerModal } from './SearchablePickerModal.tsx';

type StationOption = {
  id: number;
  name: string;
};

type StationPickerModalProps = {
  opened: boolean;
  stationId: number | null;
  searchValue: string;
  stations: StationOption[];
  onSearchChange: (value: string) => void;
  onClose: () => void;
  onSelectStation: (stationId: number) => void;
  title?: string;
  emptyMessage?: string;
};

export const StationPickerModal = ({
  opened,
  stationId,
  searchValue,
  stations,
  onSearchChange,
  onClose,
  onSelectStation,
  title = 'Select station',
  emptyMessage = 'No stations found',
}: StationPickerModalProps) => {
  return (
    <SearchablePickerModal
      opened={opened}
      title={title}
      searchValue={searchValue}
      searchPlaceholder="Search stations..."
      onSearchChange={onSearchChange}
      onClose={onClose}
      items={stations}
      getItemKey={(station) => station.id}
      isItemSelected={(station) => stationId === station.id}
      onSelectItem={(station) => onSelectStation(station.id)}
      renderItem={(station, selected) => (
        <Text fw={selected ? 700 : 500} size="sm" c={selected ? 'blue.2' : 'blue.1'}>
          {station.name}
        </Text>
      )}
      emptyMessage={emptyMessage}
    />
  );
};
