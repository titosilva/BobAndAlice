import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, ReplaySubject, Subject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { LoginRequest, UserData, UserModel } from '../api/user';

const apiBase = '/api/users';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private loggedSubject: Subject<UserModel> = new ReplaySubject<UserModel>(null);
  whenLogged: Observable<UserModel> = this.loggedSubject.asObservable();

  get user(): UserModel {
    return this.getStoredSession();
  }

  get isLogged(): boolean {
    return this.user != null;
  }

  constructor(
    private http: HttpClient,
  ) { }

  private getStoredSession(): UserModel {
    try {
      return JSON.parse(localStorage.getItem('bobandalice__user'));
    } catch {
      return null;
    }
  }

  private storeSession(user: UserModel) {
    localStorage.setItem('bobandalice__user', JSON.stringify(user));
    this.loggedSubject.next(user);
  }

  createUser(data: UserData): Observable<UserModel> {
    return this.http.post<UserModel>(`${apiBase}`, data);
  }

  login(data: LoginRequest): Observable<UserModel> {
    return this.http.post<UserModel>(`${apiBase}/login`, data)
    .pipe(switchMap(user => {
      this.storeSession(user);
      return of(user);
    }));
  }
}
