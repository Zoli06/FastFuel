import { Checkbox } from '@mantine/core';
import type { ChangeEvent } from 'react';

export interface EditableBoolProps {
  value: boolean;
  onChange: (value: boolean) => void;
  editing: boolean;
}

export const EditableBool = ({ value, onChange, editing }: EditableBoolProps) => {
  const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
    onChange(event.currentTarget.checked);
  };

  return (
    <div>
      {editing ? (
        <Checkbox checked={value} onChange={handleChange} />
      ) : (
        <span>{String(value)}</span>
      )}
    </div>
  );
};
