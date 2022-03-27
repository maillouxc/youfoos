import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { GameService } from '../../../../_services/game.service';
import { GameDto } from '../../../../_dtos/responses/game.dto';
import { UserDto } from '../../../../_dtos/user.dto';

@Component({
  selector: 'yf-profile-recent-games',
  templateUrl: './profile-recent-games.component.html',
  styleUrls: ['./profile-recent-games.component.scss']
})
export class ProfileRecentGamesComponent implements OnInit {

  games$: Observable<GameDto[]>;

  @Input() user: UserDto;

  constructor(private gameService: GameService) {}

  ngOnInit() {
    this.loadRecentGamesData();
  }

  private loadRecentGamesData(): void {
    this.games$ = this.gameService.getUserRecentGames(this.user.id);
  }

  getStyleTeam(game) {
    let black = true;
    if (this.getTeam(game) === 'Gold') {
      black = false;
    }
    return {
      color: black ? 'black' : 'darkgoldenrod'
    };
  }

  getTeam(game: GameDto) {
    if (
      this.user.id === game.goldDefenseUserId ||
      this.user.id === game.goldOffenseUserId
    ) {
      return 'Gold';
    } else {
      return 'Black';
    }
  }

  getWinning(game) {
    if (game.goldTeamScore === game.blackTeamScore) {
      return 'Tie';
    } else if (game.goldTeamScore > game.blackTeamScore) {
      if (this.getTeam(game) === 'Gold') {
        return 'Won';
      } else {
        return 'Lost';
      }
    } else {
      if (this.getTeam(game) === 'Black') {
        return 'Won';
      } else {
        return 'Lost';
      }
    }
  }

  getStyleWin(game) {
    let red = true;
    if (this.getWinning(game) === 'Won') {
      red = false;
    }
    return {
      color: red ? 'red' : 'green'
    };
  }

}
