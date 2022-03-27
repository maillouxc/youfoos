import { UserAvatarDto } from '../_dtos/useravatar.dto';

export interface User {
  id: any;
  email: string;
  firstAndLastName: string;
  token?: string;
  avatar?: UserAvatarDto;
  isAdmin: boolean;
}
