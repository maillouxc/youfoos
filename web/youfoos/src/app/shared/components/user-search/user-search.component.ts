import { Component, EventEmitter, forwardRef, OnInit, Output } from "@angular/core";
import { FormControl, NG_VALUE_ACCESSOR } from "@angular/forms";
import { Observable, Subject } from 'rxjs';
import { debounceTime, exhaustMap, filter, scan, startWith, switchMap, tap } from 'rxjs/operators';
import { takeWhileInclusive } from "rxjs-take-while-inclusive";

import { UserService } from "../../../_services/user.service";
import { Player } from "../../../_models/player.model";

/**
 * A reusable component that can be used any time that one needs to select a user from a list of users.
 *
 * This component allows the user to enter in part of the user's name and it will call the API for autocomplete
 * suggestions, paging intelligently, and handling all the complexities of doing so.
 */
@Component({
  selector: 'yf-user-search',
  templateUrl: 'user-search.component.html',
  styleUrls: ['./user-search.component.scss'],
  providers:[
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UserSearchComponent),
      multi: true
    }
  ]
})
export class UserSearchComponent implements OnInit {

  @Output() userSelected = new EventEmitter<Player>();

  searchTextControl: FormControl = new FormControl();

  nextPage$: Subject<Player> = new Subject();
  searchResults$: Observable<Player[]>;

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.searchResults$ = this.getUserList();
  }

  /**
   * Emits an Observable of users that changes as the user types in their username search string.
   *
   * This function intelligently pages API responses and manages debouncing and other advanced concepts.
   */
  getUserList(): Observable<Player[]> {
    return this.searchTextControl.valueChanges.pipe(
      startWith(''),
      debounceTime(600),
      filter(q => typeof q === 'string')
    ).pipe(
      switchMap(searchedName => {
        let currentPage = 0; // Reset the page with every new search text

        return this.nextPage$.pipe(
          startWith(currentPage),
          exhaustMap(_ => this.userService.getAllUsers(currentPage, 10, searchedName)), // Wait for API response.
          tap(() => currentPage++), // When the API returns a page, increment the page number and continue.
          takeWhileInclusive(page => page.length > 0), // If no results for this search or no more pages, stop.
          scan((currentResults, newResultsPage) => currentResults.concat(newResultsPage), [])
        );
      })
    );
  }

  /**
   * Called when the user clicks on one of the autocompleted user items.
   */
  onClick(user: Player) {
    this.userSelected.emit(user);
  }

  /**
   * Called by the template when the user scrolls the list of searched users.
   *
   * Triggers the next page subject to emit an event and begin loading the next page of users from the API.
   */
  onScroll(): void {
    this.nextPage$.next();
  }

  /**
   * Called by the template to decide how to display the object returned by the autocomplete element.
   */
  displayWith(player: Player) {
    return player?.name;
  }

}
