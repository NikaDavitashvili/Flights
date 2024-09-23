import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _currentUser?: User;
  private authorized = false;

  constructor(private http: HttpClient) {
    this.authorized = sessionStorage.getItem('Authorized') === 'true';
    const storedUser = sessionStorage.getItem('CurrentUser');
    if (storedUser) {
      this._currentUser = JSON.parse(storedUser);
    }
  }

  isAuthorized(): boolean {
    return this.authorized;
  }

  setAuthorized(value: boolean): void {
    this.authorized = value;
    sessionStorage.setItem('Authorized', value.toString());
  }

  get currentUser(): User | undefined {
    return this._currentUser;
  }

  login(Email: string, Password: string): Observable<any> {
    const user = { Email, Password };
    return this.http.post('/api/PassengerController/login', user);
  }

  loginUser(user: User): void {
    this._currentUser = user;
    this.setAuthorized(true);
    sessionStorage.setItem('CurrentUser', JSON.stringify(user));
  }

  logout(): void {
    this._currentUser = undefined;
    this.setAuthorized(false);
    sessionStorage.removeItem('CurrentUser');
    sessionStorage.removeItem('Authorized');
    console.log("auth.service.ts");
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
