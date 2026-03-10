import { type ReactNode, useEffect, useMemo } from 'react';
import {
  Button,
  Fieldset,
  Group,
  Modal,
  MultiSelect,
  NumberInput,
  PasswordInput,
  Select,
  Stack,
  TextInput,
} from '@mantine/core';
import { Form, useForm, type UseFormInput, type UseFormReturnType } from '@mantine/form';
import { TimePicker } from '@mantine/dates';
import { NumericMultiSelect, NumericSelect } from '../../common/NumericCombobox';
import type { EditorMode, Field, FormValues, ListField } from './types.ts';
import { IconTrash } from '@tabler/icons-react';

function buildInitialValues(fields: Field[]): FormValues {
  const values: FormValues = {};
  for (const field of fields) {
    if (field.type === 'custom') continue;
    if (field.type === 'fieldset') {
      Object.assign(values, buildInitialValues(field.fields));
    } else {
      values[field.key] = field.initialValue;
    }
  }
  return values;
}

/** Keys whose value should be set to null under certain conditions */
type NullableKey = { key: string; when: 'always' | 'edit' };

function collectNullableKeys(fields: Field[]): NullableKey[] {
  const keys: NullableKey[] = [];
  for (const field of fields) {
    if (field.type === 'custom') continue;
    if (field.type === 'fieldset') {
      keys.push(...collectNullableKeys(field.fields));
    } else if (field.nullable === 'always' || field.nullable === 'edit') {
      keys.push({ key: field.key, when: field.nullable });
    }
  }
  return keys;
}

function convertNullables(
  values: FormValues,
  nullableKeys: NullableKey[],
  mode: EditorMode,
): FormValues {
  const result = { ...values };
  for (const { key, when } of nullableKeys) {
    if (when === 'always' || (when === 'edit' && mode === 'edit')) {
      if (result[key] === '' || result[key] === undefined) {
        result[key] = null;
      }
    }
  }
  return result;
}

function isRequired(
  field: Exclude<Field, { type: 'custom' | 'fieldset' | 'list' }>,
  mode: EditorMode,
): boolean {
  if (field.nullable === 'always') return false; // required: 'never' enforced by type
  if (field.nullable === 'edit') return mode === 'create'; // required: 'create'
  // nullable: 'never'
  return field.required === 'always';
}

function renderField(
  field: Field,
  form: UseFormReturnType<FormValues>,
  mode: EditorMode,
): ReactNode {
  switch (field.type) {
    case 'text':
      return (
        <TextInput
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          {...form.getInputProps(field.key)}
        />
      );

    case 'number':
      return (
        <NumberInput
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          type={undefined}
          {...form.getInputProps(field.key)}
        />
      );

    case 'password':
      return (
        <PasswordInput
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          {...form.getInputProps(field.key)}
        />
      );

    case 'time':
      return (
        <TimePicker
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          {...form.getInputProps(field.key)}
        />
      );

    case 'select':
      return (
        <Select
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          {...field.selectProps}
          {...form.getInputProps(field.key)}
        />
      );

    case 'numericSelect':
      return (
        <NumericSelect
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          {...field.selectProps}
          {...form.getInputProps(field.key)}
        />
      );

    case 'multiselect':
      return (
        <MultiSelect
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          {...field.selectProps}
          {...form.getInputProps(field.key)}
        />
      );

    case 'numericMultiSelect':
      return (
        <NumericMultiSelect
          key={form.key(field.key)}
          label={field.label}
          required={isRequired(field, mode)}
          {...field.selectProps}
          {...form.getInputProps(field.key)}
        />
      );

    case 'fieldset':
      return (
        <Fieldset key={field.key} legend={field.legend}>
          <Stack>
            {field.fields.map((f, i) => (
              <span key={'key' in f ? f.key : i}>{renderField(f, form, mode)}</span>
            ))}
          </Stack>
        </Fieldset>
      );

    case 'list':
      return renderListField(field, form, mode);

    case 'custom':
      return <span>{field.render(form, mode)}</span>;

    default:
      return null;
  }
}

function renderListField(
  field: ListField,
  form: UseFormReturnType<FormValues>,
  mode: EditorMode,
): ReactNode {
  const listValue = (form.getValues()[field.key] ?? []) as FormValues[];

  const newItem = (): FormValues => {
    const item: FormValues = {};
    for (const itemField of field.items) {
      if (itemField.type === 'custom') continue;
      item[itemField.key] = itemField.initialValue;
    }
    return item;
  };

  return (
    <Stack key={field.key} gap="xs">
      {listValue.map((_, index) => (
        <Group key={index} align="flex-end" wrap="nowrap">
          <Group grow align="flex-end" style={{ flex: 1 }}>
            {field.items.map((itemField) =>
              itemField.type === 'custom'
                ? itemField.render(form, mode)
                : renderField(
                    { ...itemField, key: `${field.key}.${index}.${itemField.key}` } as Field,
                    form,
                    mode,
                  ),
            )}
          </Group>
          <Button color="red" size="sm" onClick={() => form.removeListItem(field.key, index)}>
            <IconTrash size={16} />
          </Button>
        </Group>
      ))}
      <Button onClick={() => form.insertListItem(field.key, newItem())}>Add</Button>
    </Stack>
  );
}

export type EntityEditorProps<Values extends FormValues> = {
  opened: boolean;
  onClose: () => void;
  title: string;
  fields: Field[];
  validate?: UseFormInput<Values>['validate'];
  onSubmit: (values: Values, mode: EditorMode) => void;
} & ({ mode: 'create' } | { mode: 'edit'; values: Values });

export const EntityEditor = <Values extends FormValues>(props: EntityEditorProps<Values>) => {
  const { mode, opened, onClose, title, fields, validate, onSubmit } = props;
  const editValues = mode === 'edit' ? props.values : null;

  const initialValues = useMemo(() => buildInitialValues(fields) as Values, [fields]);
  const nullableKeys = useMemo(() => collectNullableKeys(fields), [fields]);

  const form = useForm<Values>({
    mode: 'uncontrolled',
    initialValues,
    validate,
  });

  useEffect(() => {
    if (!opened) return;
    const values = { ...initialValues };
    if (editValues) {
      for (const [k, v] of Object.entries(editValues)) {
        (values as FormValues)[k] = v ?? initialValues[k];
      }
    }
    form.setInitialValues(values);
    form.reset();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [mode, editValues, opened]);

  if (!opened) return null;

  return (
    <Modal opened={opened} onClose={onClose} title={title} size="lg">
      <Form
        form={form}
        onSubmit={(values) => {
          const finalValues = convertNullables(values, nullableKeys, mode) as Values;
          onSubmit(finalValues, mode);
        }}
      >
        <Stack>
          {fields.map((f, i) => (
            <span key={'key' in f ? f.key : i}>
              {renderField(f, form as UseFormReturnType<FormValues>, mode)}
            </span>
          ))}
        </Stack>

        {form.errors && Object.keys(form.errors).length > 0 && (
          <div style={{ color: 'red', marginTop: '10px' }}>
            {Object.values(form.errors).flat().join(', ')}
          </div>
        )}

        <Group mt="md" justify="flex-end">
          <Button variant="outline" onClick={onClose} color="red">
            Cancel
          </Button>
          <Button type="submit" disabled={!form.isValid()}>
            {mode === 'create' ? 'Create' : 'Save'}
          </Button>
        </Group>
      </Form>
    </Modal>
  );
};
