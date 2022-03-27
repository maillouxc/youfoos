import { Observable } from 'rxjs';

import { UserStatsDto } from '../../../_dtos/userStats.dto';
import { Player } from '../../../_models/player.model';

export class LeaderboardItem {

  constructor(public position: number,
              public player: Observable<Player>,
              public statData: UserStatsDto) {}

}
