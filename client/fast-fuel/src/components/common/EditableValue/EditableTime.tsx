import { TimePicker } from '@mantine/dates';

export interface EditableTimeProps {
  value: string;
  onChange: (value: string) => void;
  editing: boolean;
}

export const EditableTime = ({ value, onChange, editing }: EditableTimeProps) => {
  const handleChange = (event: string) => {
    onChange(event);
  };

  return (
    <div>
      {editing ? (
        <TimePicker value={value} onChange={handleChange} />
      ) : (
        <span>{value ? value : 'No time selected'}</span>
      )}
    </div>
  );
};
