import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from '@angular/router';
import { first, map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { StatsService } from '../../_services/stats.service';
import { UserService } from '../../_services/user.service';
import { LeaderboardItem } from './leaderboard-items/leaderboard-item';
import { PagedUserStats } from '../../_models/paged-user-stats';
import { LeaderboardSorter } from './leaderboard-items/leaderboard-sorter';
import { Player } from "../../_models/player.model";

@Component({
  selector: 'yf-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.scss']
})
export class LeaderboardComponent implements OnInit, AfterViewInit {

  // These are the columns that are displayed based on what mode the leaderboard is in
  columnsOverall: string[] = ['position', 'avatar', 'name', 'gamesOverall', 'goalsOverall', 'time'];
  columns1v1: string[] = ['position', 'avatar', 'name', 'games1v1', 'goals1v1'];
  columns2v2: string[] = ['position', 'avatar', 'name', 'games2v2', 'offense', 'defense'];
  currentColumns: string[];

  pageSize = 0;
  totalResults = 0;
  sortBy = 'Winrate';
  currentSortOptions: string[] = LeaderboardSorter.sortOptionsOverall;
  leaderboardType: string = 'Overall';
  searchedPlayerName = '';

  dataSource: MatTableDataSource<LeaderboardItem>;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  pageEvent: void;

  constructor(
    private statsService: StatsService,
    private userService: UserService,
    private _router: Router,
    public snackBar: MatSnackBar
  ) {}

  get router(): Router {
    return this._router;
  }

  ngOnInit() {
    this.currentColumns = this.columnsOverall;
    this.setupPaginator();
    this.dataSource = new MatTableDataSource<LeaderboardItem>();
  }

  ngAfterViewInit() {
    this.loadData();
  }

  public handleUserSelectedEvent(user: Player) {
    this.searchedPlayerName = user.name;
    this.loadData(user);
  }

  public handlePageEvent(e: PageEvent) {
    this.loadData();
  }

  /**
   * Reloads the leaderboard data for the page - this will cause the leaderboard to refresh based on current params.
   */
  public loadData(searchedPlayer?: Player): void {
    this.getLeaderboardData(searchedPlayer).pipe(
      map(stats => {
        if (searchedPlayer) {
          this.paginator.pageIndex = stats.pageNumber;
        }

        return this.createListOfLeaderboardItems(stats);
      }),
      first()
    ).subscribe(data => {
      this.dataSource.data = data;
    },
      error => {
        this.showSnackbar(searchedPlayer);
      });
  }

  /**
   * Called when the user changes the value of the leaderboard mode radio button is changed.
   *
   * Reloads the leaderboard with the new mode's data and layout.
   */
  public changeLeaderboardStatType(type: string) {
    this.leaderboardType = type;
    this.paginator.pageIndex = 0;

    switch (type) {
      case 'Overall':
        this.currentSortOptions = LeaderboardSorter.sortOptionsOverall;
        this.currentColumns = this.columnsOverall;
        break;
      case '1V1':
        this.currentSortOptions = LeaderboardSorter.sortOptions1V1;
        this.currentColumns = this.columns1v1;
        break;
      case '2V2':
        this.currentSortOptions = LeaderboardSorter.sortOptions2V2;
        this.currentColumns = this.columns2v2;
        break;
    }

    this.sortBy = this.currentSortOptions[0];
    this.loadData();
  }

  private createListOfLeaderboardItems(stats: PagedUserStats): LeaderboardItem[] {
    let i = this.paginator.pageSize * this.paginator.pageIndex;
    return stats.results.map(stat => {
      this.totalResults = stats.totalResults;
      return new LeaderboardItem(
        ++i,
        this.userService.getPlayerInfoAndAvatar(stat.userId),
        stat
      );
    });
  }

  private getLeaderboardData(searchedPlayer?: Player): Observable<PagedUserStats> {
    if (searchedPlayer) {
      this.searchedPlayerName = searchedPlayer?.name ?? '';
      this.paginator.pageIndex = 0;
    }

    return this.statsService.getLeaderboardPage(
      LeaderboardSorter.formatSortOptionsAsStatNameForBackend(this.sortBy),
      this.leaderboardType,
      this.paginator.pageSize,
      this.paginator.pageIndex,
      searchedPlayer?.id
    );
  }

  private showSnackbar(searchedPlayer): void {
    if (searchedPlayer) {
      let msg = `${searchedPlayer?.name} was not found. Enough games played?`;
      this.snackBar.open(msg, 'Dismiss', {duration: 5000});
    }
  }

  private setupPaginator(): void {
    this.paginator.hasNextPage = () => this.hasNextPage();
    this.paginator.hasPreviousPage = () => this.hasPreviousPage();
    this.paginator.nextPage = () => this.nextPage();
    this.paginator.previousPage = () => this.previousPage();
    this.paginator.firstPage = () => this.firstPage();
    this.paginator.lastPage = () => this.lastPage();
  }

  /**
   * Called by the paginator to determine if there is a previous page to navigate to.
   */
  public hasPreviousPage(): boolean {
    return this.paginator.pageIndex > 0;
  }

  /**
   * Called by the paginator to determine if there is a next page to navigate to.
   */
  public hasNextPage(): boolean {
    return this.paginator.pageIndex + 1 < (this.totalResults / this.paginator.pageSize);
  }

  /**
   * Called by the paginator to navigate to the next page of the leaderboard.
   */
  public nextPage() {
    this.searchedPlayerName = '';
    this.paginator.pageIndex++;
    this.loadData();
  }

  /**
   * Called by the paginator to navigate to the previous page of the leaderboard.
   */
  public previousPage(): void {
    this.searchedPlayerName = '';
    this.paginator.pageIndex--;
    this.loadData();
  }

  /**
   * Called by the paginator to navigate to the first page of the leaderboard.
   */
  public firstPage(): void {
    this.searchedPlayerName = '';
    this.paginator.pageIndex = 0;
    this.loadData();
  }

  /**
   * Called by the paginator to navigate to the last page of the leaderboard.
   */
  public lastPage(): void {
    this.searchedPlayerName = '';
    this.paginator.pageIndex = this.paginator.getNumberOfPages() - 1;
    this.loadData();
  }

  /**
   * Navigates to the given user's profile page. Called when the user's name or avatar is clicked in the template.
   * @param id The ID of the user whose page we will navigate to.
   */
  public goToProfile(id): void {
    this._router.navigate(['user', id]);
  }

}
