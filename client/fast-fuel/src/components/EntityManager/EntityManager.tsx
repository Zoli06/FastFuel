import { Button, Container, Divider, Flex, Title } from '@mantine/core';
import type { UseFormInput } from '@mantine/form';

import type { ColumnDefinition } from './EntityTable/EntityTable.tsx';
import { EntityTable } from './EntityTable/EntityTable.tsx';
import type { Field } from './EntityEditor';
import { EntityEditor, useEditorState } from './EntityEditor';
import { Paper } from '../common/Paper/Paper.tsx';

export type EntityManagerProps<Values extends { id: number | string }> = {
  title: string;
  entityName: string;
  data: Values[];
  tableColumns: ColumnDefinition<Values>[];
  editorFields: Field[];
  validate?: UseFormInput<Values>['validate'];
  onSubmit: (values: Values, mode: 'create' | 'edit') => void;
  onDelete?: (item: Values) => void;
};

export const EntityManager = <Values extends { id: number | string }>({
  title,
  entityName,
  data,
  tableColumns,
  editorFields,
  validate,
  onSubmit,
  onDelete,
}: EntityManagerProps<Values>) => {
  const { opened, mode, data: editedItem, openCreate, openEdit, close } = useEditorState<Values>();

  const handleSubmit = (values: Values, submitMode: 'create' | 'edit') => {
    onSubmit(values, submitMode);
    close();
  };

  const editorModeAndData =
    mode === 'edit' ? { mode: 'edit' as const, values: editedItem! } : { mode: 'create' as const };

  return (
    <>
      <Paper>
        <Flex justify="space-between" align="center">
          <Title>{title}</Title>
          <Button onClick={openCreate} color="green">
            Create
          </Button>
        </Flex>
        <Divider my="md" color="black" />
        <Container>
          <EntityTable<Values>
            data={data}
            columns={tableColumns}
            onEdit={openEdit}
            onDelete={onDelete}
          />
        </Container>
      </Paper>

      <EntityEditor<Values>
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
