import { Pipe, PipeTransform } from '@angular/core';

/**
 * Formats a given value which is a number of seconds as an h m s format string.
 * That is, 62 would become 1m 2s.
 */
@Pipe({name: 'secondsToHms'})
export class SecondsBasedStatPipe implements PipeTransform {

  transform(value: number): string {
    // Do the calculations to get the individual values
    let hours = Math.floor(value / 3600);
    let remainderSecs = value % 3600;
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
