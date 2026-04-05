import { useSuspenseQuery } from '@tanstack/react-query';
import { Stack } from '@mantine/core';
import { $api } from '../../lib/api.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { ProfileHeader } from './ProfileHeader.tsx';
import { ProfileEditForm } from './ProfileEditForm.tsx';
import { ProfileEmployeeCard } from './ProfileEmployeeCard.tsx';
import { ProfileMachineCard } from './ProfileMachineCard.tsx';

export const Profile = () => {
  const { noPerm, recommendedPerm } = $api('Profile');
  const customerUpdateSelfApi = recommendedPerm('Permission:Customer:UpdateSelf');

  const { data: currentUser, refetch: refetchCurrentUser } = useSuspenseQuery(
    noPerm().queryOptions('get', '/api/User/me'),
  );

  const isCustomer = currentUser.userType === 'Customer';
  const isEmployee = currentUser.userType === 'Employee';
  const isMachine = currentUser.userType === 'Machine';

  const canEdit = isCustomer && !!customerUpdateSelfApi;

  const [
    { data: customerData, refetch: refetchCustomer },
    { data: employeeData },
    { data: machineData },
  ] = useConditionalSuspenseQueries([
    isCustomer ? noPerm().queryOptions('get', '/api/Customer/me') : undefined,
    isEmployee ? noPerm().queryOptions('get', '/api/Employee/me') : undefined,
    isMachine ? noPerm().queryOptions('get', '/api/Machine/me') : undefined,
  ]);

  const email = customerData?.email ?? '';

  return (
    <Stack p="md" maw={800} mx="auto">
      <ProfileHeader
        name={currentUser.name}
        userName={currentUser.userName}
        userType={currentUser.userType}
      />

      {canEdit && (
        <ProfileEditForm
          name={currentUser.name}
          userName={currentUser.userName}
          email={email}
          onSaved={() => {
            void refetchCurrentUser();
            void refetchCustomer();
          }}
        />
      )}

      {isEmployee && employeeData && <ProfileEmployeeCard data={employeeData} />}
      {isMachine && machineData && <ProfileMachineCard data={machineData} />}
    </Stack>
  );
};
