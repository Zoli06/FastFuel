import { Loginform } from './LoginForm';
import { HeaderGeneral } from '../Headers/HeaderGeneral.tsx';

export const Login = () => {
  return (
    <div>
      <HeaderGeneral title={'Login'} />

      <Loginform />
    </div>
  );
};
