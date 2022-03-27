import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { from, Observable, of } from 'rxjs';
import { map, switchMap, concatMap, toArray } from 'rxjs/operators';

import { AccoladeViewmodel } from './accolade-items/accolade.viewmodel';
import { AccoladeService } from '../../_services/accolade.service';
import { UserService } from '../../_services/user.service';

@Component({
  selector: 'yf-hall-of-fame',
  templateUrl: './hall-of-fame.component.html',
  styleUrls: ['./hall-of-fame.component.scss']
})
export class HallOfFameComponent implements OnInit {
  accolades$: Observable<AccoladeViewmodel[]>

  constructor(
    private accoladeService: AccoladeService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.accolades$ = this.getAccolades();
  }

  getAccolades(): Observable<AccoladeViewmodel[]> {
    return this.accoladeService
      .getAccolades()
      .pipe(
        switchMap(accolades =>
          from(accolades).pipe(
            concatMap(accolade =>{
              if(accolade.userId){
                return this.userService
                .getPlayerInfoAndAvatar(accolade.userId)
                .pipe(map(player => new AccoladeViewmodel(accolade, player)))
              } else {
                return of(new AccoladeViewmodel(accolade, null));
              }
            }
            ), toArray()
          )
        )
      );
  }

  /**
   * Called when the user clicks on one of the profile pictures in an accolade.
   * Navigates to that clicked user's profile page.
   * @param id The ID of the user's page to navigate to.
   */
  goToProfile(id: string): void {
    this.router.navigate(['user', id]);
  }
}
