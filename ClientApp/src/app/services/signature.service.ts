import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FileModel } from '../api/files';
import { SignatureData, SignatureModel } from '../api/signature';

const apiBase = "/api/signatures";

@Injectable({
  providedIn: 'root'
})
export class SignatureService {

  constructor(
    private http: HttpClient,
  ) { }

  createNewSignature(data: SignatureData): Observable<SignatureModel> {
    return this.http.post<SignatureModel>(`${apiBase}`, data);
  }

  getSignature(id: string): Observable<SignatureModel> {
    return this.http.get<SignatureModel>(`${apiBase}/${id}`);
  }
}
