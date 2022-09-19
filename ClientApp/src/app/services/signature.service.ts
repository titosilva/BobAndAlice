import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FileModel } from '../api/files';
import { SignatureModel } from '../api/signature';

const apiBase = "/api/signatures";

@Injectable({
  providedIn: 'root'
})
export class SignatureService {

  constructor(
    private http: HttpClient,
  ) { }

  createNewSignature(file: FileModel): Observable<SignatureModel> {
    return this.http.post<SignatureModel>(`${apiBase}`, file);
  }
}
