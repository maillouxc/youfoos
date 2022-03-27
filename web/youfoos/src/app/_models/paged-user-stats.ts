import { UserStatsDto } from '../_dtos/userStats.dto';

export class PagedUserStats {
  public pageNumber: number;
  public totalResults: number;
  public pageSize: number;
  public results: UserStatsDto[];
}
