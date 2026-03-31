import { type ColumnDefinition, EntityTable } from '../EntityManager/EntityTable/EntityTable.tsx';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { apiClient } from '../../lib/api-client.ts';
import { getDuration, parseAsUtcDate } from '../../lib/time.ts';
import { Paper } from '../common/Paper/Paper.tsx';

export const MyShifts = () => {
  const [{ data: shifts = [] }] = useConditionalSuspenseQueries([
    apiClient.queryOptions('get', '/api/Shift/my'),
  ]);

  type Shift = (typeof shifts)[number];

  const tableColumns: ColumnDefinition<Shift>[] = [
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

  return (
    <Paper>
      <EntityTable
        data={shifts}
        columns={tableColumns}
        sectionKey={(s) => parseAsUtcDate(s.startTime).toLocaleDateString()}
      />
    </Paper>
  );
};
