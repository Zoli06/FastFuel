import type { components } from '../../../types/api';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field, FormValues } from '../../EntityManager/EntityEditor';
import { LocationPicker } from './LocationPicker.tsx';
import type { UseFormReturnType } from '@mantine/form';

type Restaurant = components['schemas']['RestaurantResponseDto'];

export type RestaurantManagerProps = {
  restaurants: Restaurant[];
  refetchRestaurants: () => void;
};

// TODO: Remove this or at least extract to a helper
const maxLength = 100;
const getDisplayedDescription = (description: string | null) => {
  if (!description) return 'No description provided';
  return description.length > maxLength ? `${description.substring(0, maxLength)}...` : description;
};

const tableColumns: ColumnDefinition<Restaurant>[] = [
  { header: 'Name', accessor: 'name' },
  { header: 'Address', accessor: 'address' },
  { header: 'Description', render: (r) => getDisplayedDescription(r.description) },
  { header: 'Phone', accessor: 'phone' },
];

const dayOfWeekOptions: Restaurant['openingHours'][0]['dayOfWeek'][] = [
  'Monday',
  'Tuesday',
  'Wednesday',
  'Thursday',
  'Friday',
  'Saturday',
  'Sunday',
];

const defaultOpeningHours: Restaurant['openingHours'] = [
  { dayOfWeek: 'Monday', openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 'Tuesday', openTime: '09:00', closeTime: '17:00' },
];

const editorFields: Field[] = [
  {
    type: 'text',
    key: 'name',
    label: 'Name',
    initialValue: '',
    nullable: 'never',
    required: 'always',
  },
  {
    type: 'text',
    key: 'description',
    label: 'Description',
    nullable: 'always',
    required: 'never',
    initialValue: '',
  },
  {
    type: 'text',
    key: 'phone',
    label: 'Phone',
    nullable: 'always',
    required: 'never',
    initialValue: '',
  },
  {
    type: 'fieldset',
    key: 'location-fieldset',
    legend: 'Location',
    label: 'Location',
    initialValue: [],
    nullable: 'never',
    required: 'never',
    fields: [
      {
        type: 'custom',
        render: (form: UseFormReturnType<FormValues>) => (
          <LocationPicker
            lat={(form.values.latitude as number) || 47}
            lng={(form.values.longitude as number) || 19}
            onLocationChange={({ lat, lng, address }) => {
              form.setFieldValue('latitude', lat);
              form.setFieldValue('longitude', lng);
              if (address) form.setFieldValue('address', address);
            }}
          />
        ),
      },
      {
        type: 'text',
        key: 'address',
        label: 'Address',
        initialValue: '',
        nullable: 'never',
        required: 'always',
      },
    ],
  },
  {
    type: 'fieldset',
    key: 'opening-hours-fieldset',
    legend: 'Opening Hours',
    label: 'Opening Hours',
    initialValue: [],
    nullable: 'never',
    required: 'never',
    fields: [
      {
        type: 'list',
        key: 'openingHours',
        label: 'Opening Hours',
        initialValue: defaultOpeningHours,
        nullable: 'never',
        required: 'never',
        items: [
          {
            type: 'select',
            key: 'dayOfWeek',
            label: 'Day of Week',
            initialValue: 'Monday',
            nullable: 'never',
            required: 'always',
            selectProps: {
              data: dayOfWeekOptions,
              allowDeselect: false,
            },
          },
          {
            type: 'time',
            key: 'openTime',
            label: 'Open Time (HH:mm)',
            initialValue: '09:00',
            nullable: 'never',
            required: 'always',
          },
          {
            type: 'time',
            key: 'closeTime',
            label: 'Close Time (HH:mm)',
            initialValue: '17:00',
            nullable: 'never',
            required: 'always',
          },
        ],
      },
    ],
  },
];

export const RestaurantManager = ({ restaurants, refetchRestaurants }: RestaurantManagerProps) => {
  const { mutate: createRestaurant } = apiClient.useMutation('post', '/api/Restaurant', {
    onSuccess: () => refetchRestaurants(),
  });
  const { mutate: updateRestaurant } = apiClient.useMutation('put', '/api/Restaurant/{id}', {
    onSuccess: () => refetchRestaurants(),
  });
  const { mutate: deleteRestaurant } = apiClient.useMutation('delete', '/api/Restaurant/{id}', {
    onSuccess: () => refetchRestaurants(),
  });

  const handleSubmit = (values: Restaurant, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createRestaurant({ body: values });
    } else {
      updateRestaurant({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Restaurant>
      title="Restaurants"
      entityName="restaurant"
      data={restaurants}
      tableColumns={tableColumns}
      editorFields={editorFields}
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
