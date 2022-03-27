import { Component } from '@angular/core';

@Component({
  selector: 'yf-about-page',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent {
  contributorNames = [
    'Chris Mailloux',
    'Bryan Cordes',
    'Chris Dreiser',
    'Jeffrey Fleurent',
    'Edgar Meruvia Garron',
    'Juan Gomez',
    'Brandon Walters',
    'Brett Dube'
  ];

  contributorNamesv2 = [
    'Name coming Soon',
    'Name coming Soon',
    'Name coming Soon',
    'Name coming Soon'
  ];

  // lol
  specialPeople = [
    'Felix Lepa',
    'Zach Sadler',
    'Chase Keenan',
    'Max King',
    'Wyatt Baggett',
    'Fran Flores'
  ];

  constructor() {}
}
