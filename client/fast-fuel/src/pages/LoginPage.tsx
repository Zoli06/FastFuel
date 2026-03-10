import { Login } from '../components/Login/Login.tsx';
import { Header } from '../components/Header/Header.tsx';

export const LoginPage = () => {
  return (
    <div>
      <Header title={'Login'} />

      <Login />
    </div>
  );
};
