import { DateTimePicker } from '@mantine/dates';

export interface EditableDateTimeProps {
  value: string | null;
  onChange: (value: string | null) => void;
  editing: boolean;
}

export const EditableDateTime = ({ value, onChange, editing }: EditableDateTimeProps) => {
  const handleChange = (event: string | null) => {
    onChange(event);
  };

  return (
    <div>
      {editing ? (
        <DateTimePicker value={value} onChange={handleChange} />
      ) : (
        <span>{value ? new Date(value).toLocaleString() : 'No date and time'}</span>
      )}
    </div>
  );
};
