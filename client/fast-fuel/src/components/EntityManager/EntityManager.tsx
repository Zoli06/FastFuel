import { Button, Container, Divider, Flex, Title } from '@mantine/core';
import type { UseFormInput } from '@mantine/form';

import type { ColumnDefinition } from './EntityTable/EntityTable.tsx';
import { EntityTable } from './EntityTable/EntityTable.tsx';
import type { Field } from './EntityEditor/types.ts';
import { EntityEditor } from './EntityEditor/EntityEditor.tsx';
import { useEditorState } from './EntityEditor/useEditorState.ts';
import { Paper } from '../common/Paper/Paper.tsx';

export type EntityManagerProps<
  Values extends { id: number | string },
  FormValues extends { id: number | string } = Values,
> = {
  title: string;
  entityName: string;
  data: Values[];
  tableColumns: ColumnDefinition<Values>[];
  editorFields: Field[];
  validate?: UseFormInput<FormValues>['validate'];
  sectionKey?: (item: Values) => string;
  /** Transform a data item before it is loaded into the editor (e.g. derive extra form fields). */
  transformEditValues?: (item: Values) => FormValues;
  canCreate: boolean;
  canEdit: boolean;
  canDelete: boolean;
  canEditItem?: (item: Values) => boolean;
  canDeleteItem?: (item: Values) => boolean;
  onSubmit: (values: FormValues, mode: 'create' | 'edit') => void | Promise<void>;
  onDelete?: (item: Values) => void;
};

export const EntityManager = <
  Values extends { id: number | string },
  FormValues extends { id: number | string } = Values,
>({
  title,
  entityName,
  data,
  tableColumns,
  editorFields,
  validate,
  sectionKey,
  transformEditValues,
  canCreate,
  canEdit,
  canDelete,
  canEditItem,
  canDeleteItem,
  onSubmit,
  onDelete,
}: EntityManagerProps<Values, FormValues>) => {
  const { opened, mode, data: editedItem, openCreate, openEdit, close } = useEditorState<Values>();

  const handleSubmit = async (values: FormValues, submitMode: 'create' | 'edit') => {
    try {
      await onSubmit(values, submitMode);
      close();
    } catch {
      // Keep the editor open when submit fails so users can fix the input.
    }
  };

  const editorValues = editedItem
    ? transformEditValues
      ? transformEditValues(editedItem)
      : (editedItem as unknown as FormValues)
    : undefined;

  const editorModeAndData =
    mode === 'edit'
      ? { mode: 'edit' as const, values: editorValues! }
      : { mode: 'create' as const };

  return (
    <>
      <Paper>
        <Flex
          justify="space-between"
          align="center"
          gap="md"
          direction={{ base: 'column', sm: 'row' }}
        >
          <Title>{title}</Title>
          {canCreate && (
            <Button onClick={openCreate} color="green" style={{ flexShrink: 0 }}>
              Create
            </Button>
          )}
        </Flex>
        <Divider my="md" color="black" />
        <Container>
          <EntityTable<Values>
            data={data}
            columns={tableColumns}
            sectionKey={sectionKey}
            onEdit={canEdit ? openEdit : undefined}
            onDelete={canDelete ? onDelete : undefined}
            isEditEnabled={canEdit ? canEditItem : undefined}
            isDeleteEnabled={canDelete ? canDeleteItem : undefined}
          />
        </Container>
      </Paper>

      <EntityEditor<FormValues>
        {...editorModeAndData}
        opened={opened}
        onClose={close}
        title={mode === 'create' ? `Create ${entityName}` : `Edit ${entityName}`}
        fields={editorFields}
        validate={validate}
        onSubmit={handleSubmit}
      />
    </>
  );
};
