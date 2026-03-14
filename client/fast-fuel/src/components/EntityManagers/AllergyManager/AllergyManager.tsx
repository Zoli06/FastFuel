import type { components } from '../../../types/api';
import { apiClient } from '../../../apiClient.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor';

type Allergy = components['schemas']['AllergyResponseDto'];
type Ingredient = components['schemas']['IngredientResponseDto'];

export type AllergyManagerProps = {
  allergies: Allergy[];
  ingredients: Ingredient[];
  refetchAllergies: () => void;
};

export const AllergyManager = ({
  allergies,
  refetchAllergies,
  ingredients,
}: AllergyManagerProps) => {
  const ingredientOptions = (ingredients ?? []).map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<Allergy>[] = [
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
      type: 'text',
      key: 'message',
      label: 'Message',
      nullable: 'always',
      required: 'never',
      initialValue: '',
    },
    {
      type: 'numericMultiSelect',
      key: 'ingredientIds',
      label: 'Ingredients',
      initialValue: [],
      nullable: 'never',
      required: 'never',
      fieldProps: {
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

  const handleSubmit = (values: Allergy, mode: 'create' | 'edit') => {
    if (mode === 'create') {
      createAllergy({ body: values });
    } else {
      updateAllergy({ params: { path: { id: values.id } }, body: values });
    }
  };

  return (
    <EntityManager<Allergy>
      title="Allergies"
      entityName="allergy"
      data={allergies}
      tableColumns={tableColumns}
      editorFields={editorFields}
      onSubmit={handleSubmit}
      onDelete={(r) => deleteAllergy({ params: { path: { id: r.id } } })}
    />
  );
};
