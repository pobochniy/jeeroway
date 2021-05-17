import { Component, OnInit } from '@angular/core';
import { UsersApiService } from '../../shared/api/users-api.service';

@Component({
  selector: 'app-robo-details',
  templateUrl: './robo-details.component.html',
  styleUrls: ['./robo-details.component.css']
})
export class RoboDetailsComponent implements OnInit {


  constructor(private service: UsersApiService) {
  }

  async ngOnInit() {
    
  }
}
