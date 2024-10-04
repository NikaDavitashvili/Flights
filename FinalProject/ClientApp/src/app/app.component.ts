import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'; // Import the Router

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  constructor(public router: Router) { } // Inject the Router

  title = 'SkyConnect';
  currentSlide = 0;
  slides: HTMLElement[] = [];

  ngOnInit() {
    this.slides = Array.from(document.getElementsByClassName('slide') as HTMLCollectionOf<HTMLElement>);
    if (this.slides.length > 0) {
      this.showSlide(this.currentSlide); // Show the initial slide if slides exist
    }
  }

  moveSlides(direction: number): void {
    if (this.slides.length === 0) return; // Prevent running on pages without slides

    // Hide the current slide by removing the active class
    this.slides[this.currentSlide].classList.remove('active');

    // Calculate the next slide index
    this.currentSlide = (this.currentSlide + direction + this.slides.length) % this.slides.length;

    // Show the new slide by adding the active class
    this.slides[this.currentSlide].classList.add('active');
  }

  showSlide(index: number): void {
    if (this.slides.length === 0) return; // Ensure there are slides to display
    this.slides[index].classList.add('active'); // Show slide by index
  }

  // Function to check if the current route is '/map'
  isMapPage(): boolean {
    return this.router.url === '/map'; // Returns true if the current route is '/map'
  }
}
