import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserKeyModel } from '../../../api/user-key';
import { UserKeysService } from '../../../services/user-keys.service';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-keys-list',
  templateUrl: './keys-list.component.html',
  styleUrls: ['./keys-list.component.scss']
})
export class KeysListComponent implements OnInit {
  loading: boolean = false;

  userKeys: UserKeyModel[] = [];

  constructor(
    private userKeysService: UserKeysService,
    private userService: UserService,
    private snackBar: MatSnackBar,
  ) { }

  ngOnInit(): void {
    this.loading = true;
    this.userKeysService.getUserKeys(this.userService.user.id).subscribe(
      keys => {
        this.userKeys = keys;
        this.loading = false;
      }, err => {
        this.loading = false;
        this.snackBar.open('Não foi possível obter as chaves do usuário');
      }
    )
  }

}
