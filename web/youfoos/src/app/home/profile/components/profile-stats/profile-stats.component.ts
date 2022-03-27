import { Component, Input, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { StatsService } from '../../../../_services/stats.service';
import { UserStatsDto } from '../../../../_dtos/userStats.dto';
import { UserService } from '../../../../_services/user.service';
import { Player } from 'src/app/_models/player.model';

export interface CalculatedStats extends UserStatsDto {

  // Derived stats
  gamesPlayedOverall: number;
  gamesPlayed1V1: number;
  gamesPlayed2V2: number;
  gamesLostOffense: number;
  gamesLostDefense: number;
  goalsPerGame1V1: number;
  defenseGoalsPerGame: number;
  offenseGoalsPerGame: number;

  winrateTooltipOverall: string;
  winrateTooltip1V1: string;
  winrateTooltip2V2: string;
  winrateTooltipOffense: string;
  winrateTooltipDefense: string;
}
@Component({
  selector: 'yf-profile-stats',
  templateUrl: './profile-stats.component.html',
  styleUrls: ['./profile-stats.component.scss']
})
export class ProfileStatsComponent implements AfterViewInit {
  userCalculatedStats$: Observable<CalculatedStats>;

  bestPartner$: Observable<Player>;
  worstPartner$: Observable<Player>;
  mostFrequentPartner$: Observable<Player>;

  @Input() userId: string;

  constructor(
    private statsService: StatsService,
    private userService: UserService,
    private router: Router
  ) {}

  ngAfterViewInit() {
    this.userCalculatedStats$ = this.loadStatsData(this.userId);
  }

  private loadStatsData(userId: string): Observable<CalculatedStats> {
    // Load the associated info for the partner stats
    return this.statsService.getUserStats(userId).pipe(
      map(stats => {
        let defenseGoalsPerGame = 0;
        let goalsPerGame1V1 = 0;
        let offenseGoalsPerGame = 0;

        if (stats.stats2V2.bestPartnerId != null) {
          this.bestPartner$ = this.getPlayerInfo(stats.stats2V2.bestPartnerId);
        }
        if (stats.stats2V2.worstPartnerId != null) {
          this.worstPartner$ = this.getPlayerInfo(
            stats.stats2V2.worstPartnerId
          );
        }
        if (stats.stats2V2.mostFrequentPartnerId != null) {
          this.mostFrequentPartner$ = this.getPlayerInfo(
            stats.stats2V2.mostFrequentPartnerId
          );
        }

        // Handle the stats which need to be calculated from combining other stats
        const gamesPlayedOverall =
          stats.statsOverall.gamesWon + stats.statsOverall.gamesLost;
        const gamesPlayed1V1 =
          stats.stats1V1.gamesWon + stats.stats1V1.gamesWon;
        const gamesPlayed2V2 =
          stats.stats2V2.gamesWon + stats.stats2V2.gamesLost;
        const gamesLostOffense =
          stats.stats2V2.gamesAsOffense - stats.stats2V2.offenseWins;
        const gamesLostDefense =
          stats.stats2V2.gamesAsDefense - stats.stats2V2.defenseWins;

        // Calculate the winrate tooltips - used for showing the exact numbers of wins/losses
        const winrateTooltipOverall = `${stats.statsOverall.gamesWon} wins - ${stats.statsOverall.gamesLost} losses`;
        const winrateTooltip1V1 = `${stats.stats1V1.gamesWon} wins - ${stats.stats1V1.gamesLost} losses`;
        const winrateTooltip2V2 = `${stats.stats2V2.gamesWon} wins - ${stats.stats2V2.gamesLost} losses`;
        const winrateTooltipOffense = `${stats.stats2V2.offenseWins} wins - ${gamesLostOffense} losses`;
        const winrateTooltipDefense = `${stats.stats2V2.defenseWins} wins - ${gamesLostDefense} losses`;

        if (stats.stats2V2.goalsScoredAsDefense > 0) {
          defenseGoalsPerGame =
            stats.stats2V2.goalsScoredAsDefense / stats.stats2V2.gamesAsDefense;
        }

        if (stats.stats2V2.goalsScoredAsOffense > 0) {
          offenseGoalsPerGame =
            stats.stats2V2.goalsScoredAsOffense / stats.stats2V2.gamesAsOffense;
        }

        if (stats.stats1V1.goalsScored > 0) {
          goalsPerGame1V1 =
            stats.stats1V1.goalsScored /
            (stats.stats1V1.gamesLost + stats.stats1V1.gamesWon);
        }
        return <CalculatedStats>{...stats,
          gamesPlayedOverall: gamesPlayedOverall,
          gamesPlayed1V1: gamesPlayed1V1,
          gamesPlayed2V2: gamesPlayed2V2,
          gamesLostOffense: gamesLostOffense,
          gamesLostDefense: gamesLostDefense,
          goalsPerGame1V1: goalsPerGame1V1,
          defenseGoalsPerGame: defenseGoalsPerGame,
          offenseGoalsPerGame: offenseGoalsPerGame,
          winrateTooltipOverall: winrateTooltipOverall,
          winrateTooltip1V1: winrateTooltip1V1,
          winrateTooltip2V2: winrateTooltip2V2,
          winrateTooltipOffense: winrateTooltipOffense,
          winrateTooltipDefense: winrateTooltipDefense
        };
      })
    );
  }

  private getPlayerInfo(id: string): Observable<Player> {
    return this.userService.getPlayerInfoAndAvatar(id);
  }

  getWinrateColor(value: number): object {
    if (value >= 0.85) {
      return { color: '#8A2BE2' };
    } else if (value >= 0.65) {
      return { color: '#32CD32' };
    } else if (value >= 0.5) {
      return { color: 'forestgreen' };
    } else if (value >= 0.45) {
      return { color: 'goldenrod' };
    } else if (value >= 0.4) {
      return { color: 'goldenrod' };
    } else if (value >= 0.3) {
      return { color: 'darkorange' };
    } else if (value >= 0.25) {
      return { color: '#FF4600' };
    } else {
      return { color: '#FF0000' };
    }
  }

  goToUserProfile(id: string) {
    this.router.navigate(['user', id]);
  }

}
