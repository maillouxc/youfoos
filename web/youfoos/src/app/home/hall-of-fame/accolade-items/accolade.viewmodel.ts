import { AccoladeDto } from '../../../_dtos/responses/accolade.dto';
import { Player } from '../../../_models/player.model';
import { AccoladeConnotation } from '../../../_dtos/enums/AccoladeConnotation';

export class AccoladeViewmodel {

  accoladeName: string;
  connotation: AccoladeConnotation;
  awardeeName: string;
  statValue: string;
  userId:string;
  image: string;

  constructor(accolade: AccoladeDto, player: Player) {
    this.accoladeName = accolade.name;
    this.connotation = accolade.connotation;

    switch (accolade.type) {
      case "PlayerSpecific": {
        this.statValue = this.getFormattedValue(accolade.name, accolade.currentValue);
        this.userId = player.id;
        this.awardeeName = player.name;
        this.image = player.avatar
        break;
      }
      case "NonPlayerEntitySpecific": {
        this.statValue = this.getFormattedValue(accolade.name, accolade.currentValue);

        // Display the appropriate image for the accolade
        if (accolade.name === 'Best Table Side') {
          this.image = '../../assets/images/icons/best_foosball_team.png';
        }

        // Format the weird capitalization on best table side accolade
        if (accolade.name === 'Best Table Side') {
          if (accolade.entityName === 'GOLD') {
            this.awardeeName = 'Gold';
          }
          else {
            this.awardeeName = 'Black';
          }
        }
        break;
      }
      case "NonEntitySpecific": {
        this.statValue = this.getFormattedValue(accolade.name, accolade.currentValue);
        this.image = '../../assets/images/icons/system_stat.png';
        break;
      }
      default: {
        console.log("Unrecognized accolade type");
        break;
      }
    }
  }

  private getFormattedValue(accoladeName: string, value: string): string {
    // Handle formatting of time based stats
    if (accoladeName === 'Most Time Played'
      || accoladeName === 'Total Time Played'
      || accoladeName === 'Longest Avg. Game Length') {
      return this.formatSecondsBasedStat(Number(value))
    }

    if (accoladeName === 'Best Table Side') {
      return + (Number(value) * 100).toFixed(1) + '% Wins';
    }

    if (accoladeName === 'Lowest Winrate' || accoladeName === 'Highest Winrate') {
      return + (Number(value) * 100).toFixed(1) + '%';
    }

    // If the accolade name is not one of the above, just pass it through with no change
    return value;
  }

  private formatSecondsBasedStat(numSeconds: number) {
    // Do the calculations to get the individual values
    let hours = Math.floor(numSeconds / 3600);
    let remainderSecs = numSeconds % 3600;
    let minutes = Math.floor(remainderSecs / 60);
    remainderSecs = remainderSecs %  60;

    // Now format the values
    let resultsString = "";
    if (hours > 0) { resultsString += hours + 'h '; }
    if (minutes > 0) { resultsString += minutes + 'm '; }
    resultsString += remainderSecs.toFixed(0) + 's';

    return resultsString;
  }

}
