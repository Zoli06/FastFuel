import { Select } from '@mantine/core';

export interface EditableSelectProps {
  value: string;
  onChange: (value: string | null) => void;
  editing: boolean;
  options: string[];
}

export const EditableSelect = ({ value, onChange, editing, options }: EditableSelectProps) => {
  const handleChange = (value: string | null) => {
    onChange(value);
  };

  return (
    <div>
      {editing ? (
        <Select value={value} onChange={handleChange} data={options} searchable />
      ) : (
        <span>{value}</span>
      )}
    </div>
  );
};
