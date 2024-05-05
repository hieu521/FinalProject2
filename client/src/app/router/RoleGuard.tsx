import { useAppSelector } from "../../app/store/configureStore";

interface RoleGuardProps {
  roles: string[];
  children: React.ReactNode;
}

export const RoleGuard = ({ roles, children }: RoleGuardProps) => {
  const { user } = useAppSelector((state) => state.account);

  if (user && roles.some((role) => user.roles?.includes(role))) {
    return <>{children}</>;
  }

  return null;
};