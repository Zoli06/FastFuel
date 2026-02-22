import { Loginform } from '../components/Login/LoginForm.tsx';
import { HeaderGeneral } from '../components/Headers/HeaderGeneral.tsx';

export const Login = () => {
  return (
    <div>
      <HeaderGeneral title={'Login'} />

      <Loginform />
    </div>
  );
};
