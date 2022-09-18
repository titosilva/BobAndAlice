import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.scss']
})
export class CreateComponent implements OnInit {
  form: FormGroup;

  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      cpf: ['', [Validators.minLength(11), Validators.maxLength(11), Validators.pattern(/\d{11}/)]],
      password: ['', [Validators.required]],
    });
  }

  getError(controlName: string) {
    const control = this.form.get(controlName);

    if (control.hasError('minlength')) {
      return 'Esse campo deve ter 11 dígitos';
    } else if (control.hasError('maxlength')) {
      return 'Esse campo deve ter 11 dígitos';
    } else if (control.hasError('pattern')) {
      return 'Esse campo deve conter apenas números';
    } else if (control.hasError('required')) {
      return 'Esse campo é obrigatório';
    }
    
    return null;
  }

  submit() {
    if (!this.form.valid) {
      return;
    }

    this.userService.createUser({
      cpf: this.form.value.cpf,
      password: this.form.value.password,
    }).subscribe(
      user => {
        this.snackBar.open('Usuário criado com sucesso!');
      }, err => {
        this.snackBar.open('Não foi possível criar o usuário');
      }
    )
  }
}
