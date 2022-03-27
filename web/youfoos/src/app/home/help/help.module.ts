import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { HelpComponent } from './help.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [HelpComponent],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([{ path: '', component: HelpComponent }])
  ]
})
export class HelpModule { }
