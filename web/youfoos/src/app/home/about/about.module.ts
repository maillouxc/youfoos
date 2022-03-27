import { NgModule } from '@angular/core';
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";

import { AboutComponent } from "./about.component";
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  imports: [
    SharedModule,
    CommonModule,
    RouterModule.forChild([{ path: '', component: AboutComponent }])
  ],
  declarations: [AboutComponent]
})
export class AboutModule { }
