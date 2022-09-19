import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';

import { NgxLoadingModule } from 'ngx-loading';

import { CreateComponent } from './components/user/create/create.component';
import { LoginComponent } from './components/user/login/login.component';
import { APIInterceptor } from './api/api-interceptor';
import { SignaturesComponent } from './components/signatures/signatures/signatures.component';
import { KeysComponent } from './components/keys/keys/keys.component';
import { VerificationComponent } from './components/verification/verification/verification.component';
import { KeysListComponent } from './components/keys/keys-list/keys-list.component';
import { CreateKeyComponent } from './components/keys/create-key/create-key.component';
import { SignatureListComponent } from './components/signatures/signature-list/signature-list.component';
import { CreateSignatureComponent } from './components/signatures/create-signature/create-signature.component';
import { FileUploadComponent } from './components/file/file-upload/file-upload.component';

@NgModule({
  declarations: [
    AppComponent,
    CreateComponent,
    LoginComponent,
    SignaturesComponent,
    KeysComponent,
    VerificationComponent,
    KeysListComponent,
    CreateKeyComponent,
    SignatureListComponent,
    CreateSignatureComponent,
    FileUploadComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatSidenavModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatSnackBarModule,
    MatListModule,
    MatDividerModule,
    MatTableModule,

    NgxLoadingModule.forRoot({}),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: APIInterceptor,
      multi: true,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
