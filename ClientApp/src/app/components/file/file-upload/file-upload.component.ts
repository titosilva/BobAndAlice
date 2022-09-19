import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FileModel } from '../../../api/files';
import { FileUploadService } from '../../../services/file-upload.service';

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent implements OnInit {
  loading: boolean = false;
  file: File;

  @Output()
  onFileUploaded: EventEmitter<FileModel> = new EventEmitter<FileModel>();

  @Input()
  content: string = 'Selecionar arquivo';

  @Input()
  icon: string = null;

  @ViewChild('fileInput')
  fileInput: Element;

  constructor(
    private fileUploadService: FileUploadService,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
  }

  onChange(event) {
    this.loading = true;
    this.file = event.target.files[0];
    this.upload();
  }

  upload() {
    this.fileUploadService.upload(this.file).subscribe(
      model => {
        this.onFileUploaded.emit(model);
        this.loading = false;
      }, err => {
        if (err.status == 418) {
          this.snackBar.open(err.error.message);
        } else {
          this.snackBar.open('Não foi possível enviar o arquivo');
        }
        this.loading = false;
      }
    );
  }

}
