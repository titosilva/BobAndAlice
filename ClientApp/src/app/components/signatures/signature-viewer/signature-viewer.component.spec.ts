import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SignatureViewerComponent } from './signature-viewer.component';

describe('SignatureViewerComponent', () => {
  let component: SignatureViewerComponent;
  let fixture: ComponentFixture<SignatureViewerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SignatureViewerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SignatureViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
