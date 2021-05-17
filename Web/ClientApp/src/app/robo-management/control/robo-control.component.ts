import { Component, OnInit } from '@angular/core';
import { UsersApiService } from '../../shared/api/users-api.service';

@Component({
  selector: 'app-robo-control',
  templateUrl: './robo-control.component.html',
  styleUrls: ['./robo-control.component.css']
})
export class RoboControlComponent implements OnInit {


  constructor(private service: UsersApiService) {
  }

  async ngOnInit() {
    
  }
}
