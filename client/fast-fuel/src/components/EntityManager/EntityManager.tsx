import { Button, Container, Divider, Flex, Paper, Title } from '@mantine/core';
import type { UseFormInput } from '@mantine/form';

import { GenericTable } from '../GenericTable/GenericTable.tsx';
import type { ColumnDefinition } from '../GenericTable/GenericTable.tsx';
import { GenericEditor } from '../GenericEditor';
import type { FieldOrFieldset, FormValues } from '../GenericEditor';
import { useEditorState } from '../GenericEditor/useEditorState.ts';

export type EntityManagerProps<TData extends { id: number | string }, TForm extends FormValues> = {
  title: string;
  entityName: string;
  data: TData[];
  columns: ColumnDefinition<TData>[];
  fields: FieldOrFieldset<TForm>[];
  getInitialValues: (data: TData | null) => TForm;
  validate?: UseFormInput<TForm>['validate'];
  onSubmit: (values: TForm, mode: 'create' | 'edit', item: TData | null) => void;
  onDelete?: (item: TData) => void;
  addButtonLabel?: string;
  createTitle?: string;
  editTitle?: string;
};

export const EntityManager = <TData extends { id: number | string }, TForm extends FormValues>({
  title,
  entityName,
  data,
  columns,
  fields,
  getInitialValues,
  validate,
  onSubmit,
  onDelete,
  addButtonLabel,
  createTitle,
  editTitle,
}: EntityManagerProps<TData, TForm>) => {
  const { opened, mode, data: editingItem, openCreate, openEdit, close } = useEditorState<TData>();

  const handleSubmit = (values: TForm, submitMode: 'create' | 'edit') => {
    onSubmit(values, submitMode, editingItem);
    close();
  };

  const editorModeAndData =
    mode === 'edit' ? { mode: 'edit' as const, data: editingItem } : { mode: 'create' as const };

  return (
    <>
      <Container className="entity-manager">
        <Paper shadow="xs" p="xl" bg="beige" withBorder>
          <Flex justify="space-between" align="center">
            <Title>{title}</Title>
            <Button onClick={openCreate} color="green">
              {addButtonLabel ?? `Add ${title}`}
            </Button>
          </Flex>
          <Divider my="md" color="black" />
          <Container>
            <GenericTable<TData>
              data={data}
              columns={columns}
              onEdit={openEdit}
              onDelete={onDelete}
            />
          </Container>
        </Paper>
      </Container>

      <GenericEditor<TForm, TData>
        {...editorModeAndData}
        opened={opened}
        onClose={close}
        title={
          mode === 'create'
            ? (createTitle ?? `Create ${entityName}`)
            : (editTitle ?? `Edit ${entityName}`)
        }
        fields={fields}
        getInitialValues={getInitialValues}
        validate={validate}
        onSubmit={handleSubmit}
      />
    </>
  );
};
