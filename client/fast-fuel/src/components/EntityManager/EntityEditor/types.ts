import type { ReactNode } from 'react';
import type { MultiSelectProps, SelectProps } from '@mantine/core';
import type { UseFormReturnType } from '@mantine/form';
import type { NumericMultiSelectProps, NumericSelectProps } from '../../common/NumericCombobox';

export type FormValues = Record<string, unknown>;

export type FieldType =
  | 'text'
  | 'number'
  | 'bool'
  | 'password'
  | 'time'
  | 'select'
  | 'numericSelect'
  | 'multiselect'
  | 'numericMultiSelect'
  | 'list'
  | 'fieldset'
  | 'custom';

type BaseField<Type extends FieldType, InitialValueType> = {
  key: string;
  type: Type;
  initialValue: InitialValueType;
  label: string;
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

export type TextField = BaseField<'text', string>;
export type NumberField = BaseField<'number', number>;
export type BoolField = BaseField<'bool', boolean>;
export type PasswordField = BaseField<'password', string>;
export type TimeField = BaseField<'time', string | null>;
export type SelectField = BaseField<'select', string | null> & {
  selectProps?: OmitUnnecessaryComboboxProps<SelectProps>;
};
export type NumericSelectField = BaseField<'numericSelect', number | null> & {
  selectProps?: OmitUnnecessaryComboboxProps<NumericSelectProps>;
};
export type MultiSelectField = BaseField<'multiselect', string[]> & {
  selectProps?: OmitUnnecessaryComboboxProps<MultiSelectProps>;
};
export type NumericMultiSelectField = BaseField<'numericMultiSelect', number[]> & {
  selectProps?: OmitUnnecessaryComboboxProps<NumericMultiSelectProps>;
};
export type ListField = BaseField<'list', unknown[]> & {
  items: Field[];
};
export type FieldSet = BaseField<'fieldset', unknown[]> & {
  legend: string;
  fields: Field[];
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
  | SelectField
  | NumericSelectField
  | MultiSelectField
  | NumericMultiSelectField
  | ListField
  | FieldSet
  | CustomField;

export type EditorMode = 'create' | 'edit';
