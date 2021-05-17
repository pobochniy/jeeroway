import { Component, OnInit } from '@angular/core';
import { UsersApiService } from '../../shared/api/users-api.service';

@Component({
  selector: 'app-robo-list',
  templateUrl: './robo-list.component.html',
  styleUrls: ['./robo-list.component.css']
})
export class RoboListComponent implements OnInit {


  constructor(private service: UsersApiService) {
  }

  async ngOnInit() {
    
  }
  ed
}
