import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FileModel } from '../../../api/files';
import { SignatureModel } from '../../../api/signature';
import { SignatureService } from '../../../services/signature.service';

@Component({
  selector: 'app-verification',
  templateUrl: './verification.component.html',
  styleUrls: ['./verification.component.scss']
})
export class VerificationComponent implements OnInit {
  signature: SignatureModel;
  file: FileModel;
  loading: boolean;

  constructor(
    private signatureService: SignatureService,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
  }

  openAndVerify(file: FileModel) {
    this.signature = null;

    this.loading = true;
    this.signatureService.openAndVerifySignatureFromFile(file.id).subscribe(
      signature => {
        this.file = file;
        this.signature = signature;
        this.loading = false;
      }, err => {
        if (err.status == 418) {
          this.snackBar.open(err.error.message);
        } else {
          this.snackBar.open("Não foi possível realizar a operação");
        }
        this.loading = false;
      }
    )
  }

  downloadDecrypted() {
    window.open(`https://localhost:5001/api/signatures/from-file/download?fileId=${this.file.id}&fileName=${this.file.fileName}`);
  }
}
