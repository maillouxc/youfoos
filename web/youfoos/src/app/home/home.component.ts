import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  template: `
    <yf-header>
      <router-outlet></router-outlet>
    </yf-header>
  `,
  styleUrls: []
})
export class HomeComponent implements OnInit {

  constructor() {}

  ngOnInit() {}

}
