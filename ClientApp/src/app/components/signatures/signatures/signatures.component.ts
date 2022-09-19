import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { FileModel } from '../../../api/files';
import { SignatureService } from '../../../services/signature.service';

@Component({
  selector: 'app-signatures',
  templateUrl: './signatures.component.html',
  styleUrls: ['./signatures.component.scss']
})
export class SignaturesComponent implements OnInit {
  loading: boolean = false;

  constructor(
    public route: ActivatedRoute,
    private signatureService: SignatureService,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
  }

  createNewSignature(file: FileModel) {
    this.loading = true;
    this.signatureService.createNewSignature(file).subscribe(
      _ => {
        this.snackBar.open('Arquivo assinado com sucesso!');
        this.loading = false;
      }, err => {
        if (err.status == 418) {
          this.snackBar.open(err.error.message);
        } else {
          this.snackBar.open('Não foi possível assinar o arquivo');
        }

        this.loading = false;
      }
    )
  }
}
