import { Component, OnInit } from '@angular/core';
import { PacketService } from '../api/services/packet.service';
import { AuthService } from '../auth/auth.service';
import { PacketRm } from '../api/models/packet';

@Component({
  selector: 'app-packet',
  templateUrl: './packet.component.html',
  styleUrls: ['./packet.component.css']
})
export class PacketComponent implements OnInit {
  packets: any[] = [];
  email: string = '';
  selectedMonths: number = 0; // Initialize with default value

  constructor(
    private packetService: PacketService,
    public authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadPackets();
  }

  loadPackets(): void {
    this.packetService.getPackets().subscribe(
      (data: PacketRm[]) => {
        this.packets = data;
      },
      (error) => {
        console.error('Error fetching packets:', error);
        // Handle error as per your application's requirements
      }
    );
  }

  getCardClass(packetName: string): string {
    // Example logic, customize based on your packet names
    switch (packetName.toLowerCase()) {
      case 'silver':
        return 'bg-silver';
      case 'golden':
        return 'bg-golden';
      case 'platinum':
        return 'bg-platinum';
      default:
        return '';
    }
  }

  buyPacket(userEmail: string, packetName: string, months: number): void {
    if (!this.authService.isAuthorized()) {
      alert('Please log in to buy a packet.');
      return;
    }

    this.packetService.buyPacket(userEmail, packetName, months)
      .subscribe(
        response => {
          console.log('Packet bought successfully:', response);
          // Optionally, update UI or navigate to a different page upon success
        },
        error => {
          console.error('Error buying packet:', error);
          // Optionally, handle errors and display to the user
        }
      );
  }
}
