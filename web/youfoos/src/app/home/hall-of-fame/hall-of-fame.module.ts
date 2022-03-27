import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { HallOfFameComponent } from './hall-of-fame.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [HallOfFameComponent],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([{ path: '', component: HallOfFameComponent }])
  ]
})
export class HallOfFameModule {}
