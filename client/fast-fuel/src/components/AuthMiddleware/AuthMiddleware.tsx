import { type ReactNode, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { registerNavigate } from '../../lib/api-client.ts';

export function AuthMiddleware({ children }: { children: ReactNode }) {
  const navigate = useNavigate();

  useEffect(() => {
    registerNavigate(navigate);
  }, [navigate]);

  return <>{children}</>;
}
