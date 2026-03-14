import type { components } from '../../../types/api';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';

type Shift = components['schemas']['ShiftResponseDto'];
type Employee = components['schemas']['EmployeeResponseDto'];

type ShiftFormValues = {
  id: number;
  employeeId: number;
  startTime: string;
  durationHours: number;
  durationMinutes: number;
};

export type ShiftManagerProps = {
  shifts: Shift[];
  refetchShifts: () => void;
  employees: Employee[];
};

const getDuration = (start: Date, end: Date) => {
  const diffMs = end.getTime() - start.getTime();
  const totalMinutes = Math.round(diffMs / 60000);
  return {
    hours: Math.floor(totalMinutes / 60),
    minutes: totalMinutes % 60,
  };
};

export const ShiftManager = ({ shifts, refetchShifts, employees }: ShiftManagerProps) => {
  const employeeOptions = (employees ?? []).map((e) => ({
    value: e.id,
    label: e.name,
  }));

  const tableColumns: ColumnDefinition<Shift>[] = [
    {
      header: 'Employee',
      render: (s) =>
        employeeOptions.find((o) => o.value === s.employeeId)?.label ?? `#${s.employeeId}`,
    },
    {
      header: 'Start',
      render: (s) => new Date(s.startTime).toLocaleString(),
    },
    {
      header: 'End',
      render: (s) => new Date(s.endTime).toLocaleString(),
    },
    {
      header: 'Duration',
      render: (s) => {
        const { hours, minutes } = getDuration(new Date(s.startTime), new Date(s.endTime));
        return `${hours}h ${minutes}m`;
      },
    },
  ];

  const editorFields: Field[] = [
    {
      type: 'numericSelect',
      key: 'employeeId',
      label: 'Employee',
      initialValue: null,
      nullable: 'never',
      required: 'always',
      fieldProps: {
        data: employeeOptions,
        placeholder: 'Select employee...',
        searchable: true,
      },
    },
    {
      type: 'dateTime',
      key: 'startTime',
      label: 'Start Time',
      initialValue: null,
      nullable: 'never',
      required: 'always',
    },
    {
      type: 'fieldset',
      key: 'duration-fieldset',
      legend: 'Duration',
      label: 'Duration',
      initialValue: [],
      nullable: 'never',
      required: 'never',
      layout: 'row',
      fields: [
        {
          type: 'number',
          key: 'durationHours',
          label: 'Hours',
          initialValue: 0,
          nullable: 'never',
          required: 'always',
          fieldProps: { min: 0, max: 23 },
        },
        {
          type: 'number',
          key: 'durationMinutes',
          label: 'Minutes',
          initialValue: 0,
          nullable: 'never',
          required: 'always',
          fieldProps: { min: 0, max: 59 },
        },
      ],
    },
  ];

  const { mutate: createShift } = apiClient.useMutation('post', '/api/Shift', {
    onSuccess: () => refetchShifts(),
  });
  const { mutate: updateShift } = apiClient.useMutation('put', '/api/Shift/{id}', {
    onSuccess: () => refetchShifts(),
  });
  const { mutate: deleteShift } = apiClient.useMutation('delete', '/api/Shift/{id}', {
    onSuccess: () => refetchShifts(),
  });

  const normalizeDateTime = (value: string) => {
    const trimmed = value.trim();
    return trimmed.includes('T') ? trimmed : trimmed.replace(' ', 'T');
  };

  const toRequestDto = (values: ShiftFormValues) => {
    const start = new Date(normalizeDateTime(values.startTime));
    const totalMinutes = (values.durationHours ?? 0) * 60 + (values.durationMinutes ?? 0);
    const end = new Date(start.getTime() + totalMinutes * 60 * 1000);
    return {
      employeeId: values.employeeId,
      startTime: start.toISOString(),
      endTime: end.toISOString(),
    };
  };

  const transformEditValues = (shift: Shift): ShiftFormValues => {
    const { hours, minutes } = getDuration(new Date(shift.startTime), new Date(shift.endTime));
    return {
      id: shift.id,
      employeeId: shift.employeeId,
      startTime: shift.startTime,
      durationHours: hours,
      durationMinutes: minutes,
    };
  };

  const handleSubmit = (values: ShiftFormValues, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createShift({ body: toRequestDto(values) });
    } else {
      updateShift({ params: { path: { id: values.id } }, body: toRequestDto(values) });
    }
  };

  return (
    <EntityManager<Shift, ShiftFormValues>
      title="Shifts"
      entityName="shift"
      data={[...shifts].sort(
        (a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime(),
      )}
      tableColumns={tableColumns}
      editorFields={editorFields}
      sectionKey={(s) => new Date(s.startTime).toLocaleDateString()}
      transformEditValues={transformEditValues}
      onSubmit={handleSubmit}
      onDelete={(s) => deleteShift({ params: { path: { id: s.id } } })}
    />
  );
};
