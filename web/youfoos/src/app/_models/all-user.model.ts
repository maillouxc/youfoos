import { UserDto } from '../_dtos/user.dto';

export interface AllUser {
  pageNumber: number;
  pageSize: number;
  results: UserDto[];
}
