import type { ReactNode } from 'react';
import type {
  CheckboxProps,
  MultiSelectProps,
  NumberInputProps,
  PasswordInputProps,
  SelectProps,
  TextInputProps,
} from '@mantine/core';
import type { UseFormReturnType } from '@mantine/form';
import type { NumericMultiSelectProps, NumericSelectProps } from '../../common/NumericCombobox';
import type { DateTimePickerProps, TimePickerProps } from '@mantine/dates';

export type FormValues = Record<string, unknown>;

export type FieldType =
  | 'text'
  | 'number'
  | 'bool'
  | 'password'
  | 'time'
  | 'dateTime'
  | 'select'
  | 'numericSelect'
  | 'multiselect'
  | 'numericMultiSelect'
  | 'list'
  | 'fieldset'
  | 'custom';

type BaseField<Type extends FieldType, InitialValueType, FieldProps> = {
  key: string;
  type: Type;
  initialValue: InitialValueType;
  label: string;
  fieldProps?: OmitUnnecessaryComboboxProps<FieldProps>;
} & (
  | {
      nullable: 'never';
      required: 'always' | 'never';
    }
  | {
      nullable: 'always';
      required: 'never';
    }
  | {
      nullable: 'edit';
      required: 'create';
    }
);

export type OmitUnnecessaryComboboxProps<T> = Omit<T, 'value' | 'onChange' | 'label' | 'required'>;

export type TextField = BaseField<'text', string, TextInputProps>;
export type NumberField = BaseField<'number', number, NumberInputProps>;
export type BoolField = BaseField<'bool', boolean, CheckboxProps>;
export type PasswordField = BaseField<'password', string, PasswordInputProps>;
export type TimeField = BaseField<'time', string | null, TimePickerProps>;
export type DateTimeField = BaseField<'dateTime', string | null, DateTimePickerProps>;
export type SelectField = BaseField<'select', string | null, SelectProps>;
export type NumericSelectField = BaseField<'numericSelect', number | null, NumericSelectProps>;
export type MultiSelectField = BaseField<'multiselect', string[], MultiSelectProps>;
export type NumericMultiSelectField = BaseField<
  'numericMultiSelect',
  number[],
  NumericMultiSelectProps
>;
export type ListField = BaseField<'list', unknown[], never> & {
  items: Field[];
};
export type FieldSet = BaseField<'fieldset', unknown[], never> & {
  legend: string;
  fields: Field[];
  /** Controls how child fields are laid out. Defaults to 'column'. */
  layout?: 'row' | 'column';
};

export type CustomField = {
  type: 'custom';
  render: (form: UseFormReturnType<FormValues>, mode: EditorMode) => ReactNode;
};

export type Field =
  | TextField
  | NumberField
  | BoolField
  | PasswordField
  | TimeField
  | DateTimeField
  | SelectField
  | NumericSelectField
  | MultiSelectField
  | NumericMultiSelectField
  | ListField
  | FieldSet
  | CustomField;

export type EditorMode = 'create' | 'edit';
