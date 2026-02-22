import type { components } from '../../../types/api';
import { Button, Divider, Fieldset, Group, Modal, PasswordInput, TextInput } from '@mantine/core';
import { Form, useForm } from '@mantine/form';
import { apiClient } from '../../../apiClient.ts';
import { useEffect } from 'react';
import { TimePicker } from '@mantine/dates';
import { NumericSelect } from '../../NumericSelect/NumericSelect.tsx';
import { LocationPicker } from './LocationPicker';

type RestaurantEditorPropsCreate = {
  mode: 'create';
};

type RestaurantEditorPropsEdit = {
  mode: 'edit';
  restaurant: components['schemas']['RestaurantResponseDto'] | null;
};

export type RestaurantEditorProps = (RestaurantEditorPropsCreate | RestaurantEditorPropsEdit) & {
  opened: boolean;
  refetchRestaurants: () => void;
  onClose: () => void;
};

const daysOfWeekOptions: { value: components['schemas']['DayOfWeek']; label: string }[] = [
  { value: 0, label: 'Monday' },
  { value: 1, label: 'Tuesday' },
  { value: 2, label: 'Wednesday' },
  { value: 3, label: 'Thursday' },
  { value: 4, label: 'Friday' },
  { value: 5, label: 'Saturday' },
  { value: 6, label: 'Sunday' },
];

const defaultOpeningHours: components['schemas']['RestaurantResponseDto']['openingHours'] = [
  { dayOfWeek: 0, openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 1, openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 2, openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 3, openTime: '09:00', closeTime: '17:00' },
  { dayOfWeek: 4, openTime: '09:00', closeTime: '17:00' },
];

const getDefaultRestaurantValues = (
  restaurant: components['schemas']['RestaurantResponseDto'] | null,
) => {
  return {
    name: restaurant?.name || '',
    description: restaurant?.description || '',
    latitude: restaurant?.latitude || 0,
    longitude: restaurant?.longitude || 0,
    address: restaurant?.address || '',
    phone: restaurant?.phone || '',
    openingHours: restaurant?.openingHours || defaultOpeningHours,
    password: '',
  };
};

const convertEmptyNonRequiredFieldsToNull = (
  values: components['schemas']['RestaurantRequestDto'],
) => {
  return {
    ...values,
    description: values.description || null,
    phone: values.phone || null,
    password: values.password || null,
  };
};

const validateOneEntryPerDayOfWeek = (
  openingHours: components['schemas']['RestaurantRequestDto']['openingHours'],
) => {
  const seenDays = new Set();
  for (const oh of openingHours) {
    if (seenDays.has(oh.dayOfWeek)) {
      return 'Only one entry allowed per day of week';
    }
    seenDays.add(oh.dayOfWeek);
  }
  return null;
};

const validateOpenTimeBeforeCloseTime = (
  openingHours: components['schemas']['RestaurantRequestDto']['openingHours'],
) => {
  for (const oh of openingHours) {
    if (oh.openTime >= oh.closeTime) {
      return 'Open time must be before close time';
    }
  }
  return null;
};

export const RestaurantEditor = (props: RestaurantEditorProps) => {
  const { mode, opened, onClose, refetchRestaurants } = props;
  const restaurant = 'restaurant' in props ? props.restaurant : null;

  const form = useForm({
    initialValues: getDefaultRestaurantValues(restaurant),
    validate: {
      openingHours: (value) => {
        const oneEntryPerDayError = validateOneEntryPerDayOfWeek(value);
        if (oneEntryPerDayError) return oneEntryPerDayError;

        const openTimeBeforeCloseTimeError = validateOpenTimeBeforeCloseTime(value);
        if (openTimeBeforeCloseTimeError) return openTimeBeforeCloseTimeError;

        return null;
      },
    },
  });

  const { mutate: createRestaurant } = apiClient.useMutation('post', '/api/Restaurant', {
    onSuccess: () => {
      refetchRestaurants();
      onClose();
    },
  });
  const { mutate: updateRestaurant } = apiClient.useMutation('put', `/api/Restaurant/{id}`, {
    onSuccess: () => {
      refetchRestaurants();
      onClose();
    },
  });

  useEffect(() => {
    if (!opened) return;

    if (mode === 'edit' && restaurant) {
      form.setValues(getDefaultRestaurantValues(restaurant));
    } else if (mode === 'create') {
      form.reset();
    }

    // I left out `form` to avoid infinite loop
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [mode, restaurant, opened]);

  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title={mode === 'create' ? 'Create Restaurant' : 'Edit Restaurant'}
    >
      <Form
        form={form}
        onSubmit={(values) => {
          const finalValues: components['schemas']['RestaurantRequestDto'] =
            convertEmptyNonRequiredFieldsToNull(values);

          if (mode === 'create') {
            createRestaurant({
              body: finalValues,
            });
          } else {
            updateRestaurant({
              params: { path: { id: restaurant!.id } },
              body: finalValues,
            });
          }
        }}
      >
        <TextInput label="Name" {...form.getInputProps('name')} required />
        <TextInput label="Description" {...form.getInputProps('description')} />
        <TextInput label="Phone" {...form.getInputProps('phone')} />
        <PasswordInput
          label="Password (required for creating, optional for editing)"
          {...form.getInputProps('password')}
          required={mode === 'create'}
        />
        <Fieldset legend="Location">
          <LocationPicker
            lat={form.values.latitude}
            lng={form.values.longitude}
            onLocationChange={({ lat, lng, address }) => {
              form.setFieldValue('latitude', lat);
              form.setFieldValue('longitude', lng);
              if (address) {
                form.setFieldValue('address', address);
              }
            }}
          />
          <TextInput label="Address" {...form.getInputProps('address')} required />
        </Fieldset>
        <Fieldset legend="Opening Hours">
          {form.values.openingHours.map((_oh, index) => (
            <div key={index}>
              <NumericSelect
                label="Day of Week"
                data={daysOfWeekOptions}
                {...form.getInputProps(`openingHours.${index}.dayOfWeek`)}
                allowDeselect={false}
                required
              />
              <TimePicker
                label="Open Time (HH:mm)"
                {...form.getInputProps(`openingHours.${index}.openTime`)}
                required
              />
              <TimePicker
                label="Close Time (HH:mm)"
                {...form.getInputProps(`openingHours.${index}.closeTime`)}
                required
              />
              <Button
                color="red"
                onClick={() => form.removeListItem('openingHours', index)}
                mt={'sm'}
              >
                Remove
              </Button>
              <Divider my={'sm'} />
            </div>
          ))}
          <Button
            onClick={() => {
              form.insertListItem('openingHours', defaultOpeningHours[0]);
            }}
          >
            Add Opening Hour
          </Button>
        </Fieldset>
        {form.errors && (
          <div style={{ color: 'red', marginTop: '10px' }}>
            {Object.values(form.errors).flat().join(', ')}
          </div>
        )}
        <Group>
          <Button type="submit" mt={'md'} color={'green'}>
            Save
          </Button>
          <Button onClick={onClose} mt={'md'} color={'gray'}>
            Cancel
          </Button>
        </Group>
      </Form>
    </Modal>
  );
};
