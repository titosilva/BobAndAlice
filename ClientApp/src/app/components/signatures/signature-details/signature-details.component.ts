import { Component, Input, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { SignatureModel } from '../../../api/signature';
import { SignatureService } from '../../../services/signature.service';

@Component({
  selector: 'app-signature-details',
  templateUrl: './signature-details.component.html',
  styleUrls: ['./signature-details.component.scss']
})
export class SignatureDetailsComponent implements OnInit {
  loading: boolean;
  signature: SignatureModel;

  constructor(
    private route: ActivatedRoute,
    private signatureService: SignatureService,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
    this.loading = true;

    const signatureId = this.route.snapshot.params.id;

    this.signatureService.getSignature(signatureId).subscribe(
      signature => {
        this.signature = signature;
        this.loading = false;
      }, err => {
        if (err.status == 418) {
          this.snackBar.open(err.error.message);
        } else {
          this.snackBar.open("Não foi possível ler a assinatura");
        }
        
        this.loading = false;
      }
    );
  }

}
