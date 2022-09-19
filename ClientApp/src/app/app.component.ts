import { Component, OnInit } from '@angular/core';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  isLogged: boolean;

  notLoggedRoutes = [{
    name: 'Login',
    ref: '/user/login',
  }, {
    name: 'Criar usuário',
    ref: '/user/create',
  }];

  loggedRoutes = [ {
    name: 'Minhas chaves',
    ref: '/keys',
  }, {
    name: 'Minhas assinaturas',
    ref: '/signatures',
  }, {
    name: 'Verificar assinatura',
    ref: '/verification',
  }];

  constructor(
    private userService: UserService,
  ) { }

  ngOnInit(): void {
    if (this.userService.isLogged) {
      this.isLogged = true;
    } else {
      this.userService.whenLogged.subscribe(
        user => this.isLogged = user != null,
      );
    }
  }
}
