import { GameDto } from '../../../_dtos/responses/game.dto';
import { Player } from '../../../_models/player.model';

export class GameWithPlayers {

  constructor(public game: GameDto,
              public goldDefensePlayer: Player,
              public goldOffensePlayer: Player,
              public blackOffensePlayer: Player,
              public blackDefensePlayer: Player,
              public winner: string) {}

}
