import { NumberInput } from '@mantine/core';

export interface EditableNumber {
  value: number;
  onChange: (value: number) => void;
  editing: boolean;
}

export const EditableNumber = ({ value, onChange, editing }: EditableNumber) => {
  const handleChange = (value: string | number) => {
    if (typeof value === 'number') {
      onChange(value);
    }
  };

  return (
    <div>
      {editing ? <NumberInput value={value} onChange={handleChange} /> : <span>{value}</span>}
    </div>
  );
};
