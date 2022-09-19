import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { FileModel } from '../../../api/files';
import { SignatureService } from '../../../services/signature.service';
import { UserService } from '../../../services/user.service';

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
    private userService: UserService,
    private snackBar: MatSnackBar,
    private router: Router,
  ) { }

  ngOnInit(): void {
  }

  createNewSignature(file: FileModel) {
    this.loading = true;
    this.signatureService.createNewSignature({
      userId: this.userService.user.id,
      file: file,
    }).subscribe(
      signature => {
        this.snackBar.open('Arquivo assinado com sucesso!');
        this.router.navigateByUrl(`/signatures/${signature.id}`);
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
