import { Stats1v1Dto } from './responses/stats1v1.dto';
import { Stats2v2Dto } from './responses/stats2v2.dto';
import { StatsOverallDto } from './responses/statsOverall.dto';

export class UserStatsDto {
  public id: string;
  public userId: string;
  public stats1V1: Stats1v1Dto;
  public stats2V2: Stats2v2Dto;
  public statsOverall: StatsOverallDto;
  public timestamp: string;
}
