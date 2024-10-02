import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit {

  @Input() message: string = '';
  @Input() type: 'success' | 'error' | 'info' | 'warning' = 'info';

  ngOnInit(): void {
    if (this.message) {
      setTimeout(() => {
        this.message = '';
      }, 5000); // Hide after 5 seconds
    }
  }
}
