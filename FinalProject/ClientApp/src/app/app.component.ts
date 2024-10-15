import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  constructor(public router: Router) { }

  title = 'SkyConnect';
  currentSlide = 0;
  slides: HTMLElement[] = [];
  autoSlideInterval: any; // Store the interval ID for the auto slider

  ngOnInit() {
    this.slides = Array.from(document.getElementsByClassName('slide') as HTMLCollectionOf<HTMLElement>);
    if (this.slides.length > 0) {
      this.showSlide(this.currentSlide);
      this.startAutoSlide(); // Start auto slide on initialization
    }
  }

  ngOnDestroy() {
    this.stopAutoSlide(); // Clear the interval when the component is destroyed
  }

  startAutoSlide() {
    // Automatically change slides every 5 seconds (5000ms)
    this.autoSlideInterval = setInterval(() => {
      this.moveSlides(1); // Move to the next slide
    }, 5000);
  }

  stopAutoSlide() {
    if (this.autoSlideInterval) {
      clearInterval(this.autoSlideInterval);
    }
  }

  moveSlides(direction: number): void {
    if (this.slides.length === 0) return;

    // Remove active class from the current slide
    this.slides[this.currentSlide].classList.remove('active');

    // Calculate the next slide index
    this.currentSlide = (this.currentSlide + direction + this.slides.length) % this.slides.length;

    // Add active class to the new slide
    this.slides[this.currentSlide].classList.add('active');
  }

  showSlide(index: number): void {
    if (this.slides.length === 0) return;
    this.slides[index].classList.add('active');
  }

  isMapPage(): boolean {
    return this.router.url === '/map';
  }
}
