import type { ReactNode } from 'react';
import {
  Button,
  Divider,
  MultiSelect,
  NumberInput,
  PasswordInput,
  Select,
  TextInput,
} from '@mantine/core';
import { type UseFormReturnType } from '@mantine/form';
import { TimePicker } from '@mantine/dates';
import { NumericSelect, NumericMultiSelect } from '../NumericCombobox';
import type { FieldDefinition, FormValues, ListField } from './types.ts';

export function renderField<TForm extends FormValues>(
  field: FieldDefinition<TForm>,
  form: UseFormReturnType<TForm>,
  mode: 'create' | 'edit',
): ReactNode {
  const getRequired = (required: boolean | ((mode: 'create' | 'edit') => boolean)): boolean => {
    if (typeof required === 'function') {
      return required(mode);
    }
    return required;
  };

  switch (field.type) {
    case 'text':
      return (
        <TextInput
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          required={getRequired(field.required ?? false)}
        />
      );

    case 'number':
      return (
        <NumberInput
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          type={undefined}
          required={getRequired(field.required ?? false)}
        />
      );

    case 'password':
      return (
        <PasswordInput
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          required={getRequired(field.required ?? false)}
        />
      );

    case 'time':
      return (
        <TimePicker
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          required={getRequired(field.required ?? false)}
        />
      );

    case 'select':
      return (
        <Select
          {...field.selectProps}
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          required={getRequired(field.required ?? false)}
        />
      );

    case 'numericSelect': {
      return (
        <NumericSelect
          {...field.selectProps}
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          required={getRequired(field.required ?? false)}
        />
      );
    }

    case 'multiselect':
      return (
        <MultiSelect
          {...field.selectProps}
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          required={getRequired(field.required ?? false)}
        />
      );

    case 'numericMultiSelect':
      return (
        <NumericMultiSelect
          {...field.selectProps}
          {...form.getInputProps(field.key)}
          key={field.key}
          label={field.label}
          required={getRequired(field.required ?? false)}
        />
      );

    case 'custom':
      return <span key={field.key}>{field.render(form)}</span>;

    case 'list':
      return renderListField(field, form, mode);

    default:
      return null;
  }
}

function renderListField<TForm extends FormValues>(
  field: ListField<TForm>,
  form: UseFormReturnType<TForm>,
  mode: 'create' | 'edit',
): ReactNode {
  const items = form.values[field.key] as FormValues[];
  return (
    <div key={field.key}>
      {items.map((_item, index) => (
        <div key={index}>
          {field.itemFields.map((itemField) => {
            const nestedForm: UseFormReturnType<FormValues> = {
              ...form,
              values: (form.values[field.key] as FormValues[])[index],
              getInputProps: (key: string) => form.getInputProps(`${field.key}.${index}.${key}`),
            } as UseFormReturnType<FormValues>;
            return renderField(itemField, nestedForm, mode);
          })}
          <Button color="red" onClick={() => form.removeListItem(field.key, index)} mt="sm">
            Remove
          </Button>
          <Divider my="sm" />
        </div>
      ))}
      <Button
        onClick={() => {
          (
            form as { insertListItem: (key: string, item: unknown, index?: number) => void }
          ).insertListItem(field.key, field.defaultItem);
        }}
      >
        {field.addButtonLabel ?? 'Add'}
      </Button>
    </div>
  );
}
