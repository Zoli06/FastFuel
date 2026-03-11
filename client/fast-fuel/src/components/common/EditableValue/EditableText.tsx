import { TextInput } from '@mantine/core';
import type { ChangeEvent } from 'react';

export interface EditableTextProps {
  value: string;
  onChange: (value: string) => void;
  editing: boolean;
}

export const EditableText = ({ value, onChange, editing }: EditableTextProps) => {
  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    onChange(event.currentTarget.value);
  };

  return (
    <div>
      {editing ? <TextInput value={value} onChange={handleChange} /> : <span>{value}</span>}
    </div>
  );
};
