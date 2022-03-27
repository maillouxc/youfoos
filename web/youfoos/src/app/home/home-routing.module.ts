import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home.component';
import { AuthGuard } from '../shared/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    canLoad: [AuthGuard],
    component: HomeComponent,
    children: [
      {
        path: '',
        children: [
          {
            path: 'games',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./games/games.module').then(m => m.GamesModule)
          },
          {
            path: 'tournaments',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./tournaments/tournaments.module').then(m => m.TournamentsModule)
          },
          {
            path: 'profile',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./profile/profile.module').then(m => m.ProfileModule)
          },
          {
            path: 'user/:id',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./profile/profile.module').then(m => m.ProfileModule)
          },
          {
            path: 'leaderboard',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./leaderboard/leaderboard.module').then(
                m => m.LeaderboardModule
              )
          },
          {
            path: 'hall-of-fame',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./hall-of-fame/hall-of-fame.module').then(
                m => m.HallOfFameModule
              )
          },
          {
            path: 'help',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./help/help.module').then(m => m.HelpModule)
          },
          {
            path: 'about',
            canLoad: [AuthGuard],
            loadChildren: () =>
              import('./about/about.module').then(m => m.AboutModule)
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule {}
