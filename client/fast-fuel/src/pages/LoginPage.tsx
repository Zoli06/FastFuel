import { LoginForm } from '../components/Login/LoginForm.tsx';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral.tsx';

export const LoginPage = () => {
  return (
    <div>
      <HeaderGeneral title={'Login'} />

      <LoginForm />
    </div>
  );
};
