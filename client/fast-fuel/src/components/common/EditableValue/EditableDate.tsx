import { DatePickerInput } from '@mantine/dates';

export interface EditableDateProps {
  value: string | null;
  onChange: (value: string | null) => void;
  editing: boolean;
}

export const EditableDate = ({ value, onChange, editing }: EditableDateProps) => {
  const handleChange = (value: string | null) => {
    onChange(value);
  };

  return (
    <div>
      {editing ? (
        <DatePickerInput value={value} onChange={handleChange} />
      ) : (
        <span>{value ? new Date(value).toLocaleDateString() : 'No date'}</span>
      )}
    </div>
  );
};
