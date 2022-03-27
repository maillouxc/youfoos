import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";

@Component({
  selector: 'yf-tournaments',
  templateUrl: './tournaments.component.html',
  styleUrls: ['./tournaments.component.scss']
})
export class TournamentsComponent implements OnInit {

  constructor(private router: Router) {}

  ngOnInit() {
    // If they come to the generic 'tournaments' page, send them to the current tournament page.
    if (this.router.url.endsWith('tournaments')) {
      this.router.navigate(['tournaments/current']);
    }
  }

}
