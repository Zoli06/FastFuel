import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';
import { getDuration, normalizeDateTime, parseAsUtcDate } from '../../../lib/time.ts';

export const ShiftManager = () => {
  const { recommended } = usePagePermissions('ShiftManager');

  const [{ data: employees = [] }, { data: shifts = [], refetch: refetchShifts }] =
    useConditionalSuspenseQueries([
      recommended.Employee.Read && apiClient.queryOptions('get', '/api/Employee'),
      apiClient.queryOptions('get', '/api/Shift'),
    ]);

  type Shift = (typeof shifts)[number];
  type ShiftFormValues = Shift & {
    durationHours: number;
    durationMinutes: number;
  };

  const employeeOptions = employees.map((e) => ({
    value: e.id,
    label: e.name,
  }));

  const tableColumns: ColumnDefinition<Shift>[] = [
    ...(recommended.Employee.Read
      ? [
          {
            header: 'Employee',
            render: (s: Shift) =>
              employeeOptions.find((o) => o.value === s.employeeId)?.label ?? `#${s.employeeId}`,
          },
        ]
      : []),
    {
      header: 'Start',
      render: (s) => parseAsUtcDate(s.startTime).toLocaleString(),
    },
    {
      header: 'End',
      render: (s) => parseAsUtcDate(s.endTime).toLocaleString(),
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
    ...(recommended.Employee.Read
      ? [
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
          } satisfies Field,
        ]
      : []),
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
      endTime: shift.endTime,
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
      sectionKey={(s) => parseAsUtcDate(s.startTime).toLocaleDateString()}
      transformEditValues={transformEditValues}
      onSubmit={handleSubmit}
      onDelete={(s) => deleteShift({ params: { path: { id: s.id } } })}
      canCreate={recommended.Shift.Create}
      canEdit={recommended.Shift.Update}
      canDelete={recommended.Shift.Delete}
    />
  );
};
