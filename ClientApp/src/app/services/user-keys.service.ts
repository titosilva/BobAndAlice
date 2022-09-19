import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserKeyModel, CreateUserKeysRequest as CreateUserKeysRequest, GeneratePrimesResponse } from '../api/user-key';

const apiBase = "/api/keys";

@Injectable({
  providedIn: 'root'
})
export class UserKeysService {

  constructor(
    private http: HttpClient,
  ) { }

  getUserKeys(userId: string): Observable<UserKeyModel[]> {
    return this.http.get<UserKeyModel[]>(`/api/users/${userId}/keys`);
  }

  createKeys(request: CreateUserKeysRequest): Observable<boolean> {
    return this.http.post<boolean>(`${apiBase}`, request);
  }

  generatePrimes(): Observable<GeneratePrimesResponse> {
    return this.http.get<GeneratePrimesResponse>(`${apiBase}/random-primes`);
  }
}
