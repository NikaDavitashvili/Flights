import { Component, OnInit } from '@angular/core';
import { FlightService } from './../api/services/flight.service';
import { BookingService } from './../api/services/booking.service';
import { FlightRm } from '../api/models';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-flights',
  templateUrl: './search-flights.component.html',
  styleUrls: ['./search-flights.component.css']
})
export class SearchFlightsComponent implements OnInit {

  searchResult: FlightRm[] = [];
  currentUser: User | null = null;
  searchForm: FormGroup;
  seasonName: string | null = null;
  departureDateType: string = 'date';  // 'date' or 'month'
  returnDateType: string = 'date';     // 'date' or 'month'
  faqs = [
    {
      id: 0,
      question: 'How does SkyConnect work?',
      answer: 'SkyConnect searches multiple airlines to find the best flight options for you, offering a seamless booking experience.',
      isOpen: false
    },
    {
      id: 1,
      question: 'Can I book my flight with SkyConnect?',
      answer: 'Yes, SkyConnect allows you to book flights directly on our platform after finding the best options.',
      isOpen: false
    },
    {
      id: 2,
      question: 'How do I find the cheapest flights?',
      answer: 'Use our flexible search options and filter tools to find the best prices across different dates and airlines.',
      isOpen: false
    },
    {
      id: 3,
      question: 'What happens after I book my flight?',
      answer: 'Once you complete your booking, you will receive a confirmation email with all the necessary details and a link to manage your booking.',
      isOpen: false
    },
    {
      id: 4,
      question: 'Does SkyConnect offer customer support?',
      answer: 'Yes, we provide 24/7 customer support to assist with any booking or travel-related questions you may have.',
      isOpen: false
    }
  ];
  constructor(private flightService: FlightService, private bookService: BookingService,
    private fb: FormBuilder,
    private router: Router) {

    this.searchForm = this.fb.group({
      from: [''],
      destination: [''],
      departureDateType: ['date'], // Selector for choosing date type for departure
      fromDate: [''],              // Exact date for departure
      fromMonth: [''],             // Month input for departure
      returnDateType: ['date'],    // Selector for choosing date type for return
      toDate: [''],                // Exact date for return
      toMonth: [''],               // Month input for return
      numberOfPassengers: [1]
    });
  }

  ngOnInit(): void {
    // Set all faqs to be closed initially.
    this.faqs.forEach(faq => faq.isOpen = false);

    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      this.currentUser = JSON.parse(userJson);
    }
  }

  toggleFaq(index: number) {
    this.faqs.forEach((faq, i) => {
      if (i === index) {
        faq.isOpen = !faq.isOpen;

        // Dynamically adjust max-height for smooth transitions
        setTimeout(() => {
          const element = document.getElementById('collapse' + faq.id);
          if (element) {
            if (faq.isOpen) {
              element.style.maxHeight = element.scrollHeight + 'px'; // Smoothly expand to fit content
            } else {
              element.style.maxHeight = '0px'; // Collapse smoothly
            }
          }
        }, 0);
      } else {
        faq.isOpen = false;
        const otherElement = document.getElementById('collapse' + faq.id);
        if (otherElement) {
          otherElement.style.maxHeight = '0px';
        }
      }
    });
  }

  // Method to update the form when departure date type changes
  onDepartureDateTypeChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.departureDateType = selectElement.value;  // Correctly get the value
    if (this.departureDateType === 'date') {
      this.searchForm.get('fromMonth')?.reset();
    } else {
      this.searchForm.get('fromDate')?.reset();
    }
  }

  // Method to update the form when return date type changes
  onReturnDateTypeChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.returnDateType = selectElement.value;  // Correctly get the value
    if (this.returnDateType === 'date') {
      this.searchForm.get('toMonth')?.reset();
    } else {
      this.searchForm.get('toDate')?.reset();
    }
  }


  // Search method that sends the correct date format to the backend
  search(seasonName?: string): void {
    if (seasonName) {
      this.seasonName = seasonName;
    }

    const formValues = this.searchForm.value;

    // Capture departure date or month properly
    let departureDate = '';
    if (formValues.departureDateType === 'date') {
      departureDate = formValues.fromDate;  // Use exact date if selected
    } else {
      departureDate = formValues.fromMonth; // Use month if selected
    }

    // Capture return date or month properly
    let returnDate = '';
    if (formValues.returnDateType === 'date') {
      returnDate = formValues.toDate;       // Use exact date if selected
    } else {
      returnDate = formValues.toMonth;      // Use month if selected
    }

    const searchParams = {
      from: formValues.from,
      destination: formValues.destination,
      fromDate: departureDate,
      toDate: returnDate,
      numberOfPassengers: formValues.numberOfPassengers,
      seasonName: this.seasonName || undefined
    };

    this.router.navigate(['/flights'], {
      queryParams: {
        from: formValues.from,
        destination: formValues.destination,
        departureDateType: formValues.departureDateType,
        departureDate: departureDate,  // Pass departure date
        returnDateType: formValues.returnDateType,
        returnDate: returnDate,        // Pass return date
        numberOfPassengers: formValues.numberOfPassengers,
      }
    });

    // Send request to the backend
    //this.flightService.searchFlight(searchParams).subscribe(
    //  response => this.searchResult = response,
    //  err => this.handleError(err)
    //);


    this.seasonName = null;
  }


  // Booking method that navigates to the booking page
  bookFlight(flight: FlightRm): void {
    this.bookService.setFlight(flight);  // Store flight data in service
    this.router.navigate(['/book-flight']);
  }

  // Error handling
  private handleError(err: any): void {
    console.log("Response Error. Status: ", err.status);
    console.log("Response Error. Status Text: ", err.statusText);
    console.log(err);
  }

  // Calculate discounted price based on user data
  getDiscountedPrice(price: any): number | null {
    if (this.currentUser && this.currentUser.packetid !== 1) {
      var discountedPrice = (price * (1 - this.currentUser.purchasepercent / 100));
      return Math.round(discountedPrice);
    }
    return null;
  }
}

interface User {
  email: string;
  password: string;
  username: string;
  packetid: number;
  purchasepercent: number;
  cancelpercent: number;
}
