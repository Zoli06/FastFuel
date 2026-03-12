import type { components } from '../../../types/api';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';

type Station = components['schemas']['StationResponseDto'];
type Restaurant = components['schemas']['RestaurantResponseDto'];
type StationCategory = components['schemas']['StationCategoryResponseDto'];

export type StationManagerProps = {
  stations: Station[];
  refetchStations: () => void;
  restaurants: Restaurant[];
  stationCategories: StationCategory[];
};

export const StationManager = ({
  stations,
  refetchStations,
  restaurants,
  stationCategories,
}: StationManagerProps) => {
  const restaurantNameById = new Map((restaurants ?? []).map((r) => [r.id, r.name]));
  const categoryNameById = new Map((stationCategories ?? []).map((c) => [c.id, c.name]));

  const restaurantOptions = (restaurants ?? []).map((r) => ({ value: r.id, label: r.name }));
  const categoryOptions = (stationCategories ?? []).map((c) => ({ value: c.id, label: c.name }));

  const tableColumns: ColumnDefinition<Station>[] = [
    { header: 'Name', accessor: 'name' },
    {
      header: 'In Operation',
      render: (s) => (s.inOperation ? 'Yes' : 'No'),
    },
    {
      header: 'Restaurant',
      render: (s) => restaurantNameById.get(s.restaurantId) ?? `#${s.restaurantId}`,
    },
    {
      header: 'Category',
      render: (s) => categoryNameById.get(s.stationCategoryId) ?? `#${s.stationCategoryId}`,
    },
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
    {
      type: 'numericSelect',
      key: 'restaurantId',
      label: 'Restaurant',
      initialValue: 0,
      nullable: 'never',
      required: 'always',
      selectProps: {
        data: restaurantOptions,
        placeholder: 'Select restaurant',
        searchable: true,
      },
    },
    {
      type: 'numericSelect',
      key: 'stationCategoryId',
      label: 'Station Category',
      initialValue: 0,
      nullable: 'never',
      required: 'always',
      selectProps: {
        data: categoryOptions,
        placeholder: 'Select category',
        searchable: true,
      },
    },
  ];

  const { mutate: createStation } = apiClient.useMutation('post', '/api/Station', {
    onSuccess: () => refetchStations(),
  });
  const { mutate: updateStation } = apiClient.useMutation('put', '/api/Station/{id}', {
    onSuccess: () => refetchStations(),
  });
  const { mutate: deleteStation } = apiClient.useMutation('delete', '/api/Station/{id}', {
    onSuccess: () => refetchStations(),
  });

  const handleSubmit = (values: Station, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createStation({ body: values });
    } else {
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
      onDelete={(s) => deleteStation({ params: { path: { id: s.id } } })}
    />
  );
};
