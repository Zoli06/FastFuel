import { Register } from '../components/Registration/Registration.tsx';

export const RegistrationPage = () => {
  return (
    <div>
      <Header title={'Registration'} authButton={'Login'} />

      <Register />
    </div>
  );
  return <Register />;
};
