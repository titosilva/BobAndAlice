import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { UserData, UserModel } from '../api/user';

const apiBase = '/api/users';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  user: UserModel;

  constructor(
    private http: HttpClient,
  ) { }

  createUser(data: UserData): Observable<UserModel> {
    return this.http.post<UserModel>(`${apiBase}`, data);
  }

  login(data: UserData): Observable<UserModel> {
    return this.http.post<UserModel>(`${apiBase}/login`, data)
    .pipe(switchMap(user => {
      this.user = user
      return of(user);
    }));
  }
}
