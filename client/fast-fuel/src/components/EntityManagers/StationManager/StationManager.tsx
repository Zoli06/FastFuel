import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { $api } from '../../../lib/api.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import { Button } from '@mantine/core';
import { Link } from 'react-router-dom';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const StationManager = () => {
  const { necessaryPerm, recommendedPerm } = $api('StationManager');

  const restaurantReadApi = recommendedPerm('Permission:Restaurant:Read');
  const stationCategoryReadApi = recommendedPerm('Permission:StationCategory:Read');
  const viewTasksApi = recommendedPerm('Permission:Station:ViewTasks');

  const [
    { data: restaurants = [] },
    { data: stationCategories = [] },
    { data: stations = [], refetch: refetchStations },
  ] = useConditionalSuspenseQueries([
    restaurantReadApi?.queryOptions('get', '/api/Restaurant'),
    stationCategoryReadApi?.queryOptions('get', '/api/StationCategory'),
    necessaryPerm('Permission:Station:Read').queryOptions('get', '/api/Station'),
  ]);

  type Station = (typeof stations)[number];

  const restaurantNameById = new Map(restaurants.map((r) => [r.id, r.name]));
  const categoryNameById = new Map(stationCategories.map((c) => [c.id, c.name]));

  const restaurantOptions = restaurants.map((r) => ({ value: r.id, label: r.name }));
  const categoryOptions = stationCategories.map((c) => ({ value: c.id, label: c.name }));

  const tableColumns: ColumnDefinition<Station>[] = [
    { header: 'Name', accessor: 'name' },
    { header: 'In Operation', render: (s) => (s.inOperation ? 'Yes' : 'No') },
    ...(restaurantReadApi
      ? [
          {
            header: 'Restaurant',
            render: (s: Station) => restaurantNameById.get(s.restaurantId) ?? `#${s.restaurantId}`,
          },
        ]
      : []),
    ...(stationCategoryReadApi
      ? [
          {
            header: 'Category',
            render: (s: Station) =>
              categoryNameById.get(s.stationCategoryId) ?? `#${s.stationCategoryId}`,
          },
        ]
      : []),
    ...(viewTasksApi
      ? [
          {
            header: 'Tasks',
            render: (s: Station) => (
              <Link to={`/stations/${s.id}/tasks`}>
                <Button>View Tasks</Button>
              </Link>
            ),
          },
        ]
      : []),
  ];

  const editorFields: Field[] = [
    {
      type: 'text',
      key: 'name',
      label: 'Name',
      initialValue: '',
      nullable: 'never',
      required: 'always',
    },
    {
      type: 'bool',
      key: 'inOperation',
      label: 'In Operation',
      initialValue: true,
      nullable: 'never',
      required: 'always',
    },
    ...(restaurantReadApi
      ? [
          {
            type: 'numericSelect',
            key: 'restaurantId',
            label: 'Restaurant',
            initialValue: 0,
            nullable: 'never',
            required: 'always',
            fieldProps: {
              data: restaurantOptions,
              placeholder: 'Select restaurant',
              searchable: true,
            },
          } satisfies Field,
        ]
      : []),
    ...(stationCategoryReadApi
      ? [
          {
            type: 'numericSelect',
            key: 'stationCategoryId',
            label: 'Station Category',
            initialValue: 0,
            nullable: 'never',
            required: 'always',
            fieldProps: {
              data: categoryOptions,
              placeholder: 'Select category',
              searchable: true,
            },
          } satisfies Field,
        ]
      : []),
  ];

  const createStation = recommendedPerm('Permission:Station:Create')?.useMutation(
    'post',
    '/api/Station',
    {
      onSuccess: () => refetchStations(),
    },
  ).mutate;
  const updateStation = recommendedPerm('Permission:Station:Update')?.useMutation(
    'put',
    '/api/Station/{id}',
    {
      onSuccess: () => refetchStations(),
    },
  ).mutate;
  const deleteStation = recommendedPerm('Permission:Station:Delete')?.useMutation(
    'delete',
    '/api/Station/{id}',
    {
      onSuccess: () => refetchStations(),
    },
  ).mutate;

  const handleSubmit = (values: Station, mode: 'create' | 'edit') => {
    if (mode === 'create' && createStation) {
      createStation({ body: values });
    } else if (mode === 'edit' && updateStation) {
      updateStation({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Station>
      title="Stations"
      entityName="stations"
      data={stations}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={
        deleteStation ? (s) => deleteStation({ params: { path: { id: s.id } } }) : undefined
      }
      canCreate={!!createStation}
      canEdit={!!updateStation}
      canDelete={!!deleteStation}
    />
  );
};
