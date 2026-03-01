import type { ReactNode } from 'react';
import type { UseFormReturnType } from '@mantine/form';
import type { NumericSelectProps } from '../NumericSelect/NumericSelect.tsx';
import type { SelectProps } from '@mantine/core';

export type FormValues = Record<string, unknown>;

type BaseField<T extends FormValues> = {
  key: keyof T & string;
  label: string;
  required?: boolean | ((mode: 'create' | 'edit') => boolean);
  nullable?: boolean | ((mode: 'create' | 'edit') => boolean);
};

export type TextField<T extends FormValues> = BaseField<T> & { type: 'text' };
export type NumberField<T extends FormValues> = BaseField<T> & { type: 'number' };
export type PasswordField<T extends FormValues> = BaseField<T> & { type: 'password' };
export type TimeField<T extends FormValues> = BaseField<T> & { type: 'time' };
export type SelectField<T extends FormValues> = BaseField<T> & {
  type: 'select';
  selectProps?: SelectProps;
};
export type NumericSelectField<T extends FormValues> = BaseField<T> & {
  type: 'numericSelect';
  selectProps?: NumericSelectProps;
};
export type CustomField<T extends FormValues> = {
  type: 'custom';
  key: string;
  render: (form: UseFormReturnType<T>) => ReactNode;
};

export type ListField<T extends FormValues, Item extends FormValues = FormValues> = BaseField<T> & {
  type: 'list';
  defaultItem: Item;
  addButtonLabel?: string;
  itemFields: ListItemField<Item>[];
};

export type ListItemField<Item extends FormValues = FormValues> =
  | TextField<Item>
  | NumberField<Item>
  | TimeField<Item>
  | SelectField<Item>
  | NumericSelectField<Item>
  | CustomField<Item>;

// A field or fieldset that groups several fields
export type FieldOrFieldset<T extends FormValues> =
  | FieldDefinition<T>
  | { type: 'fieldset'; legend: string; fields: FieldDefinition<T>[] };

export type FieldDefinition<T extends FormValues> =
  | TextField<T>
  | NumberField<T>
  | PasswordField<T>
  | TimeField<T>
  | SelectField<T>
  | NumericSelectField<T>
  | CustomField<T>
  | ListField<T>;
