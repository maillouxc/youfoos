import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ProfileComponent } from './containers/profile.component';
import { ProfileRecentGamesComponent } from './components/profile-recent-games/profile-recent-games.component';
import { ProfileSummaryComponent } from './components/profile-summary/profile-summary.component';
import { ProfileStatsComponent } from './components/profile-stats/profile-stats.component';
import { UserAchievementsComponent } from './components/user-achievements/user-achievements.component';
import { DialogChangeRfidComponent } from './components/change-rfid/change-rfid.component';
import { DialogChangePasswordComponent } from './components/change-password/dialog-change-password.component';
import { DialogAvatarUploadComponent } from './components/avatar-upload/dialog-avatar-upload.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    ProfileComponent,
    ProfileRecentGamesComponent,
    ProfileSummaryComponent,
    ProfileStatsComponent,
    UserAchievementsComponent,
    DialogChangeRfidComponent,
    DialogChangePasswordComponent,
    DialogAvatarUploadComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([{ path: '', component: ProfileComponent }])
  ]
})
export class ProfileModule { }
