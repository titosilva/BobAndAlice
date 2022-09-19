import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FileModel } from '../api/files';

const apiRoute = '/api/files';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  constructor(
    private http: HttpClient,
  ) { }

  upload(file: File): Observable<FileModel> {
    const formData = new FormData(); 
    formData.append("file", file, file.name);

    return this.http.post<FileModel>(`${apiRoute}/upload`, formData);
  }
}
