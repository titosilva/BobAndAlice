import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CreateComponent } from './components/user/create/create.component';
import { LoginComponent } from './components/user/login/login.component';


const routes: Routes = [
  {
    path: 'user/create',
    component: CreateComponent,
  },
  {
    path: 'user/login',
    component: LoginComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
