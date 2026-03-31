import { useQuery, useSuspenseQuery } from '@tanstack/react-query';
import { Stack } from '@mantine/core';
import { apiClient, myCurrentUserQueryOptions } from '../../lib/api-client.ts';
import { ProfileHeader } from './ProfileHeader.tsx';
import { ProfileEditForm } from './ProfileEditForm.tsx';
import { ProfileEmployeeCard } from './ProfileEmployeeCard.tsx';
import { ProfileMachineCard } from './ProfileMachineCard.tsx';

export const Profile = () => {
  const { data: currentUser, refetch: refetchCurrentUser } = useSuspenseQuery(
    myCurrentUserQueryOptions(),
  );

  const isCustomer = currentUser.userType === 'Customer';
  const isAdmin = currentUser.userType === 'Admin';
  const isEmployee = currentUser.userType === 'Employee';
  const isMachine = currentUser.userType === 'Machine';

  const { data: customerData, refetch: refetchCustomer } = useQuery({
    ...apiClient.queryOptions('get', '/api/Customer/me'),
    enabled: isCustomer,
  });

  const { data: adminData, refetch: refetchAdmin } = useQuery({
    ...apiClient.queryOptions('get', '/api/Admin/me'),
    enabled: isAdmin,
  });

  const { data: employeeData } = useQuery({
    ...apiClient.queryOptions('get', '/api/Employee/me'),
    enabled: isEmployee,
  });

  const { data: machineData } = useQuery({
    ...apiClient.queryOptions('get', '/api/Machine/me'),
    enabled: isMachine,
  });

  const specificUser = customerData ?? adminData;
  const email = specificUser?.email ?? '';

  return (
    <Stack p="md" maw={800} mx="auto">
      <ProfileHeader
        name={currentUser.name}
        userName={currentUser.userName}
        userType={currentUser.userType}
      />

      {(isCustomer || isAdmin) && (
        <ProfileEditForm
          userId={currentUser.id}
          name={currentUser.name}
          userName={currentUser.userName}
          email={email}
          userType={isCustomer ? 'Customer' : 'Admin'}
          onSaved={() => {
            void refetchCurrentUser();
            void (isCustomer ? refetchCustomer() : refetchAdmin());
          }}
        />
      )}

      {isEmployee && employeeData && <ProfileEmployeeCard data={employeeData} />}
      {isMachine && machineData && <ProfileMachineCard data={machineData} />}
    </Stack>
  );
};
