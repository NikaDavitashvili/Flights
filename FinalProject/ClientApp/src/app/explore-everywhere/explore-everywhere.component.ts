import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-explore-everywhere',
  templateUrl: './explore-everywhere.component.html',
  styleUrls: ['./explore-everywhere.component.css']
})
export class ExploreEverywhereComponent implements OnInit {

  popularFlights = [
    { destination: 'United States', price: 285, image: '/assets/images/us.png' },
    { destination: 'Italy', price: 73, image: '/assets/images/italy.png' },
    { destination: 'Greece', price: 120, image: '/assets/images/greece.png' },
    { destination: 'New Zealand', price: 359, image: '/assets/images/nz.png' },
    { destination: 'Japan', price: 192, image: '/assets/images/japan.png' },
    { destination: 'Zanzibar', price: 402, image: '/assets/images/zanzibar.png' },
    { destination: 'Egypt', price: 116, image: '/assets/images/egypt.png' },
    { destination: 'Czech Republic', price: 98, image: '/assets/images/czech.png' }
  ];

  constructor() { }

  ngOnInit(): void { }
}
