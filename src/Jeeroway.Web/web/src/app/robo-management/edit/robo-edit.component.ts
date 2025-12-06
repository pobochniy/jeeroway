import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { RoboApiService } from '../../../shared/api/robo-api.service';
import { roboFormModel } from '../../../shared/form-models/robo-form.model';
import { FormValidationComponent } from '../../../shared/form-validation/form-validation.component';

@Component({
  selector: 'app-robo-edit',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FormValidationComponent],
  templateUrl: './robo-edit.component.html',
  styleUrls: ['./robo-edit.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoboEditComponent implements OnInit {

  public roboForm = roboFormModel;

  constructor(private service: RoboApiService
    , private router: Router
    , private route: ActivatedRoute) {
  }

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    if (id) {
      const robot = await this.service.Details(id);
      this.roboForm.setValue({
        id: robot.id
        , name: robot.name
        , description: robot.description
      });
    }
  }

  async onSubmit() {
    this.roboForm.markAllAsTouched();

    try {
      if (this.roboForm.valid) {
        const goToList = !!this.roboForm.value['id'];
        const res = await this.service.Update(this.roboForm);
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
