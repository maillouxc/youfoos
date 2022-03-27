import { Component, Input, OnInit } from '@angular/core';
import { Observable } from "rxjs";

import { AchievementsService } from '../../../../_services/achievements.service';
import { AchievementStatusDto } from "../../../../_dtos/responses/achievementStatus.dto";

/**
 * Component that displays the achievements that a user has.
 */
@Component({
    selector: 'yf-user-achievements',
    templateUrl: './user-achievements.component.html',
    styleUrls: ['./user-achievements.component.scss']
})
export class UserAchievementsComponent implements OnInit {

  @Input() userId: string;

  userAchievements$: Observable<AchievementStatusDto[]>

  constructor(private achievementsService: AchievementsService) {}

  ngOnInit(): void {
    this.userAchievements$ = this.achievementsService.getUserAchievements(this.userId);
  }

  private getImageForAchievement(achievementName: string, unlockDate: string): string {
    const pathPrefix: string = '../../../../assets/images/icons/achievements';

    if (!unlockDate) {
      return `${pathPrefix}/achievement_locked.png`;
    }

    switch (achievementName) {
      case "Man of the People":
        return `${pathPrefix}/man_of_the_people.png`;
      case "Look, Mom!":
        return `${pathPrefix}/look_mom.png`;
      case "Seppuku":
        return `${pathPrefix}/seppuku.png`;
      case "Sore Back":
        return `${pathPrefix}/sore_back.png`;
      case "King of the World":
        return `${pathPrefix}/king_of_the_world.png`;
      case "Comeback King":
        return `${pathPrefix}/comeback_king.png`;
      case "It's Not An Addiction":
        return `${pathPrefix}/its_not_an_addiction.png`;
      case "I Wasn't Ready!":
        return `${pathPrefix}/i_wasnt_ready.png`;
      case "Kingslayer":
        return `${pathPrefix}/kingslayer.png`;
      case "On a Roll":
        return `${pathPrefix}/on_a_roll.png`;
      case "Penultimate":
        return `${pathPrefix}/penultimate.png`;
      case "Slow Roller":
        return `${pathPrefix}/slow_roller.png`;
      case "Thank U, Next.":
        return `${pathPrefix}/thank_u_next.png`;
      case "The Best Offense...":
        return `${pathPrefix}/the_best_offense.png`;
      case "Reproducible Bug":
        return `${pathPrefix}/reproducible_bug.png`;
      default:
        return `${pathPrefix}/../system_stat.png`;
    }
  }

}
