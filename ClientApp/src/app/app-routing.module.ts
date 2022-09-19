import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CreateKeyComponent } from './components/keys/create-key/create-key.component';
import { KeysComponent } from './components/keys/keys/keys.component';
import { SignatureDetailsComponent } from './components/signatures/signature-details/signature-details.component';
import { SignaturesComponent } from './components/signatures/signatures/signatures.component';
import { CreateComponent } from './components/user/create/create.component';
import { LoginComponent } from './components/user/login/login.component';
import { VerificationComponent } from './components/verification/verification/verification.component';
import { LoggedInGuard } from './guards/logged-in.guard';


const routes: Routes = [
  { path: '', redirectTo: '/keys', pathMatch: 'full', canActivate: [LoggedInGuard] },
  { path: '', redirectTo: '/user/login', pathMatch: 'full' },
  {
    path: 'user/create',
    component: CreateComponent,
  },
  {
    path: 'user/login',
    component: LoginComponent,
  },
  {
    path: 'keys',
    component: KeysComponent,
    canActivate: [LoggedInGuard],
    children: [
      {
        path: 'create',
        component: CreateKeyComponent,
      },
    ],
  },
  {
    path: 'verification',
    component: VerificationComponent,
    canActivate: [LoggedInGuard],
  },
  {
    path: 'signatures',
    component: SignaturesComponent,
    canActivate: [LoggedInGuard],
    children: [
      {
        path: ':id',
        component: SignatureDetailsComponent,
      }
    ],
  },
  {
    path: '**',
    component: KeysComponent,
    canActivate: [LoggedInGuard],
  },
  { 
    path: '**', 
    component: LoginComponent,
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
