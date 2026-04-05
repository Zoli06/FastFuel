import { EntityManager } from '../../EntityManager/EntityManager.tsx';
import type { ColumnDefinition } from '../../EntityManager/EntityTable/EntityTable.tsx';
import type { Field } from '../../EntityManager/EntityEditor/types.ts';
import { $api } from '../../../lib/api.ts';
import { useConditionalSuspenseQueries } from '../../../hooks/useConditionalSuspenseQueries.ts';

export const AllergyManager = () => {
  const { necessaryPerm, recommendedPerm } = $api('AllergyManager');
  const ingredientReadApi = recommendedPerm('Permission:Ingredient:Read');

  const [{ data: ingredients = [] }, { data: allergies = [], refetch: refetchAllergies }] =
    useConditionalSuspenseQueries([
      ingredientReadApi?.queryOptions('get', '/api/Ingredient'),
      necessaryPerm('Permission:Allergy:Read').queryOptions('get', '/api/Allergy'),
    ]);

  type Allergy = (typeof allergies)[number];

  const ingredientOptions = ingredients.map((ingredient) => ({
    value: ingredient.id,
    label: ingredient.name,
  }));

  const tableColumns: ColumnDefinition<Allergy>[] = [
    { header: 'Name', accessor: 'name' },
    ...(ingredientReadApi
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
    ...(ingredientReadApi
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

  const createAllergy = recommendedPerm('Permission:Allergy:Create')?.useMutation(
    'post',
    '/api/Allergy',
    {
      onSuccess: () => refetchAllergies(),
    },
  ).mutate;
  const updateAllergy = recommendedPerm('Permission:Allergy:Update')?.useMutation(
    'put',
    '/api/Allergy/{id}',
    {
      onSuccess: () => refetchAllergies(),
    },
  ).mutate;
  const deleteAllergy = recommendedPerm('Permission:Allergy:Delete')?.useMutation(
    'delete',
    '/api/Allergy/{id}',
    {
      onSuccess: () => refetchAllergies(),
    },
  ).mutate;

  const handleSubmit = (values: Allergy, mode: 'create' | 'edit') => {
    if (mode === 'create' && createAllergy) {
      createAllergy({ body: values });
    } else if (mode === 'edit' && updateAllergy) {
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
      onDelete={
        deleteAllergy ? (r) => deleteAllergy({ params: { path: { id: r.id } } }) : undefined
      }
      canCreate={!!createAllergy}
      canEdit={!!updateAllergy}
      canDelete={!!deleteAllergy}
    />
  );
};
