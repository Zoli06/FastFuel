import type { components } from '../../types/api';
import { apiClient } from '../../apiClient.ts';
import { EntityManager } from '../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../GenericTable/GenericTable.tsx';
import type { FieldOrFieldset } from '../GenericEditor';

export type AllergyProps = {
  allergies: components['schemas']['AllergyResponseDto'][];
  refetchAllergies: () => void;
};

type AllergyDto = components['schemas']['AllergyResponseDto'];

type AllergyFormValues = {
  name: string;
  message: string;
  ingredientIds: number[];
  id: number;
};

export const Allergies = ({ allergies, refetchAllergies }: AllergyProps) => {
  const { data: ingredients } = apiClient.useQuery('get', '/api/Ingredient');

  const ingredientOptions = (ingredients ?? []).map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<AllergyDto>[] = [
    { header: 'Name', accessor: 'name' },
    {
      header: 'Ingredients',
      render: (r) => {
        if (!r.ingredientIds?.length) return 'None';
        return r.ingredientIds
          .map((id) => ingredientOptions.find((o) => o.value === id)?.label ?? id)
          .join(', ');
      },
    },
    { header: 'Message', render: (r) => r.message ?? 'No message provided' },
  ];

  const editorFields: FieldOrFieldset<AllergyFormValues>[] = [
    { type: 'text', key: 'name', label: 'Name', required: true, initialValue: '' },
    { type: 'text', key: 'message', label: 'Message', nullable: true, initialValue: '' },
    {
      type: 'numericMultiSelect',
      key: 'ingredientIds',
      label: 'Ingredients',
      initialValue: [],
      selectProps: {
        data: ingredientOptions,
        placeholder: 'Search ingredients...',
        searchable: true,
        clearable: true,
      },
    },
  ];
  const { mutate: createAllergy } = apiClient.useMutation('post', '/api/Allergy', {
    onSuccess: () => refetchAllergies(),
  });
  const { mutate: updateAllergy } = apiClient.useMutation('put', '/api/Allergy/{id}', {
    onSuccess: () => refetchAllergies(),
  });
  const { mutate: deleteAllergy } = apiClient.useMutation('delete', '/api/Allergy/{id}', {
    onSuccess: () => refetchAllergies(),
  });

  const handleSubmit = (
    values: AllergyFormValues,
    mode: 'create' | 'edit',
    item: AllergyDto | null,
  ) => {
    const body = values;
    if (mode === 'create') {
      createAllergy({ body });
    } else {
      updateAllergy({ params: { path: { id: item!.id } }, body });
    }
  };

  return (
    <EntityManager<AllergyDto, AllergyFormValues>
      title="Allergies"
      entityName="allergies"
      data={allergies}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(r) => deleteAllergy({ params: { path: { id: r.id } } })}
    />
  );
};
