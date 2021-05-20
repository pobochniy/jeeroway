import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RoboApiService } from '../../shared/api/robo-api.service';
import { RoboModel } from '../../shared/models/robo.model';

@Component({
  selector: 'app-robo-details',
  templateUrl: './robo-details.component.html',
  styleUrls: ['./robo-details.component.css'],
  providers: [RoboApiService]
})
export class RoboDetailsComponent implements OnInit {

  robot: RoboModel;

  constructor(private service: RoboApiService
    , private router: Router
    , private route: ActivatedRoute) {
  }

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    this.robot = await this.service.Details(id);
  }
}
