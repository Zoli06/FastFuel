import { Button, Table } from '@mantine/core';
import type { ReactNode } from 'react';

export type ColumnDefinition<Values> = {
  header: string;
  accessor?: keyof Values;
  render?: (item: Values) => ReactNode;
};

export type EntityTableProps<Values extends { id: number | string }> = {
  data: Values[];
  columns: ColumnDefinition<Values>[];
  renderRow?: (item: Values, columns: ColumnDefinition<Values>[]) => ReactNode;
  sectionKey?: (item: Values) => string;
  onEdit?: (item: Values) => void;
  onDelete?: (item: Values) => void;
};

export const EntityTable = <Values extends { id: number | string }>({
  data,
  columns,
  renderRow,
  sectionKey,
  onEdit,
  onDelete,
}: EntityTableProps<Values>) => {
  const defaultRenderRow = (item: Values) => (
    <Table.Tr key={item.id}>
      {columns.map((column, index) => (
        <Table.Td key={index}>
          {column.render
            ? column.render(item)
            : column.accessor
              ? item[column.accessor] == null
                ? ''
                : String(item[column.accessor])
              : ''}
        </Table.Td>
      ))}
      {onEdit && (
        <Table.Td>
          <Button onClick={() => onEdit(item)}>Edit</Button>
        </Table.Td>
      )}
      {onDelete && (
        <Table.Td>
          <Button color="red" onClick={() => onDelete(item)}>
            Delete
          </Button>
        </Table.Td>
      )}
    </Table.Tr>
  );

  const totalColumns = columns.length + (onEdit ? 1 : 0) + (onDelete ? 1 : 0);

  const renderRows = (): ReactNode => {
    if (!sectionKey) {
      return data.map((item) => (renderRow ? renderRow(item, columns) : defaultRenderRow(item)));
    }

    const rows: ReactNode[] = [];
    let currentSection: string | null = null;

    for (const item of data) {
      const section = sectionKey(item);
      if (section !== currentSection) {
        currentSection = section;
        rows.push(
          <Table.Tr key={`section-${section}`}>
            <Table.Td colSpan={totalColumns} fw="bold" bg="gray.1">
              {section}
            </Table.Td>
          </Table.Tr>,
        );
      }
      rows.push(renderRow ? renderRow(item, columns) : defaultRenderRow(item));
    }

    return rows;
  };

  return (
    <Table>
      <Table.Thead>
        <Table.Tr>
          {columns.map((column, index) => (
            <Table.Th key={index}>{column.header}</Table.Th>
          ))}
          {onEdit && <Table.Th>Edit</Table.Th>}
          {onDelete && <Table.Th>Delete</Table.Th>}
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>{renderRows()}</Table.Tbody>
    </Table>
  );
};
