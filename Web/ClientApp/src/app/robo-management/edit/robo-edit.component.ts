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
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      const robot = await this.service.Details(id);

      this.cdRef.detectChanges()

      this.roboForm.setValue({
        id: robot.id
        , name: robot.name
        , description: robot.description
      });
    }
  }

  async onSubmit() {
    for (let item in this.roboForm.controls) {
      this.roboForm.controls[item].markAsDirty();
    }

    try {
      if (this.roboForm.valid) {
        const goToList = !!this.roboForm.value['id'];
        const res = await this.service.Update(this.roboForm);
        debugger;
        if (goToList) {
          this.router.navigateByUrl('/robo/list');
        }
        else {
          this.router.navigateByUrl('/robo/details/' + res.id);
        }
      }
    }
    catch {
      alert('Возникли непредвиденные ошибки. Попробуйте ввести другие значения или сообщите программисту');
    }
  }
}
