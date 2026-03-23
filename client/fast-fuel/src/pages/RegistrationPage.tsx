import { Register } from '../components/Registration/Registration.tsx';
import { Header } from '../components/Header/Header.tsx';

export const RegistrationPage = () => {
  return (
    <div>
      <Header title={'Registration'} />

      <Register />
    </div>
  );
};
