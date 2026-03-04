export type { FormValues } from './types.ts';
export type {
  TextField,
  PasswordField,
  TimeField,
  NumericSelectField,
  NumericMultiSelectField,
  CustomField,
  ListField,
  ListItemField,
  FieldDefinition,
  FieldOrFieldset,
} from './types.ts';

import { useEffect, useMemo } from 'react';
import { Button, Fieldset, Group, Modal } from '@mantine/core';
import { Form, useForm, type UseFormInput } from '@mantine/form';
import type { FieldOrFieldset, FormValues } from './types.ts';
import { buildInitialValues, collectNullableKeys, convertNullables } from './helpers.ts';
import { renderField } from './renderFields.tsx';

type GenericEditorPropsCreate = { mode: 'create' };
type GenericEditorPropsEdit<TData> = { mode: 'edit'; data: TData | null };

export type GenericEditorProps<TForm extends FormValues, TData = TForm> = (
  | GenericEditorPropsCreate
  | GenericEditorPropsEdit<TData>
) & {
  opened: boolean;
  onClose: () => void;
  title: string;
  fields: FieldOrFieldset<TForm>[];
  validate?: UseFormInput<TForm>['validate'];
  onSubmit: (values: TForm, mode: 'create' | 'edit') => void;
};

export const GenericEditor = <TForm extends FormValues, TData = TForm>(
  props: GenericEditorProps<TForm, TData>,
) => {
  const { mode, opened, onClose, title, fields, validate, onSubmit } = props;
  const data = mode === 'edit' ? (props as GenericEditorPropsEdit<TData>).data : null;

  const nullableKeys = useMemo(() => collectNullableKeys(fields), [fields]);
  const initialValues = useMemo(() => buildInitialValues(fields), [fields]);

  const form = useForm<TForm>({
    mode: 'controlled',
    initialValues,
    validate,
  });

  useEffect(() => {
    if (!opened) return;

    const values = { ...initialValues, ...(data || {}) };
    form.reset();
    form.setValues(values);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [mode, data, opened]);

  const renderFieldOrFieldset = (item: FieldOrFieldset<TForm>, idx: number) => {
    if (item.type === 'fieldset') {
      return (
        <Fieldset key={idx} legend={item.legend}>
          {item.fields.map((f) => renderField(f, form, mode))}
        </Fieldset>
      );
    }
    return renderField(item, form, mode);
  };

  if (!opened) return null;

  return (
    <Modal opened={opened} onClose={onClose} title={title}>
      <Form
        form={form}
        onSubmit={(values) => {
          const finalValues = convertNullables(values, nullableKeys);
          onSubmit(finalValues, mode);
        }}
      >
        {fields.map((f, idx) => renderFieldOrFieldset(f, idx))}

        {form.errors && Object.keys(form.errors).length > 0 && (
          <div style={{ color: 'red', marginTop: '10px' }}>
            {Object.values(form.errors).flat().join(', ')}
          </div>
        )}

        <Group>
          <Button type="submit" mt="md" color="green">
            Save
          </Button>
          <Button onClick={onClose} mt="md" color="gray">
            Cancel
          </Button>
        </Group>
      </Form>
    </Modal>
  );
};
