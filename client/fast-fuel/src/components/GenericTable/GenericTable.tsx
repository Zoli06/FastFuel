import { Button, Table } from '@mantine/core';
import type { ReactNode } from 'react';

export type ColumnDefinition<T> = {
  header: string;
  accessor?: keyof T;
  render?: (item: T) => ReactNode;
};

export type GenericTableProps<T extends { id: number | string }> = {
  data: T[];
  columns: ColumnDefinition<T>[];
  renderRow?: (item: T, columns: ColumnDefinition<T>[]) => ReactNode;
  onEdit?: (item: T) => void;
  onDelete?: (item: T) => void;
};

export const GenericTable = <T extends { id: number | string }>({
  data,
  columns,
  renderRow,
  onEdit,
  onDelete,
}: GenericTableProps<T>) => {
  const defaultRenderRow = (item: T) => (
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
      <Table.Tbody>
        {data.map((item) => (renderRow ? renderRow(item, columns) : defaultRenderRow(item)))}
      </Table.Tbody>
    </Table>
  );
};
