import type { components } from '../../types/api';
import { apiClient } from '../../apiClient.ts';
import { EntityManager } from '../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../GenericTable/GenericTable.tsx';
import type { FieldOrFieldset } from '../GenericEditor';
import { TextInput } from '@mantine/core';

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

const maxLength = 100;
const getDisplayedMessage = (message: string | null) => {
  if (!message) return 'No message provided';
  return message.length > maxLength ? `${message.substring(0, maxLength)}...` : message;
};

const tableColumns: ColumnDefinition<AllergyDto>[] = [
  { header: 'Name', accessor: 'name' },
  { header: 'Ingredient IDs', accessor: 'ingredientIds' },
  { header: 'Message', render: (r) => getDisplayedMessage(r.message) },
];

const editorFields: FieldOrFieldset<AllergyFormValues>[] = [
  { type: 'text', key: 'name', label: 'Name', required: true, initialValue: '' },
  { type: 'text', key: 'message', label: 'Message', nullable: true, initialValue: '' },
  {
    type: 'custom',
    key: 'ingredientIds',
    render: (form) => (
      <TextInput
        label="Ingredient IDs"
        description="Comma-separated ingredient IDs (for example: 1, 2, 3)"
        placeholder="1, 2, 3"
        value={(form.values.ingredientIds ?? []).join(', ')}
        onChange={(event) => {
          const parsed = event.currentTarget.value
            .split(',')
            .map((part) => part.trim())
            .filter((part) => part.length > 0)
            .map((part) => Number(part))
            .filter((value) => Number.isInteger(value) && value > 0);

          form.setFieldValue('ingredientIds', parsed);

          //TODO
          //Remake this into a multiselect where the id is the value and the label is the ingredient's
          //name and it gets these from the ingredients in backend
        }}
      />
    ),
  },
];

export const Allergies = ({ allergies, refetchAllergies }: AllergyProps) => {
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
