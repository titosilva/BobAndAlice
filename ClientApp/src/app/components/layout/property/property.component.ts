import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-property',
  templateUrl: './property.component.html',
  styleUrls: ['./property.component.scss']
})
export class PropertyComponent implements OnInit {
  @Input()
  name: string;

  constructor() { }

  ngOnInit(): void {
  }

}
