import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { AuthenticationService } from '../../../_services/authentication.service';
import { StatsService } from '../../../_services/stats.service';

interface ProgressBarValue {
  gamesNeeded: Number;
  gameWord: string;
  progress: Number;
}

@Component({
  selector: 'yf-games-progress-bar',
  templateUrl: 'progress.component.html',
  styleUrls: ['./progress-bar.scss'],
  animations: [
    trigger('simpleFadeAnimation', [
      state('in', style({opacity: 1})),
      transition(':enter', [style({ opacity: 0 }), animate(600 )]),
    ])
  ]
})
export class GamesProgressComponent implements OnInit {

  @Input() gameType: string;

  constructor(
    private statsService: StatsService,
    private authenticator: AuthenticationService
  ) {}

  showBar$: Observable<boolean>;
  progressData$: Observable<ProgressBarValue>;

  ngOnInit() {}

  //Every Change will reset progress data to a new observable. Async Pipe will handle unsubscribing.
  ngOnChanges(changes: SimpleChanges) {
    this.progressData$ = this.getProgressData();
    this.showBar$ = this.progressData$.pipe(map(progress => {
      // Hide the progress bar if they've already played the required number of games
      if (progress.gamesNeeded > 0) {
        return true;
      } else {
        return false;
      }
    }))
  }

  getProgressData(): Observable<ProgressBarValue> {
       return this.statsService.getUserStats(this.authenticator.currentUserValue.id).pipe(
          map(stats => {
            let gameWord = '';
            let gamesNeeded = 10;

            switch (this.gameType) {
              case 'Overall':
                gamesNeeded = 10 - (stats.statsOverall.gamesWon + stats.statsOverall.gamesLost);
                break;
              case '1V1':
                gamesNeeded = 10 - (stats.stats1V1.gamesWon + stats.stats1V1.gamesLost);
                gameWord = '1V1';
                break;
              case '2V2':
                gamesNeeded = 10 - (stats.stats2V2.gamesWon + stats.stats2V2.gamesLost);
                gameWord = '2V2';
                break;
            }

            // We don't want to show weird negative numbers in the UI
            if (gamesNeeded < 0) {
              gamesNeeded = 0;
            }

            // Determine whether to make the word game plural or singular
            if (gamesNeeded == 1) {
              gameWord += ' game';
            } else {
              gameWord += ' games';
            }

            return <ProgressBarValue>{
              progress: (10 - gamesNeeded) * 10,
              gamesNeeded: gamesNeeded,
              gameWord: gameWord
            };
          })
        );
  }

  onDismiss() {
    this.showBar$ = of(false);
  }

}
