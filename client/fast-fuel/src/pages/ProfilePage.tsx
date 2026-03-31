import { Profile } from '../components/Profile/Profile.tsx';
import { PagePermissionGuard } from '../components/common/PagePermissionGuard/PagePermissionGuard.tsx';

export const ProfilePage = () => (
  <PagePermissionGuard page="Profile">
    <Profile />
  </PagePermissionGuard>
);
