import { Component, Input, OnInit } from '@angular/core';
import { SignatureModel } from '../../../api/signature';

@Component({
  selector: 'app-signature-viewer',
  templateUrl: './signature-viewer.component.html',
  styleUrls: ['./signature-viewer.component.scss']
})
export class SignatureViewerComponent implements OnInit {
  @Input()
  signature: SignatureModel;

  constructor() { }

  ngOnInit(): void {
  }

}
