import { Component, OnInit } from '@angular/core';
import { PacketService } from '../api/services/packet.service';
import { AuthService } from '../auth/auth.service';
import { PacketResponseDto } from '../api/models/packet-response-dto';

@Component({
  selector: 'app-packet',
  templateUrl: './packet.component.html',
  styleUrls: ['./packet.component.css']
})
export class PacketComponent implements OnInit {
  PacketResponseDto: any;
  email: string = '';
  selectedMonths: number = 0;
  showCardInsertionPopup = false;
  showSuccessAlert = false;
  packetId: number = 0; // Initialize packetId

  cardNumber = '';
  expiryDate = '';
  cvv = '';
  currentPacketName = '';

  constructor(
    private packetService: PacketService,
    public authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadPackets();
  }

  loadPackets(): void {
    this.packetService.getPackets(this.authService.currentUser!.email).subscribe(
      (response: PacketResponseDto) => {
        this.PacketResponseDto = response;
      },
      (error) => {
        console.error('Error fetching packets:', error);
        // Handle error as per your application's requirements
      }
    );
  }

  openCardPopup(packetName: string, selectedMonths: number): void {
    if (!this.authService.isAuthorized()) {
      alert('Please log in to buy a packet.');
      return;
    }
    this.currentPacketName = packetName;
    this.selectedMonths = selectedMonths;
    this.showCardInsertionPopup = true;
    document.body.classList.add('no-scroll'); // Disable scroll
  }

  submitCardDetails(): void {
    if (!this.cardNumber || !this.expiryDate || !this.cvv) {
      alert('Please enter all card details.');
      return;
    }

    const userEmail = this.authService.currentUser?.email || '';
    this.packetService.buyPacket(userEmail, this.currentPacketName, this.selectedMonths)
      .subscribe(
        response => {
          this.showSuccessAlert = true;
          this.closePopup();
          console.log('Packet bought successfully:', response);
          this.updatePacketInfoInSesssion(response.id, response.purchasePercent, response.cancelPercent);
        },
        error => {
          console.error('Error buying packet:', error);
        }
    );
  }

  updatePacketInfoInSesssion(packetId: number, purchasePercent: number, cancelPercent: number): void {
    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      const user: User = JSON.parse(userJson);
      user.packetid = packetId;
      user.purchasepercent = purchasePercent;
      user.cancelpercent = cancelPercent;

      sessionStorage.setItem('CurrentUser', JSON.stringify(user));
    }
  }

  closePopup(): void {
    this.showCardInsertionPopup = false;
    document.body.classList.remove('no-scroll'); // Enable scroll
    setTimeout(location.reload.bind(location), 1000);
  }

  closeSuccessAlert(): void {
    this.showSuccessAlert = false;
  }
  get cardNumberInvalid(): boolean {
    return this.cardNumber.length < 16 || isNaN(Number(this.cardNumber));
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
