import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSignatureComponent } from './create-signature.component';

describe('CreateSignatureComponent', () => {
  let component: CreateSignatureComponent;
  let fixture: ComponentFixture<CreateSignatureComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateSignatureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateSignatureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
