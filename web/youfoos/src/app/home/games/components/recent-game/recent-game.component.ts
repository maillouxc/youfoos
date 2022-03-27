import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

import { GameWithPlayers } from '../../game-items/gameWithPlayers';

@Component({
  selector: 'yf-recent-game',
  templateUrl: './recent-game.component.html',
  styleUrls: ['./recent-game.component.scss']
})
export class RecentGameComponent {

  @Input() recentGame$: GameWithPlayers;

  showEvents = false;

  constructor(private router: Router) {}

  /**
   * Navigates to the profile page of the given user.
   */
  public goToUserProfile(id: string) {
    this.router.navigate(['user', id]);
  }

  /**
   * Navigates to the tournament details page for the given tournament.
   */
  public goToTournament(id: string) : void {
    this.router.navigate(['tournaments', id]);
  }

  toggleEvents(): void {
    this.showEvents = !this.showEvents;
  }

}
