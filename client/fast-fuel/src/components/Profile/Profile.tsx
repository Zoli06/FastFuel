import { useSuspenseQuery } from '@tanstack/react-query';
import { Stack } from '@mantine/core';
import { apiClient, myCurrentUserQueryOptions } from '../../lib/api-client.ts';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { usePagePermissions } from '../../hooks/usePagePermissions.ts';
import { ProfileHeader } from './ProfileHeader.tsx';
import { ProfileEditForm } from './ProfileEditForm.tsx';
import { ProfileEmployeeCard } from './ProfileEmployeeCard.tsx';
import { ProfileMachineCard } from './ProfileMachineCard.tsx';

export const Profile = () => {
  const { data: currentUser, refetch: refetchCurrentUser } = useSuspenseQuery(
    myCurrentUserQueryOptions(),
  );
  const { recommended } = usePagePermissions('Profile');

  const isCustomer = currentUser.userType === 'Customer';
  const isEmployee = currentUser.userType === 'Employee';
  const isMachine = currentUser.userType === 'Machine';

  const canEdit = isCustomer && recommended.Customer.UpdateSelf;

  const [
    { data: customerData, refetch: refetchCustomer },
    { data: employeeData },
    { data: machineData },
  ] = useConditionalSuspenseQueries([
    isCustomer && apiClient.queryOptions('get', '/api/Customer/me'),
    isEmployee && apiClient.queryOptions('get', '/api/Employee/me'),
    isMachine && apiClient.queryOptions('get', '/api/Machine/me'),
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
