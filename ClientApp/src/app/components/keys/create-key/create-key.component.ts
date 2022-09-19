import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { UserKeysService } from '../../../services/user-keys.service';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-create-key',
  templateUrl: './create-key.component.html',
  styleUrls: ['./create-key.component.scss']
})
export class CreateKeyComponent implements OnInit {
  loading: boolean = false;
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userKeyService: UserKeysService,
    private userService: UserService,
    private snackBar: MatSnackBar,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      prime1: ['', [Validators.required]],
      prime2: ['', [Validators.required]],
    });
  }

  getError(controlName: string) {
    if (this.loading) {
      return null;
    }
    
    const control = this.form.get(controlName);

    if (control.hasError('required')) {
      return 'Este campo é obrigatório';
    }

    return null;
  }

  generatePrimes() {
    this.loading = true;
    this.userKeyService.generatePrimes().subscribe(
      primes => {
        this.form.setValue({
          prime1: primes.prime1Base64,
          prime2: primes.prime2Base64,
        });
        this.loading = false;
      }, err => {
        this.snackBar.open('Não foi possível completar a operação');
        this.loading = false;
      }
    )
  }

  createKeys() {
    if (!this.form.valid) {
      return;   
    }

    this.loading = true;
    this.userKeyService.createKeys({
      userId: this.userService.user.id,
      prime1Base64: this.form.value.prime1,
      prime2Base64: this.form.value.prime2,
    }).subscribe(
      _ => {
        this.snackBar.open('Chaves geradas com sucesso!');
        this.router.navigateByUrl('/keys');
      }, err => {
        this.loading = false;
        if (err.status == 418) {
          this.snackBar.open(err.error.message);
        } else {
          this.snackBar.open('Não foi possível gerar as chaves');
        }
      }
    )
  }
}
