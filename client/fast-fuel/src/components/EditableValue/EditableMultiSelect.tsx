import { MultiSelect } from '@mantine/core';

export interface EditableMultiSelectProps {
  value: string[];
  onChange: (value: string[]) => void;
  editing: boolean;
  options: string[];
}

export const EditableMultiSelect = ({
  value,
  onChange,
  editing,
  options,
}: EditableMultiSelectProps) => {
  const handleChange = (value: string[]) => {
    onChange(value);
  };

  return (
    <div>
      {editing ? (
        <MultiSelect value={value} onChange={handleChange} data={options} searchable />
      ) : (
        <span>{value.join(', ')}</span>
      )}
    </div>
  );
};
