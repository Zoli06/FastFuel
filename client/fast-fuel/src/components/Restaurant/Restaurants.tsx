import type { components } from '../../types/api';
import { apiClient } from '../../apiClient.ts';
import { EntityManager } from '../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../GenericTable/GenericTable.tsx';
import type { FieldOrFieldset } from '../GenericEditor';
import { LocationPicker } from './LocationPicker.tsx';

export type RestaurantsProps = {
  restaurants: components['schemas']['RestaurantResponseDto'][];
  refetchRestaurants: () => void;
};

type RestaurantDto = components['schemas']['RestaurantResponseDto'];

type RestaurantFormValues = {
  name: string;
  description: string;
  latitude: number;
  longitude: number;
  address: string;
  phone: string;
  openingHours: components['schemas']['RestaurantOpeningHourDto'][];
  password: string;
};

const daysOfWeekOptions: string[] = [
  'Monday',
  'Tuesday',
  'Wednesday',
  'Thursday',
  'Friday',
  'Saturday',
  'Sunday',
];

const defaultOpeningHour: components['schemas']['RestaurantOpeningHourDto'] = {
  dayOfWeek: 'Monday',
  openTime: '09:00',
  closeTime: '17:00',
};

const defaultOpeningHours: components['schemas']['RestaurantOpeningHourDto'][] = [
  { dayOfWeek: 'Monday', openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 'Tuesday', openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 'Wednesday', openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 'Thursday', openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 'Friday', openTime: '09:00', closeTime: '17:00' },
];

const maxLength = 100;
const getDisplayedDescription = (description: string | null) => {
  if (!description) return 'No description provided';
  return description.length > maxLength ? `${description.substring(0, maxLength)}...` : description;
};

const tableColumns: ColumnDefinition<RestaurantDto>[] = [
  { header: 'Name', accessor: 'name' },
  { header: 'Address', accessor: 'address' },
  { header: 'Description', render: (r) => getDisplayedDescription(r.description) },
  { header: 'Phone', accessor: 'phone' },
];

const editorFields: FieldOrFieldset<RestaurantFormValues>[] = [
  { type: 'text', key: 'name', label: 'Name', required: true },
  { type: 'text', key: 'description', label: 'Description', nullable: true },
  { type: 'text', key: 'phone', label: 'Phone', nullable: true },
  {
    type: 'password',
    key: 'password',
    label: 'Password',
    required: (mode) => mode === 'create',
    nullable: (mode) => mode === 'edit',
  },
  {
    type: 'fieldset',
    legend: 'Location',
    fields: [
      {
        type: 'custom',
        key: 'location-picker',
        render: (form) => (
          <LocationPicker
            lat={form.values.latitude}
            lng={form.values.longitude}
            onLocationChange={({ lat, lng, address }) => {
              form.setFieldValue('latitude', lat);
              form.setFieldValue('longitude', lng);
              if (address) form.setFieldValue('address', address);
            }}
          />
        ),
      },
      { type: 'text', key: 'address', label: 'Address', required: true },
    ],
  },
  {
    type: 'fieldset',
    legend: 'Opening Hours',
    fields: [
      {
        type: 'list',
        key: 'openingHours',
        label: 'Opening Hours',
        defaultItem: defaultOpeningHour,
        addButtonLabel: 'Add Opening Hour',
        itemFields: [
          {
            type: 'select',
            key: 'dayOfWeek',
            label: 'Day of Week',
            required: true,
            selectProps: {
              data: daysOfWeekOptions,
              allowDeselect: false,
            },
          },
          { type: 'time', key: 'openTime', label: 'Open Time (HH:mm)', required: true },
          { type: 'time', key: 'closeTime', label: 'Close Time (HH:mm)', required: true },
        ],
      },
    ],
  },
];

const getInitialValues = (restaurant: RestaurantDto | null): RestaurantFormValues => ({
  name: restaurant?.name ?? '',
  description: restaurant?.description ?? '',
  latitude: restaurant?.latitude ?? 0,
  longitude: restaurant?.longitude ?? 0,
  address: restaurant?.address ?? '',
  phone: restaurant?.phone ?? '',
  openingHours: restaurant?.openingHours ?? defaultOpeningHours,
  password: '',
});

export const Restaurants = ({ restaurants, refetchRestaurants }: RestaurantsProps) => {
  const { mutate: createRestaurant } = apiClient.useMutation('post', '/api/Restaurant', {
    onSuccess: () => refetchRestaurants(),
  });
  const { mutate: updateRestaurant } = apiClient.useMutation('put', '/api/Restaurant/{id}', {
    onSuccess: () => refetchRestaurants(),
  });
  const { mutate: deleteRestaurant } = apiClient.useMutation('delete', '/api/Restaurant/{id}', {
    onSuccess: () => refetchRestaurants(),
  });

  const handleSubmit = (
    values: RestaurantFormValues,
    mode: 'create' | 'edit',
    item: RestaurantDto | null,
  ) => {
    const body = values as components['schemas']['RestaurantRequestDto'];
    if (mode === 'create') {
      createRestaurant({ body });
    } else {
      updateRestaurant({ params: { path: { id: item!.id } }, body });
    }
  };

  return (
    <EntityManager<RestaurantDto, RestaurantFormValues>
      title="Restaurants"
      entityName="restaurant"
      data={restaurants}
      columns={tableColumns}
      fields={editorFields}
      getInitialValues={getInitialValues}
      validate={{
        openingHours: (value) => {
          const seenDays = new Set();
          for (const oh of value) {
            if (seenDays.has(oh.dayOfWeek)) return 'Only one entry allowed per day of week';
            seenDays.add(oh.dayOfWeek);
          }
          for (const oh of value) {
            if (oh.openTime >= oh.closeTime) return 'Open time must be before close time';
          }
          return null;
        },
      }}
      onSubmit={handleSubmit}
      onDelete={(r) => deleteRestaurant({ params: { path: { id: r.id } } })}
    />
  );
};
