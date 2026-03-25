import { apiClient } from '../../../lib/api-client.ts';
import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { usePagePermissions } from '../../../hooks/usePagePermissions.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const AllergyManager = () => {
  const { necessary, recommended } = usePagePermissions('AllergyManager', { split: true });

  const [{ data: ingredients = [] }, { data: allergies = [], refetch: refetchAllergies }] =
    useConditionalSuspenseQueries([
      recommended.Ingredient.Read && apiClient.queryOptions('get', '/api/Ingredient'),
      necessary.Allergy.Read && apiClient.queryOptions('get', '/api/Allergy'),
    ]);

  type Allergy = (typeof allergies)[number];

  const ingredientOptions = ingredients.map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<Allergy>[] = [
    { header: 'Name', accessor: 'name' },
    ...(recommended.Ingredient.Read
      ? [
          {
            header: 'Ingredients',
            render: (r: Allergy) => {
              if (!r.ingredientIds?.length) return 'None';
              return r.ingredientIds
                .map((id) => ingredientOptions.find((o) => o.value === id)?.label ?? id)
                .join(', ');
            },
          },
        ]
      : []),
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
    ...(recommended.Ingredient.Read
      ? [
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
          } satisfies Field,
        ]
      : []),
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
      canCreate={recommended.Allergy.Create}
      canEdit={recommended.Allergy.Update}
      canDelete={recommended.Allergy.Delete}
    />
  );
};
