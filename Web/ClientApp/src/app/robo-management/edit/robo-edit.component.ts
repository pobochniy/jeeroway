import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RoboApiService } from '../../shared/api/robo-api.service';
import { roboFormModel } from '../../shared/form-models/robo-form.model';

@Component({
  selector: 'app-robo-edit',
  templateUrl: './robo-edit.component.html',
  styleUrls: ['./robo-edit.component.css'],
  providers: [RoboApiService]
})
export class RoboEditComponent implements OnInit {

  public roboForm = roboFormModel;

  constructor(private service: RoboApiService
    , private router: Router
    , private route: ActivatedRoute
    , private cdRef: ChangeDetectorRef) {
  }

  async ngOnInit() {
    debugger;
    const id = this.route.snapshot.paramMap.get('id');

    const robot = await this.service.Details(id);

    this.cdRef.detectChanges()

    this.roboForm.setValue({
      id: robot.id
      , name: robot.name
      , description: robot.description
    });
  }
}
